using Microsoft.EntityFrameworkCore;
using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Staffs
{
    [Owned]
    public class AvailabilitySlot : IValueObject
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        private AvailabilitySlot()
        {

        }

        public AvailabilitySlot(string startTime, string endTime)
        {

            if (!DateTime.TryParse(startTime, out DateTime parsedStartTime))
            {
                throw new BusinessRuleValidationException("Error: Invalid start time format.");
            }

            if (!DateTime.TryParse(endTime, out DateTime parsedEndTime))
            {
                throw new BusinessRuleValidationException("Error: Invalid end time format.");
            }

            if (parsedEndTime <= parsedStartTime)
            {
                throw new BusinessRuleValidationException("Error: The end time must be after the start time!");
            }

            this.StartTime = parsedStartTime;
            this.EndTime = parsedEndTime;
        }

        public override string ToString()
        {
            //return StartTime.ToString("g") + " - " + EndTime.ToString("g");
            return StartTime.ToString("o") + "/" + EndTime.ToString("o");
        }

        public static AvailabilitySlot CreateAvailabilitySlot(string stringAvSlot)
        {
            string[] times = stringAvSlot.Split('/');
            if (times.Length == 2)
            {
                string startTime = times[0];
                string endTime = times[1];

                return new AvailabilitySlot(startTime, endTime);
            }
            else
            {
                throw new BusinessRuleValidationException("Error: Invalid Availability slot format!");
            }
        }
    }
}