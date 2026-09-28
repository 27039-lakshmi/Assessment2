namespace BoilerControllerApplication.Application.Services
{
    public class NotificationService
    {
        public event Action<string> Notifier;

        /// <summary>
        /// Invokes Notifier event to display notification
        /// </summary>
        /// <param name="message">The message to be notified</param>
        public void Notify(string message)
        {
            Notifier?.Invoke(message);
        }
    }
}
