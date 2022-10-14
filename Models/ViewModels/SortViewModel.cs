namespace WebWeather.Models
{
    public enum SortState
    {
        DateAsc,    // по имени по возрастанию
        DateDesc,   // по имени по убыванию
        MouthAsc, // по возрастанию
        MouthDesc,    // по убыванию
    }
    public record SortViewModel
    {
        public SortState DateSort { get; private init; } // значение для сортировки по дате
        public SortState Current { get; private init; }  // текущее значение сортировки
        public SortViewModel(SortState sortOrder)
        {
            DateSort = sortOrder == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            Current = sortOrder;
        }

        public SortViewModel(WeathersFilter weathersFilter) :this(weathersFilter.sortOrder)
        {
        }
    }
}
