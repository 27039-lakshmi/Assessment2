namespace BoilerControllerApplication.Application.Services
{
    public class CountdownService
    {
        public System.Timers.Timer countdownTimer;
        public event Action CountdownRemover;
        public int timeleft;
        public CountdownService()
        {
            countdownTimer = new System.Timers.Timer();
        }

        /// <summary>
        /// Stops the countdown and invokes event to remove printed countdown
        /// </summary>
        public void StopCountdown()
        {
            countdownTimer.Stop();
            CountdownRemover?.Invoke();
        }

        /// <summary>
        /// sets the interval for timer
        /// </summary>
        /// <param name="interval">the interval for which timer need to run</param>
        public void SetInterval(int interval)
        {
            countdownTimer.Interval = interval;
        }

        /// <summary>
        /// Starts the countdown timer
        /// </summary>
        public void StartTimer()
        {
            countdownTimer.Start();
        }
    }
}
