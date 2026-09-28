using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Domain.Enums;
using System.Diagnostics;

namespace BoilerControllerApplication.Application.Services
{
    public class BoilerService
    {
        public Boiler boiler;
        public bool isStopped = false;
        public bool isErrorSimulated = false;
        public bool isRunning = false;
        private Dictionary<BoilerStatus, int> timings = new () { 
            { BoilerStatus.PrePurge, 10000 },
            { BoilerStatus.Ignition, 10000 } };
        private LoggerService logger;
        public BoilerService(LoggerService loggerService)
        {
            boiler = new Boiler(BoilerStatus.Lockout, SwitchPosition.Open);
            logger = loggerService;
        }

        public void ToggleSwitch()
        {
            if (boiler.Switch == SwitchPosition.Open)
            {
                boiler.Switch = SwitchPosition.Close;
                logger.AddLog(new Log(DateTime.Now, "Switch Toggled", "Interlock Switch toggled to close."));
            }
            else
            {
                boiler.Switch = SwitchPosition.Open;
                logger.AddLog(new Log(DateTime.Now, "Switch Toggled", "Interlock Switch toggled to open."));
            }
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

        public async void StartBoiler()
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
            //if (isStopped)
            //{
            //    boiler.Status = BoilerStatus.Ready;
            //    isStopped = false;
            //    return;
            //}
            //if (isErrorSimulated)
            //{
            //    boiler.Status = BoilerStatus.Lockout;
            //    return;
            //}
            BoilerStatus nextStatus = GetNextStatus(boiler.Status);
            await Task.Delay(timings[boiler.Status]);
            if (boiler.Status == BoilerStatus.Operational )
            {
                logger.AddLog(new Log(DateTime.Now, "Status Changed", $"Boiler now operational"));
                return;
            }
            if(boiler.Status == BoilerStatus.Lockout || boiler.Status == BoilerStatus.Ready)
            {
                return;
            }
            //if (isStopped)
            //{
            //    boiler.Status = BoilerStatus.Ready;
            //    return;
            //}
            //if (isErrorSimulated)
            //{
            //    boiler.Status = BoilerStatus.Lockout;
            //    return;
            //}
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
            if(newStatus == BoilerStatus.Ready) logger.AddLog(new Log(DateTime.Now, "Status Changed", $"Boiler Status changed to Ready"));
            if(newStatus == BoilerStatus.Ignition) logger.AddLog(new Log(DateTime.Now, "Status Changed", $"Pre purge completed"));
            if (newStatus == BoilerStatus.Operational) logger.AddLog(new Log(DateTime.Now, "Status Changed", $"Igniton phase completed"));
            if (newStatus == BoilerStatus.Lockout) logger.AddLog(new Log(DateTime.Now, "Error Occured", $"System in lockout"));
            boiler.Status = newStatus;
        }
    }
}
