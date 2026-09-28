using BoilerControllerApplication.Application.Services;
using BoilerControllerApplication.Infrastructure.Repository;
using BoilerControllerApplication.Presentation.View;

namespace BoilerControllerApplication
{
    public class Program
    {
        /// <summary>
        /// Entry point of application
        /// </summary>
        /// <param name="args">array of strings as argument</param>
        public static void Main(string[] args)
        {
            try
            {
                var loggerRepo = new LoggerRepo();
                var notificationService = new NotificationService();
                var loggerService = new LoggerService(loggerRepo, notificationService);
                var countdownService = new CountdownService();
                var boilerService = new BoilerService(loggerService, countdownService);
                var dashboard = new DashboardView(loggerService, boilerService, notificationService, countdownService);
                dashboard.StartApplication();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
