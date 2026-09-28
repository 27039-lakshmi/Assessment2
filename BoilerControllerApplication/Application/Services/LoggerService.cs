using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Infrastructure.Repository;

namespace BoilerControllerApplication.Application.Services
{
    public class LoggerService
    {
        private LoggerRepo loggerRepo;
        private NotificationService _notificationService = new();
        public LoggerService(LoggerRepo loggerRepo, NotificationService notificationService)
        {
            this.loggerRepo = loggerRepo;
            _notificationService = notificationService;
        }

        public void AddLog(Log log)
        {
            loggerRepo.AddLog(log);
            _notificationService.Notify($"{log.Event}: {log.EventData}",ConsoleColor.Green);
        }

        public IEnumerable<Log> GetAllLogs()
        {
            return loggerRepo.GetAllLogs();
        }
    }
}
