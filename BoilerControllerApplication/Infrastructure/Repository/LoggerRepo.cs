using BoilerControllerApplication.Domain.Entities;
using BoilerControllerApplication.Infrastructure.Interface;

namespace BoilerControllerApplication.Infrastructure.Repository
{
    public class LoggerRepo : ILoggerRepo
    {
        private string filepath = "Boiler Log.txt";
        private List<Log> logs = new ();

        public LoggerRepo()
        {
            LoadData();
        }

        /// <summary>
        /// Adds log to the file
        /// </summary>
        /// <param name="log">The log that need to be added</param>
        public void AddLog(Log log)
        {
            logs.Add(log);
            WriteData(log);
        }

        /// <summary>
        /// Returns all the logs
        /// </summary>
        /// <returns>The logs that needs to be traversed</returns>
        public IEnumerable<Log> GetAllLogs()
        {
            return logs;
        }

        /// <summary>
        /// Loads data from csv file and stores in in-memory list
        /// </summary>
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

        /// <summary>
        /// Writes the log from in-memory list to csv file
        /// </summary>
        /// <param name="log">The log that needs to be written</param>
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
