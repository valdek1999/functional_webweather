namespace WebWeather.Models.Excel
{
    public class ExcelError
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
