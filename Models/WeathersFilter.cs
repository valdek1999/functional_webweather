namespace WebWeather.Models
{
    /// <summary>
    /// Неизменяймый тип. Хранит данные для фильтрации списка погод
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="page"></param>
    /// <param name="sortOrder"></param>
    /// <param name="count"></param>
    /// <param name="pageSize"></param>
    public record WeathersFilter(int? year, int? month, int page = 1, SortState sortOrder = SortState.DateAsc, int count = 0, int pageSize = 10);
}
