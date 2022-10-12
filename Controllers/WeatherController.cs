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
                // добавили фабричный метод(действие, т.к идёт обращение к переменным окружения) по созданию контекста к бд
                using var _dataWeatherContext = _dataWeatherContextFactory.CreateDbContext(); 

                var weatherService = WeatherService.Create(_dataWeatherContext);// добавил фабричный метод(вычисление) для созданию сущности WeatherService 

                //Действие т.к просходит загрузка эксель файлов в бд
                var isLoad = await weatherService.LoadExcelWithWeatherToDb(excelFiles);
                if (isLoad)
                {
                    //действие т.к происход вывод лога с информацией
                    _logger.LogInformation($"Controller{nameof(WeatherController)}. Загрузка файлов в бд успешно завершилась.");
                    
                    return Ok();
                }
                else
                {
                    #region Вычисление
                    var modelStateWithErros = GetErrorsAboutParsingOfWeathers(weatherService, ModelState);
                    return BadRequest(modelStateWithErros);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                //действие лог зависит от внешнего вызов(файла с ошибкой)
                _logger.LogError($"Controller{nameof(WeatherController)}. Error: {ex.Message}.");
                return StatusCode(500);
            }
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

        public async Task<IActionResult> Weathers(int? year, int? month, int page = 1,
            SortState sortOrder = SortState.DateAsc)
        {
            try
            {
                using var dataWeatherContext = _dataWeatherContextFactory.CreateDbContext();

                // Формирование дерево запроса - строится дерево запросов, но не идёт запрос в бд - вычисление.
                // Требуется вынести в отдельную функцию
                #region Вычисление
                int pageSize = 10;
                IQueryable<Weather> weathers = GetWeatherQueryableBy(dataWeatherContext);
                weathers = GetWeathersFilteredByYearAndMonth(year, month, weathers);

                //сортировка
                weathers = SortWeathersByOrderType(sortOrder, weathers);
                #endregion
                //Идёт запрос в бд для получения кол-во записией в бд с погодой и для формирования списка записей "погод"
                #region Действие
                // пагинация                
                int count = await GetCountOfWeathers(weathers); // на этом этапе формируется запрос в бд - является действие
                List<Weather> weathersSlice = await GetSliceOfWeathers(page, pageSize, weathers); // на этом этапе формируется запрос в бд - является действие
                #endregion
                //Модель для хранения списка погод. Будем выделять вычисления, чтобы получить данные.
                #region Преобразуем в данные
                WeathersViewModel viewModel = WeathersViewModel.Create(year, month, page, sortOrder, pageSize, count, weathersSlice);
                #endregion
                return View(viewModel);
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }

        private static async Task<List<Weather>> GetSliceOfWeathers(int page, int pageSize, IQueryable<Weather> weathers)
        {
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

        private static IQueryable<Weather> GetWeathersFilteredByYearAndMonth(int? year, int? month, IQueryable<Weather> weathers)
        {
            if (year != null)
            {
                weathers = weathers.Where(w => w.Date.Year == year);
            }
            if (month != null)
            {
                weathers = weathers.Where(w => w.Date.Month == month);
            }

            return weathers;
        }

        private static IQueryable<Weather> SortWeathersByOrderType(SortState sortOrder, IQueryable<Weather> weather)
        {
            switch (sortOrder)
            {
                case SortState.DateAsc:
                    weather = weather.OrderBy(w => w.Date).ThenBy(w => w.Time);
                    break;
                case SortState.DateDesc:
                    weather = weather.OrderByDescending(w => w.Date).ThenByDescending(w => w.Time);
                    break;
            }

            return weather;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
