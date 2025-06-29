using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Xunit;

namespace Backoffice.Tests
{
    public class AvailabilitySlotTest
    {
        [Fact]
        public void ValidAvailabilitySlot()
        {
            string validStartTime = "2024-10-10T12:00:00";
            string validEndTime = "2024-10-10T15:00:00";

            var availabilitySlot = new AvailabilitySlot(validStartTime, validEndTime);

            Assert.NotNull(availabilitySlot);
            Assert.Equal(DateTime.Parse(validStartTime), availabilitySlot.StartTime);
            Assert.Equal(DateTime.Parse(validEndTime), availabilitySlot.EndTime);
        }

        [Fact]
        public void InvalidStartTime()
        {
            string invalidStartTime = "111";
            string validEndTime = "2024-10-10T15:00:00";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(invalidStartTime, validEndTime));

            Assert.Equal("Error: Invalid start time format.", exception.Message);
        }

        [Fact]
        public void InvalidEndTime()
        {
            string validStartTime = "2024-10-10T12:00:00";
            string invalidEndTime = "111";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(validStartTime, invalidEndTime));

            Assert.Equal("Error: Invalid end time format.", exception.Message);
        }

        [Fact]
        public void EndTimeBeforeStartTime()
        {
            string startTime = "2024-10-10T15:00:00";
            string endTime = "2024-10-10T12:00:00";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(startTime, endTime));

            Assert.Equal("Error: The end time must be after the start time!", exception.Message);
        }
    }
}
