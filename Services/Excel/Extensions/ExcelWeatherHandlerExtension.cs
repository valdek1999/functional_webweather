using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using WebWeather.Models.Excel;

namespace WebWeather.Extensions
{
    /// <summary>
    /// Класс расширяющий функционал класса ExcelWeatherHelper
    /// Содержит набор вычислений для работы с эксель файлом для обработки погоды
    /// </summary>
    public static class ExcelWeatherHelperExtension
    {
        #region Парсеры ячеек в нужный тип
        public static (bool IsParsed, DateTime Value) GetDateFromCell(this ICell cell)
        {
            return (DateTime.TryParseExact(cell.ToString().Trim(), "dd.MM.yyyy", new CultureInfo("ru-Ru"), DateTimeStyles.AssumeLocal, out var value), value);
        }
        public static (bool IsParsed, DateTime Value) GetTimeFromCell(this ICell cell)
        {
            var isParsed = DateTime.TryParseExact(cell.ToString().Trim(), "HH:mm", new CultureInfo("ru-Ru"), DateTimeStyles.AssumeLocal, out var value);
            return (isParsed, value);
        }
        public static (bool isParsed, float? Value) GetFloatFromCell(this ICell cell)
        {
            var isParsed = float.TryParse(cell.ToString().Trim(), out float value);
            if (isParsed)
            {
                return (isParsed, value);
            }
            else
            {
                return (isParsed, null);
            }
        }
        public static (bool isParsed, int? Value) GetIntFromCell(this ICell cell)
        {
            var isParsed = int.TryParse(cell.ToString().Trim(), out int value);
            if (isParsed)
            {
                return (isParsed, value);
            }
            else
            {
                return (isParsed, null);
            }
        }
        public static (bool isParsed, string Value) GetStringFromCell(this ICell cell)
        {
            return (true, cell.ToString().Trim());
        }

        #endregion

        /// <summary>
        /// Получение ячейки из строки по типу ячейки
        /// </summary>
        /// <param name="row"></param>
        /// <param name="weatherCellType"></param>
        /// <returns></returns>
        public static ICell GetCellFromRowByType(this IRow row, WeatherCell weatherCellType)
        {
            return row.GetCell((int)weatherCellType);
        }
        
       
    }
}
