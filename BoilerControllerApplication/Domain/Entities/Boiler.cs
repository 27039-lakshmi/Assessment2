using BoilerControllerApplication.Domain.Enums;

namespace BoilerControllerApplication.Domain.Entities
{
    public class Boiler
    {
        public Boiler(BoilerStatus status, SwitchPosition switchPosition)
        {
            Status = status;
            Switch = switchPosition;
        }

        public BoilerStatus Status { get; set; }
        public SwitchPosition Switch { get; set; }
    }
}
