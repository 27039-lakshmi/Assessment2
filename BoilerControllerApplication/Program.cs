using BoilerControllerApplication.Application.Services;
using BoilerControllerApplication.Infrastructure.Repository;
using BoilerControllerApplication.Presentation.View;

namespace BoilerControllerApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var loggerRepo = new LoggerRepo();
                var loggerService = new LoggerService(loggerRepo);
                var boilerService = new BoilerService(loggerService);
                var dashboard = new DashboardView(loggerService, boilerService);
                dashboard.StartApplication();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
