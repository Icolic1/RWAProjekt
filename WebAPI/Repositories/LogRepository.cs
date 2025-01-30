using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly CulturalHeritageDbContext _context;

        public LogRepository(CulturalHeritageDbContext context)
        {
            _context = context;
        }

        public void AddLog(Log log)
        {
            _context.Add(log);
            _context.SaveChanges();
        }

        public IEnumerable<Log> GetLastLogs(int count)
        {
            return _context.Set<Log>()
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToList();
        }

        public int GetLogCount()
        {
            return _context.Set<Log>().Count();
        }
    }
}
