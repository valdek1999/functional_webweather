using System;
using System.Collections.Generic;
using System.Linq;
using WebWeather.DataAccess.Models;

namespace WebWeather.Services.Helper
{
    public static class HelperExtensions
    {
        // Глубокое копирование листа. Обобщённое вычесление для защищённого КПЗ
        public static IList<T> CloneItems<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
