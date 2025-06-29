using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Xunit;

namespace Backoffice.Tests
{
    public class MechanographicNumberTest
    {
        [Fact]
        public void ValidMecNum()
        {
            string mecNumber = "N20241";

            MechanographicNumber newMecNumber = new MechanographicNumber("Nurse", 2024, 1);

            Assert.NotNull(newMecNumber);
            Assert.Equal(mecNumber, newMecNumber.MechanographicNum);
        }

        [Fact]
        public void NullRole()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new MechanographicNumber(null, 1, 1));

            Assert.Equal("Error: The role is needed!", exception.Message);
        }

        [Fact]
        public void EmptyRole()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new MechanographicNumber("", 1, 1));

            Assert.Equal("Error: The role is needed!", exception.Message);
        }

        [Fact]
        public void InvalidRecruitmentYear()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new MechanographicNumber("Nurse", -1, 1));

            Assert.Equal("Error: The year must be bigger than zero!", exception.Message);
        }

        [Fact]
        public void InvalidMecNum()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new MechanographicNumber("Nurse", 2024, -1));

            Assert.Equal("Error: The mechanographic number must be bigger than zero!", exception.Message);
        }

        [Fact]
        public void NullMecNum()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new MechanographicNumber(null));

            Assert.Equal("Error: The mechanographic number is invalid!", exception.Message);
        }

        [Fact]
        public void EmptyMecNum()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new MechanographicNumber(""));

            Assert.Equal("Error: The mechanographic number is invalid!", exception.Message);
        }
    }
}