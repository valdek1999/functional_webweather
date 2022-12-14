namespace WebWeather.Models.Excel
{
    /// <summary>
    /// Модель ошибок. Группа: данные
    /// </summary>
    public record ExcelError
    {
        public TypeExcelFile Type { get; init; }
        public WeatherCell TypeCell { get; init; }
        public string Message { get; init; }
        public string Sheet { get; init; }
        public int Row { get; init; }
        public int Column { get; init; }
    }

    public enum TypeExcelFile
    {
        Unknown,
        Weather,
    }
}
