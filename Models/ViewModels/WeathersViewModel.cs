using System.Collections.Generic;
using WebWeather.DataAccess.Models;
using WebWeather.Services.Helper;

namespace WebWeather.Models
{
    public record WeathersViewModel
    {
        public IEnumerable<Weather> Weathers { get; init; }
        public PageViewModel PageViewModel { get; init; }
        public FilterViewModel FilterViewModel { get; init; }
        public SortViewModel SortViewModel { get; init; }

        public static WeathersViewModel CreateWeathersViewModel(WeathersFilter weathersFilter)
        {
            return new WeathersViewModel
            {
                PageViewModel = new PageViewModel(weathersFilter),
                SortViewModel = new SortViewModel(weathersFilter),
                FilterViewModel = new FilterViewModel(weathersFilter),
                Weathers = weathersFilter.weathersSlice.CloneItems()
            };
        }
    }
}
