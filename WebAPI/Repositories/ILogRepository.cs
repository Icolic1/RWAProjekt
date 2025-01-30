using WebAPI.Models;

namespace WebAPI.Repositories
{
    public interface ILogRepository
    {
        void AddLog(Log log);
        IEnumerable<Log> GetLastLogs(int count);
        int GetLogCount();
    }
}
