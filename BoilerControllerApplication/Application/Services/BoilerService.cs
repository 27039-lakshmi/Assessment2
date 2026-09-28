using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Domain.Enums;
using System.Diagnostics;

namespace BoilerControllerApplication.Application.Services
{
    public class BoilerService
    {
        public Boiler boiler;
        private Dictionary<BoilerStatus, int> timings = new () { 
            { BoilerStatus.PrePurge, 10000 },
            { BoilerStatus.Ignition, 10000 } };
        private LoggerService logger;
        private NotificationService _notificationService = new();
        private System.Timers.Timer _timer;
        private CountdownService _countdownService;

        public int timeleft;
        public BoilerService(LoggerService loggerService, CountdownService countdownService)
        {
            boiler = new Boiler(BoilerStatus.Lockout, SwitchPosition.Open);
            logger = loggerService;
            _timer = new System.Timers.Timer();
            _countdownService = countdownService;
        }

        public void ToggleSwitch()
        {
            if(boiler.Status != BoilerStatus.Lockout) 
            {
                logger.AddLog(new Log(DateTime.Now, "Error", "Switch toggled to open while boiler is running"));
            }
            if (boiler.Switch == SwitchPosition.Open)
            {
                boiler.Switch = SwitchPosition.Close;
                logger.AddLog(new Log(DateTime.Now, "Switch Toggled", "Interlock Switch toggled to close."));

            }
            else
            {
                boiler.Switch = SwitchPosition.Open;
                if(boiler.Status != BoilerStatus.Lockout && boiler.Status != BoilerStatus.Ready)
                {
                    SimulateError();
                }
                logger.AddLog(new Log(DateTime.Now, "Switch Toggled", "Interlock Switch toggled to open."));
            }
        }

        public void SimulateError()
        {
            StopProcess();
            UpdateStatus(BoilerStatus.Lockout);
        }
        public bool ResetLockout()
        {
            if(boiler.Switch == SwitchPosition.Close)
            {
                UpdateStatus(BoilerStatus.Ready);
                return true;
            }
            return false;
        }

        public bool CanStartBoiler()
        {
            return boiler.Status == BoilerStatus.Ready && boiler.Switch == SwitchPosition.Close;
        }

        public void StartBoiler()
        {
            if (boiler.Status == BoilerStatus.Operational)
            {
                logger.AddLog(new Log(DateTime.Now, "Status Changed", $"Boiler now operational"));
                return;
            }
            if (boiler.Status == BoilerStatus.Lockout || boiler.Status == BoilerStatus.Ready)
            {
                return;
            }            
            _timer.AutoReset = false;
            _timer.Interval = timings[boiler.Status];
            _timer.Elapsed += (sender,e) => CompletePhase();
            _countdownService.SetInterval(1000);           
            timeleft = timings[boiler.Status]/1000;
            _timer.Start();
            _countdownService.StartTimer();
        }

        public void StopProcess()
        {
            _timer.Stop();
            _countdownService.StopCountdown();
        }

        private void CompletePhase()
        {
            _countdownService.StopCountdown();
            BoilerStatus nextStatus = GetNextStatus(boiler.Status);
            UpdateStatus(nextStatus);            
            StartBoiler();
        }

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

        public void UpdateStatus(BoilerStatus newStatus)
        {            
            if(newStatus == BoilerStatus.Ready) { 
                logger.AddLog(new Log(DateTime.Now, "Status Changed", "Boiler Status changed to Ready"));
            }
            if (newStatus == BoilerStatus.PrePurge)
            {
                logger.AddLog(new Log(DateTime.Now, "Status Changed", "Boiler Status changed to Pre purge"));
            }
            if (newStatus == BoilerStatus.Ignition) {
                logger.AddLog(new Log(DateTime.Now, "Status Changed", "Pre purge completed"));
            }
            if (newStatus == BoilerStatus.Operational) {
                logger.AddLog(new Log(DateTime.Now, "Status Changed", "Igniton phase completed"));
            }
            if (newStatus == BoilerStatus.Lockout) {
                logger.AddLog(new Log(DateTime.Now, "Error Occured", "System in lockout"));
            }
            boiler.Status = newStatus;
        }
    }
}
