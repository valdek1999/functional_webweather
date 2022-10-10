using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using WebWeather.DataAccess.Models;

namespace WebWeather.Models
{
    public class FilterViewModel
    {
        public FilterViewModel(int? mouth, int? year)
        {
            Years = new SelectList(new[]{2008,2009,2010,2011,2012,2013,2014,2015,2016,2017,2018,2019,2020,2021,2022,2023});
            Mouth = new SelectList(new[] { 1,2,3,4,5,6,7,8,9,10,11,12 });
            SelectedMouth = mouth;
            SelectedYear = year;
        }
        public SelectList Years { get; private set; } // список годов
        public SelectList Mouth { get; private set; } // список месяцев
        public int? SelectedMouth { get; private set; }   // выбранный месяц
        public int? SelectedYear { get; private set; }    // выбранный год
    }
}
