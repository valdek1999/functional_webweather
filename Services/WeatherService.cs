using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebWeather.DataAccess;
using WebWeather.DataAccess.Models;
namespace WebWeather.Services
{
    public class WeatherService
    {
        private readonly Repository<Weather, int> _dataWeatherRepository;
        public ExcelWeatherHandler ExcelWeatherHandler { get; }
        public WeatherService(DataWeatherContext dataWeatherContext)
        {
            _dataWeatherRepository = new Repository<Weather, int>(dataWeatherContext);
            ExcelWeatherHandler = new ExcelWeatherHandler();
        }

        public async Task<bool> LoadExcelWithWeatherToDb(IFormFileCollection files)
        {
            foreach (var excelBook in ExcelTransformer.TransformFilesToExcel(files))
            {
                foreach (var sheet in excelBook)
                {
                    foreach (var weatherData in ExcelWeatherHandler.GetWeatherDataEnumerator(sheet))
                    {
                        await _dataWeatherRepository.AddAsync(weatherData);
                    }
                    if (ExcelWeatherHandler.HasSomeError)
                    {
                        return false;
                    }
                    else
                    {
                        await _dataWeatherRepository.SaveAsync();
                    }
                }
            }
            return true;
        }
    }
}
