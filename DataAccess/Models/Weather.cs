using System;

namespace WebWeather.DataAccess.Models
{
    /// <summary>
    /// Табличный объект сущности "Погода"
    /// </summary>
    public class Weather:Entity<int>
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Время(московское)
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Температура воздуха
        /// </summary>
        public float AirTemperature { get; set; }

        /// <summary>
        /// Относительная влажность воздуха
        /// </summary>
        public int AirHumidity { get; set; }

        /// <summary>
        /// Температура точки росы
        /// </summary>
        public float DewPointTemperature { get; set; }

        /// <summary>
        /// Атм. давление, мм рт.ст.
        /// </summary>
        public int AtmosphericPressure { get; set; }

        /// <summary>
        /// Направление ветра
        /// </summary>
        public string WindDirection { get; set; }

        /// <summary>
        /// Скорость ветра в м/с
        /// </summary>
        public int? WindSpeed { get; set; }

        /// <summary>
        /// Облачность в %
        /// </summary>
        public int? Cloudiness { get; set; }

        /// <summary>
        /// Нижний предел облачности
        /// </summary>
        public float? LowerCloudinessLimit { get; set; }

        /// <summary>
        /// Горизонтальная видимость
        /// </summary>
        public string HorizontalVisibility { get; set; }

        /// <summary>
        /// Погодные явления
        /// </summary>
        public string WeatherEvent { get; set; }
    }
}
