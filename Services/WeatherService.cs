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
            List<ExcelError> excelErrors = new List<ExcelError>();
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
                    dataWeatherContext.AddRange(Weathers);
                    await dataWeatherContext.SaveChangesAsync();
                }
            }
            return excelErrors;
        }
    }
}
