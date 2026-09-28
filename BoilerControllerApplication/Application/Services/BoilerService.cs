using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Domain.Enums;

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
        public BoilerService()
        {
            boiler = new Boiler(BoilerStatus.Lockout, SwitchPosition.Open);
        }

        public void ToggleSwitch()
        {
            if (boiler.Switch == SwitchPosition.Open)
            {
                boiler.Switch = SwitchPosition.Close;
            }
            else
            {
                boiler.Switch = SwitchPosition.Open;
            }
        }

        public bool ResetLockout()
        {
            if(boiler.Switch == SwitchPosition.Close)
            {
                boiler.Status = BoilerStatus.Ready;
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
            if (boiler.Status == BoilerStatus.Operational || boiler.Status == BoilerStatus.Lockout || boiler.Status == BoilerStatus.Ready)
            {
                return;
            }
            if (isStopped)
            {
                boiler.Status = BoilerStatus.Ready;
                isStopped = false;
                return;
            }
            if (isErrorSimulated)
            {
                boiler.Status = BoilerStatus.Lockout;
                return;
            }
            BoilerStatus nextStatus = GetNextStatus(boiler.Status);
            await Task.Delay(timings[boiler.Status]);
            if (boiler.Status == BoilerStatus.Operational || boiler.Status == BoilerStatus.Lockout || boiler.Status == BoilerStatus.Ready)
            {
                return;
            }
            if (isStopped)
            {
                boiler.Status = BoilerStatus.Ready;
                return;
            }
            if (isErrorSimulated)
            {
                boiler.Status = BoilerStatus.Lockout;
                return;
            }
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

        public void UpdateStatus(BoilerStatus status)
        {
            boiler.Status = status;
        }
    }
}
