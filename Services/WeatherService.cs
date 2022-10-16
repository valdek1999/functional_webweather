using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebWeather.DataAccess;
using WebWeather.DataAccess.Models;
using System.Linq;
using WebWeather.Models.Excel;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace WebWeather.Services
{
    public static class WeatherService
    {
        //Действие т.к зависит от бд
        public static async Task<List<ExcelError>> LoadWeathersExcelToDb(IFormFileCollection files)
        {
            List<ExcelError> excelErrors = GetEmptyExcelErrors();
            using var dataWeatherContext = WeatherContextFactory.CreateDbContext();

            foreach (var excelBook in ExcelTransformer.TransformFilesToExcel(files))
            {
                foreach (var sheet in excelBook)
                {
                    var (Weathers, WeatherErrors, HasSomeError) = ExcelWeatherHandler.GetWeathersData(sheet);
                    if (HasSomeError)
                    {
                        return WeatherErrors;
                    }
                    await SaveWeathers(dataWeatherContext, Weathers);
                }
            }
            return excelErrors;
        }
        public static List<ExcelError> GetEmptyExcelErrors()
        {
            return new List<ExcelError>(0);
        }
        private static async Task SaveWeathers(DataWeatherContext dataWeatherContext, List<Weather> Weathers)
        {
            dataWeatherContext.AddRange(Weathers);
            await dataWeatherContext.SaveChangesAsync();
        }
    }
}
