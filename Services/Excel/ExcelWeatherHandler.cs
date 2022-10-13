using System.Collections.Generic;
using WebWeather.Models.Excel;
using NPOI.SS.UserModel;
using System;
using WebWeather.DataAccess.Models;
using WebWeather.Extensions;
using System.Linq;
using System.Globalization;

namespace WebWeather.Services
{
    /// <summary>
    /// Содержит набор вычислений для работы с эксель файлом для обработки погоды
    /// </summary>
    public static class ExcelWeatherHandler
    {
        /// <summary>
        /// Получить иттератор данных о погоде по листу
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static (List<Weather> Weathers, List<ExcelError> WeatherErrors, bool HasSomeError) GetWeatherDataEnumerator(ISheet sheet)
        {
            var weathers = new List<Weather>();
            var startRow = SearchOfStartingRowBySheet(sheet);
            var errorRow = GetRows(sheet, startRow).FirstOrDefault(row => CheckValid(row) is false);
            var errors = SearchErrorsInRow(errorRow);
            var hasSomeError = errors?.Count > 0;
            if(hasSomeError is not true)
            {
                weathers = GetRows(sheet, startRow).Select(row => GetWeatherData(row)).ToList();
            }
            return (weathers, errors, hasSomeError);
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

                if (CheckValid(row))
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
            if(row is null)
            {
                return excelErrors;
            }
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

        /// <summary>
        /// Итератор возвращающий список строк - вычисление
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        /// <returns></returns>
        public static IEnumerable<IRow> GetRows(ISheet sheet, int startRow)
        {
            for (int i = startRow; i < sheet.LastRowNum; i++)
            {
                yield return sheet.GetRow(i);
            }
        }

        #region Проверка на валидность

        /// <summary>
        /// Проверка на валидность строки
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool CheckValid(IRow row)
        {
            var cellTypes = Enum.GetValues<WeatherCell>();
            foreach (var cellType in cellTypes)
            {
                if (row.Cells.Count <= (int)cellType)
                {
                    return true;
                }
                var cell = row.GetCellFromRowByType(cellType);
                if (cell.CheckValid(cellType) is false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка на валидность ячейки
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellType"></param>
        /// <returns></returns>
        public static bool CheckValid(this ICell cell, WeatherCell cellType)
        {
            switch (cellType)
            {
                case WeatherCell.Date:
                    return DateTime.TryParseExact(cell.ToString().Trim(), "dd.MM.yyyy", new CultureInfo("ru-Ru"), DateTimeStyles.AssumeLocal, out _);
                case WeatherCell.Time:
                    return DateTime.TryParseExact(cell.ToString().Trim(), "H:m", new CultureInfo("ru-Ru"), DateTimeStyles.AssumeLocal, out var _);
                case WeatherCell.AirTemperature:
                case WeatherCell.DewPointTemperature:
                case WeatherCell.AirHumidity:
                    return float.TryParse(cell.ToString(), out _);
                case WeatherCell.AtmosphericPressure:
                    return int.TryParse(cell.ToString(), out _);
                case WeatherCell.WindSpeed:
                case WeatherCell.Cloudiness:
                    if (string.IsNullOrEmpty(cell.ToString().Replace(" ", "")) || cell.CellType == CellType.Blank)
                    {
                        return true;
                    }
                    else
                    {
                        return int.TryParse(cell.ToString(), out _);
                    }
                case WeatherCell.LowerCloudinessLimit:
                    if (string.IsNullOrEmpty(cell.ToString().Replace(" ", "")) || cell.CellType == CellType.Blank)
                    {
                        return true;
                    }
                    else
                    {
                        return float.TryParse(cell.ToString(), out _);
                    }
                case WeatherCell.WindDirection:
                case WeatherCell.WeatherEvent:
                case WeatherCell.HorizontalVisibility:
                    return true;
                default:
                    return true;
            }
        }
        #endregion

    }
}
