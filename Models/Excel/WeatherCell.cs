namespace WebWeather.Models.Excel
{
    /// <summary>
    /// Типы ячеек в Excel таблице.
    /// </summary>
    public enum WeatherCell
    {
        Date,
        Time,
        AirTemperature,
        AirHumidity,
        DewPointTemperature,
        AtmosphericPressure,
        WindDirection,
        WindSpeed,
        Cloudiness,
        LowerCloudinessLimit,
        HorizontalVisibility,
        WeatherEvent
    }
}
