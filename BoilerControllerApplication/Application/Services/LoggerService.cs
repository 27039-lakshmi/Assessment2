using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Infrastructure.Repository;

namespace BoilerControllerApplication.Application.Services
{
    public class LoggerService
    {
        private LoggerRepo loggerRepo;

        public LoggerService(LoggerRepo loggerRepo)
        {
            this.loggerRepo = loggerRepo;
        }

        public void AddLog(Log log)
        {
            loggerRepo.AddLog(log);
        }

        public IEnumerable<Log> GetAllLogs()
        {
            return loggerRepo.GetAllLogs();
        }
    }
}
