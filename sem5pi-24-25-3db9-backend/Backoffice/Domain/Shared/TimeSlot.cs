using Backoffice.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.Shared
{
    [Owned]
    public class TimeSlot : IValueObject
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        private TimeSlot(){}

        public TimeSlot(string startTime, string endTime)
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
            return StartTime.ToString("g") + " - " + EndTime.ToString("g");
        }

        public static TimeSlot CreateTimeSlot(string stringTimeSlot)
        {
            string[] times = stringTimeSlot.Split('/');
            if (times.Length == 2)
            {
                string startTime = times[0];
                string endTime = times[1];

                return new TimeSlot(startTime, endTime);
            }
            else
            {
                throw new BusinessRuleValidationException("Error: Invalid Time slot format!");
            }
        }
    }
}