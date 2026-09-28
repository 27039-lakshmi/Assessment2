using BoilerControllerApplication.Application.Services;
using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Domain.Enums;
using BoilerControllerApplication.Presentation.Validators;
using System.Drawing;
using System.Net.Http.Headers;
using System.Timers;

namespace BoilerControllerApplication.Presentation.View
{
    public class DashboardView
    {
        private LoggerService _loggerService;
        private BoilerService _boilerService;
        private NotificationService _notificationService ;
        private int notificationDisplayPosition = 0;
        private CountdownService countdownService;

        public DashboardView(LoggerService loggerService, BoilerService boilerService, NotificationService notificationService, CountdownService countdownService)
        {
            this._loggerService = loggerService;
            this._boilerService = boilerService;
            _notificationService = notificationService;
            _notificationService.Notifier += DisplayNotification;
            this.countdownService = countdownService;
            countdownService.countdownTimer.Elapsed += (sender,e) => DisplayCountdown();
            countdownService.CountdownRemover += ClearCountdown;
        }

        public void StartApplication()
        {
            Console.WriteLine("Boiler Control Initialized");
            _loggerService.AddLog(new Log(DateTime.Now,"Boiler Initialized","Started boiler control application"));
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
                    ClearHalfScreen();
                    continue;
                }
                switch (userChoice)
                {
                    case 1:
                        if(_boilerService.CanStartBoiler())
                        {
                            _boilerService.UpdateStatus(BoilerStatus.PrePurge);
                            _boilerService.StartBoiler();
                        }
                        else
                        {
                            Console.WriteLine("Toggle switch to close and reset lockout and try again");
                        }
                        ClearHalfScreen();
                        break;
                    case 2:
                         _boilerService.StopProcess();
                         _boilerService.UpdateStatus(BoilerStatus.Ready);
                        ClearHalfScreen();
                        break;
                    case 3:
                        if (_boilerService.boiler.Status == BoilerStatus.Operational)
                        {
                            _boilerService.SimulateError();
                        }
                        else
                        {
                            Console.WriteLine("Can simulate error only when boiler is operational");
                        }
                        ClearHalfScreen();
                        break;
                    case 4:
                        _boilerService.ToggleSwitch();
                        ClearHalfScreen();
                        break;
                    case 5:
                        bool isResetSucess = _boilerService.ResetLockout();
                        if(!isResetSucess)
                        {
                            Console.WriteLine("Toggle switch to close to reset lockout");
                        }

                        ClearHalfScreen();
                        break;
                    case 6:
                        var logs = _loggerService.GetAllLogs();
                        DisplayLogs(logs);
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

        private void DisplayLogs(IEnumerable<Log> logs)
        {
            string timestampLabel = "Timestamp";
            string eventLabel = "Event";
            string eventDataLabel = "EventData";
            Console.WriteLine($"{timestampLabel,-25}  {eventLabel,-25}  {eventDataLabel,-30}");
            Console.WriteLine(new string('-',90));
            foreach(var log in logs)
            {
                Console.WriteLine($"{log.TimeStamp,-25}  {log.Event,-25}  {log.EventData,-30}");
            }
            ClearScreen();
        }

        private void ClearScreen()
        {
            Console.WriteLine("press any key to continue ..");
            Console.ReadKey();
            Console.Clear();
        }

        private void ClearHalfScreen()
        {
            Console.WriteLine("press any key to continue ..");
            Console.ReadKey();
            Console.SetCursorPosition(0,0);
            for (int i = 0; i < Console.WindowHeight/2; i++)
            {
                Console.WriteLine(new string(' ',Console.WindowWidth/2));
            }
            Console.SetCursorPosition(0, 0);
        }
        private void DisplayNotification(string message, ConsoleColor color)
        {
            int currentCursorLeftPosition = Console.CursorLeft;
            int currentCursorTopPosition = Console.CursorTop;
            Console.ForegroundColor = color;
            Console.SetCursorPosition(Console.WindowWidth / 2, notificationDisplayPosition++);
            Console.Write(message);
            Console.SetCursorPosition(currentCursorLeftPosition,currentCursorTopPosition);
            Console.ResetColor();
        }

        private void DisplayCountdown()
        {
            int currentCursorLeftPosition = Console.CursorLeft;
            int currentCursorTopPosition = Console.CursorTop;
            Console.SetCursorPosition(Console.WindowWidth / 3, 0);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Countdown : {_boilerService.timeleft--,-2}");
            Console.SetCursorPosition(currentCursorLeftPosition, currentCursorTopPosition);
            Console.ResetColor();
        }

        private void ClearCountdown()
        {
            int currentCursorLeftPosition = Console.CursorLeft;
            int currentCursorTopPosition = Console.CursorTop;
            Console.SetCursorPosition(Console.WindowWidth / 3, 0);
            Console.Write(new string(' ',15));
            Console.SetCursorPosition(currentCursorLeftPosition, currentCursorTopPosition);
        }
    }
}
