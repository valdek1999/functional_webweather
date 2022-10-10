using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebWeather.DataAccess.Models;

namespace WebWeather.DataAccess
{
    /// <summary>
    /// Репозитория для работы с таблицами
    /// </summary>
    /// <typeparam name="TEntity">Тип объекта, таблицы</typeparam>
    /// <typeparam name="TypeId">Тип ключа</typeparam>

    /// TODO: возможна доработка функционала. Пока только делегирует обязанности объекту контекста данных бд.
    public class Repository<TEntity, TypeId> where TEntity : Entity<TypeId>
    {
        private readonly DbContext _context;
        public Repository(DbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
