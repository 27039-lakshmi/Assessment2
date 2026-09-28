

namespace BoilerControllerApplication.Application.Services
{
    public class CountdownService
    {
        public System.Timers.Timer countdownTimer;
        public event Action CountdownRemover;
        public CountdownService()
        {
            countdownTimer = new System.Timers.Timer();
        }
        public void StopCountdown()
        {
            countdownTimer.Stop();
            CountdownRemover?.Invoke();
        }

        public void SetInterval(int interval)
        {
            countdownTimer.Interval = interval;
        }

        public void StartTimer()
        {
            countdownTimer.Start();
        }
    }
}
