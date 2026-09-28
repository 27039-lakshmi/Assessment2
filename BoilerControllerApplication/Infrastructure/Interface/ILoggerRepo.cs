using BoilerControllerApplication.Domain.Entities;

namespace BoilerControllerApplication.Infrastructure.Interface
{
    public interface ILoggerRepo
    {
        public void AddLog(Log log);
        public IEnumerable<Log> GetAllLogs();
    }
}
