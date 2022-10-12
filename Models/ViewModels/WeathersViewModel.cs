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

        public static WeathersViewModel Create(int? year, int? month, int page, SortState sortOrder, int pageSize, int count, List<Weather> items)
        {
            return new WeathersViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(month, year),
                Weathers = items
            };
        }
    }
}
