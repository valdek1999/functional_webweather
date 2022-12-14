using System;

namespace WebWeather.Models
{
    public record PageViewModel
    {
        public int PageNumber { get; private init; }
        public int TotalPages { get; private init; }

        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public PageViewModel(WeathersFilter weathersFilter)
        {
            var page = weathersFilter.page;
            var pageSize = weathersFilter.pageSize;
            var count = weathersFilter.count;

            PageNumber = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}
