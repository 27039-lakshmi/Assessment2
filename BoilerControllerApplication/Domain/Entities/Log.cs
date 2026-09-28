namespace BoilerControllerApplication.Domain.Entities
{
    public class Log
    {
        public Log(DateTime timeStamp, string @event, string eventData)
        {
            TimeStamp = timeStamp;
            Event = @event;
            EventData = eventData;
        }

        public DateTime TimeStamp { get; set; }
        public string Event { get; set; }
        public string EventData { get; set; }
    }
}
