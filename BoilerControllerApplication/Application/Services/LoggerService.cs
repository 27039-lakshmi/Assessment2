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

        /// <summary>
        /// Calls repo method to add log
        /// </summary>
        /// <param name="log">The log needed to be added</param>
        public void AddLog(Log log)
        {
            loggerRepo.AddLog(log);
            _notificationService.Notify($"{log.Event}: {log.EventData}");
        }

        /// <summary>
        /// Returns all the logs
        /// </summary>
        /// <returns>All logs that need to be traversed</returns>
        public IEnumerable<Log> GetAllLogs()
        {
            return loggerRepo.GetAllLogs();
        }
    }
}
