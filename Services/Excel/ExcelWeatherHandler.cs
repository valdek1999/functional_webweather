using System.Collections.Generic;
using WebWeather.Models.Excel;
using NPOI.SS.UserModel;
using System;
using WebWeather.DataAccess.Models;
using WebWeather.Extensions;

namespace WebWeather.Services
{
    public class ExcelWeatherHandler
    {
        public List<ExcelError> WeatherErrors { get; private set; } = new List<ExcelError>();
        public bool HasSomeError { get; private set; } = false;

        /// <summary>
        /// Получить иттератор данных о погоде по листу
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public IEnumerable<Weather> GetWeatherDataEnumerator(ISheet sheet)
        {
            var startRow = SearchOfStartingRowBySheet(sheet);
            for (int i = startRow; i < sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row.CheckValid() is false)
                {
                    var erros = SearchErrorsInRow(row);
                    WeatherErrors.AddRange(erros);
                    HasSomeError = true;
                    break;
                }
                var weather = GetWeatherData(row);
                yield return weather;
            }
        }
        /// <summary>
        /// Получить модель данных о погоде по строке листа
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Weather GetWeatherData(IRow row)
        {
            var date = row.GetCellFromRowByType(WeatherCell.Date).GetDateFromCell().Value;
            var time = row.GetCellFromRowByType(WeatherCell.Time).GetTimeFromCell().Value;
            return new Weather
            {
                Date = date,
                Time = time,
                AirTemperature = row.GetCellFromRowByType(WeatherCell.AirTemperature).GetFloatFromCell().Value.GetValueOrDefault(),
                AirHumidity = row.GetCellFromRowByType(WeatherCell.AirHumidity).GetIntFromCell().Value.GetValueOrDefault(),
                DewPointTemperature = row.GetCellFromRowByType(WeatherCell.DewPointTemperature).GetFloatFromCell().Value.GetValueOrDefault(),
                AtmosphericPressure = row.GetCellFromRowByType(WeatherCell.AtmosphericPressure).GetIntFromCell().Value.GetValueOrDefault(),
                WindDirection = row.GetCellFromRowByType(WeatherCell.WindDirection)?.GetStringFromCell().Value,
                WindSpeed = row.GetCellFromRowByType(WeatherCell.WindSpeed)?.GetIntFromCell().Value,
                Cloudiness = row.GetCellFromRowByType(WeatherCell.Cloudiness)?.GetIntFromCell().Value,
                LowerCloudinessLimit = row.GetCellFromRowByType(WeatherCell.LowerCloudinessLimit)?.GetFloatFromCell().Value,
                HorizontalVisibility = row.GetCellFromRowByType(WeatherCell.HorizontalVisibility)?.GetStringFromCell().Value,
                WeatherEvent = row.GetCellFromRowByType(WeatherCell.WeatherEvent)?.GetStringFromCell().Value,
            };
        }

        /// <summary>
        /// Поиск строки от которой надо начать парсинг данных
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="sheetIsValid"></param>
        /// <returns></returns>
        private static int SearchOfStartingRowBySheet(ISheet sheet)
        {
            for (int i = 0; i < sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row.CheckValid())
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Поиск всех ошибок в строке
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static List<ExcelError> SearchErrorsInRow(IRow row)
        {
            var cellTypes = Enum.GetValues<WeatherCell>();
            var excelErrors = new List<ExcelError>();
            foreach (var cellType in cellTypes)
            {
                if (row.Cells.Count <= (int)cellType)
                {
                    break;
                }
                var cell = row.GetCellFromRowByType(cellType);
                if (cell.CheckValid(cellType) is false)
                {
                    ExcelError excelError = new()
                    {
                        Message = $"Ошибка в ячейке типа {cellType}. Лист {cell.Sheet.SheetName}. Строка: {cell.RowIndex+1}, столбец: {cell.ColumnIndex+1}",
                        Sheet = cell.Sheet.SheetName,
                        Column = cell.ColumnIndex+1,
                        Row = cell.RowIndex+1,
                        Type = TypeExcelFile.Weather,
                        TypeCell = cellType
                    };
                    excelErrors.Add(excelError);
                }
            }
            return excelErrors;
        }
    }
}
