using NPOI.SS.UserModel;
using System;
using System.Globalization;
using WebWeather.Models.Excel;

namespace WebWeather.Extensions
{

    public static class ExcelWeatherHelper
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

        #region Проверка на валидность

        /// <summary>
        /// Проверка на валидность строки
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool CheckValid(this IRow row)
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
