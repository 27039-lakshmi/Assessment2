using BoilerControllerApplication.Application.Services;
using BoilerControllerApplication.Domain.Enums;
using BoilerControllerApplication.Presentation.Validators;

namespace BoilerControllerApplication.Presentation.View
{
    public class DashboardView
    {
        private LoggerService _loggerService;
        private BoilerService _boilerService;

        public DashboardView(LoggerService loggerService, BoilerService boilerService)
        {
            this._loggerService = loggerService;
            this._boilerService = boilerService;
        }

        public void StartApplication()
        {
            int userChoice;
            do
            {
                Console.WriteLine("[1] Start boiler sequence\n" +
                                  "[2] Stop boiler sequence\n" +
                                  "[3] Simulate boiler error\n" +
                                  "[4] Toggle Run Interlock Switch\n" +
                                  "[5] Reset Lockout\n" +
                                  "[6] View Event Log\n" +
                                  "[7] Exit Application\n" +
                                  "Enter your choice");
                string userInput = Console.ReadLine() ?? string.Empty;
                if(!Validator.IsValidInteger(userInput, out userChoice))
                {
                    Console.WriteLine("Enter a valid integer");
                    continue;
                }
                switch (userChoice)
                {
                    case 1:
                        if(_boilerService.CanStartBoiler())
                        {
                            _boilerService.UpdateStatus(BoilerStatus.PrePurge);
                            _boilerService.isRunning = true;
                            _boilerService.StartBoiler();
                            _boilerService.isRunning = false;
                        }
                        else
                        {
                            Console.WriteLine("Toggle switch to close and reset lockout and try again");
                        }
                            break;
                    case 2:
                            _boilerService.UpdateStatus(BoilerStatus.Ready);
                        break;
                    case 3:
                        if (_boilerService.boiler.Status == BoilerStatus.Operational)
                        {
                            _boilerService.UpdateStatus(BoilerStatus.Lockout);
                        }
                        else
                        {
                            Console.WriteLine("Can simulate error only when boiler is operational");
                        }

                            break;
                    case 4:
                        _boilerService.ToggleSwitch();
                        break;
                    case 5:
                        bool isResetSucess = _boilerService.ResetLockout();
                        if(!isResetSucess)
                        {
                            Console.WriteLine("Toggle switch to close to reset lockout");
                        }
                        else
                        {
                            Console.WriteLine("Boiler Status changed to Ready");
                        }
                            break;
                    case 6:
                        break;
                    case 7:
                        Console.WriteLine("Exiting application");
                        break;
                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                }
            }
            while (userChoice != 7);
        }
    }
}
