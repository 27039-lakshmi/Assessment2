using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Domain.Enums;

namespace BoilerControllerApplication.Application.Services
{
    public class BoilerService
    {
        public Boiler boiler;
        private Dictionary<BoilerStatus, int> _timings = new () { 
            { BoilerStatus.PrePurge, 10000 },
            { BoilerStatus.Ignition, 10000 } };
        public bool isForceClosed;
        private readonly LoggerService _logger;
        private readonly NotificationService _notificationService = new();
        private System.Timers.Timer _timer;
        private readonly CountdownService _countdownService;
        

        public BoilerService(LoggerService loggerService, CountdownService countdownService)
        {
            boiler = new Boiler(BoilerStatus.Lockout, SwitchPosition.Open);
            _logger = loggerService;
            _timer = new System.Timers.Timer();
            _countdownService = countdownService;
            _timer.Elapsed += (sender, e) => CompletePhase();
        }

        /// <summary>
        /// Toggles switch to open or close.
        /// When toggled while boiler is running then simulates error
        /// </summary>
        public void ToggleSwitch()
        {
            if(boiler.Status != BoilerStatus.Lockout) 
            {
                _logger.AddLog(new Log(DateTime.Now, "Error", "Switch toggled to open while boiler is running"));
            }
            if (boiler.Switch == SwitchPosition.Open)
            {
                boiler.Switch = SwitchPosition.Close;
                _logger.AddLog(new Log(DateTime.Now, "Switch Toggled", "Interlock Switch toggled to close."));
            }
            else
            {
                boiler.Switch = SwitchPosition.Open;
                if(boiler.Status != BoilerStatus.Lockout && boiler.Status != BoilerStatus.Ready)
                {
                    isForceClosed = true;
                    SimulateError();
                }
                _logger.AddLog(new Log(DateTime.Now, "Switch Toggled", "Interlock Switch toggled to open."));
            }
        }

        /// <summary>
        /// Stops boiler running , updates the status to lockout
        /// </summary>
        public void SimulateError()
        {
            StopProcess();
            UpdateStatus(BoilerStatus.Lockout);
        }
        /// <summary>
        /// Updates boiler status to ready when switch is in close position
        /// </summary>
        /// <returns></returns>
        public bool ResetLockout()
        {
            if(boiler.Switch == SwitchPosition.Close)
            {
                UpdateStatus(BoilerStatus.Ready);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if boiler can be started
        /// </summary>
        /// <returns>True if status is ready and switch is closed,
        /// otherwise false</returns>
        public bool CanStartBoiler()
        {
            return boiler.Status == BoilerStatus.Ready && boiler.Switch == SwitchPosition.Close;
        }

        /// <summary>
        /// Starts the boiler running
        /// Adds interval to timer based on the status and 
        /// the elapsed event calls the method to move to next status
        /// Starts countdown timer when a phase is running
        /// </summary>
        public void StartBoiler()
        {
            if (boiler.Status == BoilerStatus.Operational)
            {
                _logger.AddLog(new Log(DateTime.Now, "Status Changed", "Boiler now operational"));
                return;
            }
            if (boiler.Status == BoilerStatus.Lockout || boiler.Status == BoilerStatus.Ready)
            {
                return;
            }            
            _timer.AutoReset = false;
            _timer.Interval = _timings[boiler.Status];
            
            _countdownService.SetInterval(1000);           
            _countdownService.timeleft = _timings[boiler.Status]/1000;
            _timer.Start();
            _countdownService.StartTimer();
        }

        /// <summary>
        /// Stops timer and countdown
        /// </summary>
        public void StopProcess()
        {
            _timer.Stop();
            _countdownService.StopCountdown();
        }

        /// <summary>
        /// stops countdown, updates the status and start process again
        /// </summary>
        private void CompletePhase()
        {
            _countdownService.StopCountdown();
            BoilerStatus nextStatus = GetNextStatus(boiler.Status);
            UpdateStatus(nextStatus);            
            StartBoiler();
        }

        /// <summary>
        /// Finds the next status of the boiler
        /// </summary>
        /// <param name="status">The current status of boiler</param>
        /// <returns>The next status based on switch result</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when unknown status is being received</exception>
        public BoilerStatus GetNextStatus(BoilerStatus status)
        {
            return status switch
            {
                BoilerStatus.Ready => BoilerStatus.PrePurge,
                BoilerStatus.PrePurge => BoilerStatus.Ignition,
                BoilerStatus.Ignition => BoilerStatus.Operational,
                _ => throw new InvalidOperationException("no next status available")
            };
        }

        /// <summary>
        /// Updates the status of boiler with new status
        /// </summary>
        /// <param name="newStatus">The new status of the boiler</param>
        public void UpdateStatus(BoilerStatus newStatus)
        {            
            if(newStatus == BoilerStatus.Ready) { 
                _logger.AddLog(new Log(DateTime.Now, "Status Changed", "Boiler Status changed to Ready"));
            }
            if (newStatus == BoilerStatus.PrePurge)
            {
                _logger.AddLog(new Log(DateTime.Now, "Status Changed", "Boiler Status changed to Pre purge"));
            }
            if (newStatus == BoilerStatus.Ignition) {
                _logger.AddLog(new Log(DateTime.Now, "Status Changed", "Pre purge completed"));
            }
            if (newStatus == BoilerStatus.Operational) {
                _logger.AddLog(new Log(DateTime.Now, "Status Changed", "Igniton phase completed"));
            }
            if (newStatus == BoilerStatus.Lockout) {
                _logger.AddLog(new Log(DateTime.Now, "Error Occured", "System in lockout"));
            }
            boiler.Status = newStatus;
        }
    }
}
