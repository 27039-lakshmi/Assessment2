using BoilerControllerApplication.Application.Services;
using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Domain.Enums;
using BoilerControllerApplication.Presentation.Validators;

namespace BoilerControllerApplication.Presentation.View
{
    public class DashboardView
    {
        private readonly LoggerService _loggerService;
        private readonly BoilerService _boilerService;
        private readonly NotificationService _notificationService ;
        private int _notificationDisplayPosition = 0;
        private readonly CountdownService _countdownService;

        public DashboardView(LoggerService loggerService,
            BoilerService boilerService,
            NotificationService notificationService,
            CountdownService countdownService)
        {
            _loggerService = loggerService;
            _boilerService = boilerService;
            _notificationService = notificationService;
            _notificationService.Notifier += DisplayNotification;
            this._countdownService = countdownService;
            countdownService.countdownTimer.Elapsed += (sender,e) => DisplayCountdown();
            countdownService.CountdownRemover += ClearCountdown;
        }

        /// <summary>
        /// Displays dashboard, controls the flow
        /// </summary>
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
                switch ((MenuOption)userChoice)
                {
                    case MenuOption.StartBoiler:
                        if (_boilerService.boiler.Switch == SwitchPosition.Open)
                        {
                            Console.WriteLine("Close the switch and try again");
                            ClearHalfScreen();
                            continue;
                        }
                        if (_boilerService.boiler.Status == BoilerStatus.Lockout && _boilerService.isForceClosed)
                        {
                            _boilerService.UpdateStatus(BoilerStatus.Ready);
                            _boilerService.isForceClosed = false;
                        }
                        if (_boilerService.boiler.Status == BoilerStatus.Lockout)
                        {
                            Console.WriteLine("Reset lockout and try again");
                            ClearHalfScreen();
                            continue;
                        }
                        _boilerService.UpdateStatus(BoilerStatus.PrePurge);
                        _boilerService.StartBoiler();
                        ClearHalfScreen();
                        break;

                    case MenuOption.StopBoiler:
                         _boilerService.StopProcess();
                         _boilerService.UpdateStatus(BoilerStatus.Ready);
                        ClearHalfScreen();
                        break;

                    case MenuOption.SimulateError:
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

                    case MenuOption.ToggleSwitch:
                        _boilerService.ToggleSwitch();
                        ClearHalfScreen();
                        break;

                    case MenuOption.Reset:
                        bool isResetSucess = _boilerService.ResetLockout();
                        if(!isResetSucess)
                        {
                            Console.WriteLine("Toggle switch to close to reset lockout");
                        }

                        ClearHalfScreen();
                        break;

                    case MenuOption.ViewLog:
                        var logs = _loggerService.GetAllLogs();
                        DisplayLogs(logs);
                        break;

                    case MenuOption.Exit:
                        Console.WriteLine("Exiting application");
                        break;

                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                }
            }
            while ((MenuOption)userChoice != MenuOption.Exit);
        }

        /// <summary>
        /// Displays the list of logs in neat format
        /// </summary>
        /// <param name="logs">The list of logs that needs to be traversed 
        /// and displayed</param>
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

        /// <summary>
        /// Clears the screen after user enters a key
        /// </summary>
        private void ClearScreen()
        {
            Console.WriteLine("press any key to continue ..");
            Console.ReadKey();
            Console.Clear();
        }

        /// <summary>
        /// Clears only the menu options so that notification is not cleared
        /// </summary>
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

        /// <summary>
        /// Displays notification
        /// </summary>
        /// <param name="message">The message to be notified</param>
        /// <param name="color">The color in which </param>
        private void DisplayNotification(string message)
        {
            int currentCursorLeftPosition = Console.CursorLeft;
            int currentCursorTopPosition = Console.CursorTop;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(Console.WindowWidth / 2, _notificationDisplayPosition++);
            Console.Write(message);
            Console.SetCursorPosition(currentCursorLeftPosition,currentCursorTopPosition);
            Console.ResetColor();
        }

        /// <summary>
        /// Displays countdown in between menu and notification panels
        /// </summary>
        private void DisplayCountdown()
        {
            int currentCursorLeftPosition = Console.CursorLeft;
            int currentCursorTopPosition = Console.CursorTop;
            Console.SetCursorPosition(Console.WindowWidth / 3, 0);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Countdown : {_countdownService.timeleft--,-2}");
            Console.SetCursorPosition(currentCursorLeftPosition, currentCursorTopPosition);
            Console.ResetColor();
        }

        /// <summary>
        /// Clears the countdown by printing whitespaces
        /// </summary>
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
