using System.Collections.Generic;
using WebWeather.DataAccess.Models;

namespace WebWeather.Models
{
    public class WeathersViewModel
    {
        public IEnumerable<Weather> Weathers { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }

        public static WeathersViewModel Create(WeathersFilter weathersFilter, List<Weather> items)
        {

            return new WeathersViewModel
            {
                PageViewModel = new PageViewModel(weathersFilter),
                SortViewModel = new SortViewModel(weathersFilter),
                FilterViewModel = new FilterViewModel(weathersFilter),
                Weathers = items
            };
        }
    }
}
