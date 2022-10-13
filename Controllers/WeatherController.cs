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

namespace WebWeather.Controllers
{
    public class WeatherController : Controller
    {
        private readonly ILogger<WeatherController> _logger;

        private readonly IDbContextFactory<DataWeatherContext> _dataWeatherContextFactory;
        public WeatherController(ILogger<WeatherController> logger, IDbContextFactory<DataWeatherContext> dataWeatherContextFactory)
        {
            _logger = logger;
            _dataWeatherContextFactory = dataWeatherContextFactory;
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
                using var _dataWeatherContext = _dataWeatherContextFactory.CreateDbContext(); // Действие, обращение к бд.
                var weatherService = WeatherService.Create(_dataWeatherContext);// Вычисление
                var isLoad = await weatherService.LoadExcelWithWeatherToDb(excelFiles); //Действие

                if (isLoad)
                {
                    _logger.LogInformation($"Controller{nameof(WeatherController)}. Загрузка файлов в бд успешно завершилась.");//Действие
                    return Ok();
                }
                else
                {
                    var modelStateWithErros = GetErrorsAboutParsingOfWeathers(weatherService, ModelState);// Вычисление
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
                using var dataWeatherContext = _dataWeatherContextFactory.CreateDbContext(); // Действие

                IQueryable<Weather> weatherQuery = GetWeatherQueryableBy(dataWeatherContext); // Вычисление - формирование дерева запроса
                weatherQuery = GetWeathersFilteredByYearAndMonth(weathersFilter, weatherQuery); // Вычисление
                weatherQuery = SortWeathersByOrderType(weathersFilter, weatherQuery);      // Вычисление

                int count = await GetCountOfWeathers(weatherQuery); // Действие
                List<Weather> weathersSlice = await GetSliceOfWeathers(weathersFilter, weatherQuery); // Действие

                weathersFilter = weathersFilter with { count = count }; // Создаём новый объект данных на основе исходного
                WeathersViewModel viewModel = WeathersViewModel.Create(weathersFilter, weathersSlice); // Вычисление по созданию данных
                return View(viewModel);
            }
            catch
            {
                return RedirectToAction("Error");
            }
        } 
        
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private static async Task<List<Weather>> GetSliceOfWeathers(WeathersFilter weathersFilter, IQueryable<Weather> weathers)
        {
            var page = weathersFilter.page;
            var pageSize = weathersFilter.pageSize;           
            return await weathers.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        private static async Task<int> GetCountOfWeathers(IQueryable<Weather> weathers)
        {
            return await weathers.CountAsync();
        }

        private static IQueryable<Weather> GetWeatherQueryableBy(DataWeatherContext dataWeatherContext)
        {
            return dataWeatherContext.Weather.AsQueryable();
        }

        private static IQueryable<Weather> GetWeathersFilteredByYearAndMonth(WeathersFilter weathersFilter, IQueryable<Weather> weatherQuery)
        {
            var year = weathersFilter.year;
            var month = weathersFilter.month;
            if (year != null)
            {
                weatherQuery = weatherQuery.Where(w => w.Date.Year == year);
            }
            if (month != null)
            {
                weatherQuery = weatherQuery.Where(w => w.Date.Month == month);
            }

            return weatherQuery;
        }

        private static IQueryable<Weather> SortWeathersByOrderType(WeathersFilter weathersFilter, IQueryable<Weather> weatherQuery)
        {
            switch (weathersFilter.sortOrder)
            {
                case SortState.DateAsc:
                    weatherQuery = weatherQuery.OrderBy(w => w.Date).ThenBy(w => w.Time);
                    break;
                case SortState.DateDesc:
                    weatherQuery = weatherQuery.OrderByDescending(w => w.Date).ThenByDescending(w => w.Time);
                    break;
            }

            return weatherQuery;
        }

        private ModelStateDictionary GetErrorsAboutParsingOfWeathers(WeatherService weatherService, ModelStateDictionary modelState)
        {
            var model = modelState.Copy();
            foreach (var error in weatherService.ExcelWeatherHandler.WeatherErrors)
            {
                model.AddModelError($"Ошибка в ячейке {error.TypeCell}", $"Лист:{error.Sheet}; Строка:{error.Row}; Столбец:{error.Column};");
            }
            return model;
        }
    }
}

