using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebWeather.Models;
using Microsoft.EntityFrameworkCore;
using WebWeather.DataAccess;
using WebWeather.DataAccess.Models;
using WebWeather.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using WebWeather.Models.Excel;
using WebWeather.Services.Helper;

namespace WebWeather.Controllers
{
    public class WeatherController : Controller
    {
        private readonly ILogger<WeatherController> _logger;
        public WeatherController(ILogger<WeatherController> logger)
        {
            _logger = logger;
        }

        public IActionResult ExcelLoader()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFileCollection excelFiles)
        {
            try
            {
                var excelErrors = await WeatherService.LoadWeathersExcelToDb(excelFiles); //Действие

                if (excelErrors?.Count == 0)
                {
                    _logger.LogInformation($"Controller{nameof(WeatherController)}. Загрузка файлов в бд успешно завершилась.");//Действие
                    return Ok();
                }
                else
                {
                    var modelStateWithErros = GetErrorsAboutParsingOfWeathers(excelErrors, ModelState);// Вычисление
                    return BadRequest(modelStateWithErros);
                }
            }
            catch (Exception ex)
            {
                //действие лог зависит от внешнего вызов(файла с ошибкой)
                _logger.LogError($"Controller{nameof(WeatherController)}. Error: {ex.Message}.");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Weathers([FromQuery]WeathersFilter weathersFilter)
        {
            try
            {
                weathersFilter = await CalulateWeathersFilter(weathersFilter); // Действие
                WeathersViewModel viewModel = WeathersViewModel.CreateWeathersViewModel(weathersFilter); // Вычисление по созданию данных
                return View(viewModel);
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }

        private static async Task<WeathersFilter> CalulateWeathersFilter(WeathersFilter weathersFilter)
        {
            using var dataWeatherContext = WeatherContextFactory.CreateDbContext(); // Действие

            IQueryable<Weather> weatherQuery = GetWeatherQuery(dataWeatherContext); // Вычисление - формирование дерева запроса
            weatherQuery = FilterWeathersByDate(weathersFilter, weatherQuery); // Вычисление
            weatherQuery = SortWeathersByOrderType(weathersFilter, weatherQuery);      // Вычисление

            int count = await GetCountOfWeathers(weatherQuery); //Действие
            List<Weather> weathersSlice = await GetSliceOfWeathers(weathersFilter, weatherQuery);//Действие

            weathersFilter = SetCountForFilter(weathersFilter, count); //Вычисление
            weathersFilter = SetWeathersSliceForFilter(weathersFilter, weathersSlice); //Вычисление

            return weathersFilter;
        }

        private static WeathersFilter SetWeathersSliceForFilter(WeathersFilter weathersFilter, List<Weather> weathersSlice)
        {
            weathersFilter = weathersFilter with { weathersSlice = weathersSlice.CloneItems()};
            return weathersFilter;
        }

        //метод 4го слоя
        private static WeathersFilter SetCountForFilter(WeathersFilter weathersFilter, int count)
        {
            weathersFilter = weathersFilter with { count = count }; // Создаём копию объекта на основе исходного
            return weathersFilter;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private static async Task<List<Weather>> GetSliceOfWeathers(WeathersFilter weathersFilter, IQueryable<Weather> weatherQuery)
        {
            var page = weathersFilter.page;
            var pageSize = weathersFilter.pageSize;
            var newWeatherQuery = weatherQuery.AsQueryable();//Копия

            return await newWeatherQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        private static async Task<int> GetCountOfWeathers(IQueryable<Weather> weatherQuery)
        {
            return await weatherQuery.CountAsync();
        }

        private static IQueryable<Weather> GetWeatherQuery(DataWeatherContext dataWeatherContext)
        {
            return dataWeatherContext.Weather.AsQueryable();
        }

        private static IQueryable<Weather> FilterWeathersByDate(WeathersFilter weathersFilter, IQueryable<Weather> weatherQuery)
        {
            var year = weathersFilter.year;
            var month = weathersFilter.month;
            var newWeatherQuery = weatherQuery.AsQueryable();//Копия

            if (year != null)
            {
                newWeatherQuery = newWeatherQuery.Where(w => w.Date.Year == year);
            }
            if (month != null)
            {
                newWeatherQuery = newWeatherQuery.Where(w => w.Date.Month == month);
            }

            return newWeatherQuery;
        }

        private static IQueryable<Weather> SortWeathersByOrderType(WeathersFilter weathersFilter, IQueryable<Weather> weatherQuery)
        {
            var newWeatherQuery = weatherQuery.AsQueryable();//Копия
            switch (weathersFilter.sortOrder)
            {
                case SortState.DateAsc:
                    weatherQuery = newWeatherQuery.OrderBy(w => w.Date).ThenBy(w => w.Time);
                    break;
                case SortState.DateDesc:
                    weatherQuery = newWeatherQuery.OrderByDescending(w => w.Date).ThenByDescending(w => w.Time);
                    break;
            }

            return newWeatherQuery;
        }

        private ModelStateDictionary GetErrorsAboutParsingOfWeathers(List<ExcelError> weatherErrors, ModelStateDictionary modelState)
        {
            var model = modelState.Copy();
            weatherErrors.ForEach(error => model.AddModelError($"Ошибка в ячейке {error.TypeCell}",
                                                               $"Лист:{error.Sheet}; " +
                                                               $"Строка:{error.Row}; " +
                                                               $"Столбец:{error.Column};"));
            return model;
        }
    }
}

