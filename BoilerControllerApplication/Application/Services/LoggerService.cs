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
    }
}
