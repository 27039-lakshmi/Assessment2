using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Infrastructure.Interface;

namespace BoilerControllerApplication.Infrastructure.Repository
{
    public class LoggerRepo : ILoggerRepo
    {
        string filepath = "Boiler Log.txt";
        List<Log> logs = new ();
        public LoggerRepo()
        {
            LoadData();
        }

        public void AddLog(Log log)
        {
            logs.Add(log);
            WriteData(log);
        }

        public IEnumerable<Log> GetAllLogs()
        {
            return logs;
        }

        private void LoadData()
        {
            if (!File.Exists(filepath))
            {
                logs = new List<Log>();
                return;
            }
            string[] lines = File.ReadAllLines(filepath);
            foreach (string line in lines.Skip(1))
            {
                if (string.IsNullOrEmpty(line)) continue;
                string[] values = line.Split(',');
                logs.Add(new Log(DateTime.Parse(values[0]), values[1], values[2]));
            }
        }

        private void WriteData(Log log)
        {
            if (!File.Exists(filepath))
            {
                File.WriteAllText(filepath, "Timespan,Event,EventData" + Environment.NewLine );
            }
            File.AppendAllLines(filepath, new string[] { $"{log.TimeStamp},{log.Event},{log.EventData}"});
            
        }
    }
}
