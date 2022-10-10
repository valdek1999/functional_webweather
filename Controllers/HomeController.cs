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

namespace WebWeather.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DataWeatherContext _dataWeatherContext;

        private readonly WeatherService _weatherService;
        public HomeController(ILogger<HomeController> logger, DataWeatherContext dataWeatherContext, WeatherService weatherService)
        {
            _logger = logger;
            _dataWeatherContext = dataWeatherContext;
            _weatherService = weatherService;
        }

        public IActionResult ExcelLoader()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFileCollection uploads)
        {
            try
            {
                var isLoad = await _weatherService.LoadExcelWithWeatherToDb(uploads);
                if (isLoad)
                {
                    _logger.LogInformation($"Controller{nameof(HomeController)}. Загрузка файлов в бд успешно завершилась.");
                    return Ok();
                }
                else
                {
                    foreach(var error in _weatherService.ExcelWeatherHandler.WeatherErrors)
                    {
                        ModelState.AddModelError($"Ошибка в ячейке {error.TypeCell}", $"Лист:{error.Sheet}; Строка:{error.Row}; Столбец:{error.Column};");
                    }
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Controller{nameof(HomeController)}. Error: {ex.Message}.");
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> Weathers(int? year, int? month, int page = 1,
            SortState sortOrder = SortState.DateAsc)
        {
            try
            {
                int pageSize = 10;

                //фильтрация
                IQueryable<Weather> weather = _dataWeatherContext.Weather;

                if (year != null)
                {
                    weather = weather.Where(w => w.Date.Year == year);
                }
                if (month != null)
                {
                    weather = weather.Where(w => w.Date.Month == month);
                }

                //сортировка
                switch (sortOrder)
                {
                    case SortState.DateAsc:
                        weather = weather.OrderBy(w => w.Date).ThenBy(w => w.Time);
                        break;
                    case SortState.DateDesc:
                        weather = weather.OrderByDescending(w => w.Date).ThenByDescending(w => w.Time);
                        break;
                }

                // пагинация
                var count = await weather.CountAsync();
                var items = await weather.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                // формируем модель представления
                WeathersViewModel viewModel = new WeathersViewModel
                {
                    PageViewModel = new PageViewModel(count, page, pageSize),
                    SortViewModel = new SortViewModel(sortOrder),
                    FilterViewModel = new FilterViewModel(month, year),
                    Weathers = items
                };
                return View(viewModel);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
