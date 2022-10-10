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
    }
}
