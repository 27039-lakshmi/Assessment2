namespace BoilerControllerApplication.Application.Services
{
    public class NotificationService
    {
        public event Action<string,ConsoleColor> Notifier;

        public void Notify(string message, ConsoleColor color)
        {
            Notifier?.Invoke(message, color);
        }
    }
}
