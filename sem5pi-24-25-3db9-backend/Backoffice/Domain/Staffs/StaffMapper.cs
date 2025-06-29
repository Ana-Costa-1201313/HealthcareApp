using Backoffice.Domain.Specializations;

namespace Backoffice.Domain.Staffs
{
    public class StaffMapper
    {
        public StaffDto ToStaffDto(Staff staff)
        {
            List<string> stringAvailabilitySlots = new List<string>();

            if (staff.AvailabilitySlots == null)
            {
                stringAvailabilitySlots = null;
            }
            else
            {
                foreach (AvailabilitySlot availabilitySlot in staff.AvailabilitySlots)
                {
                    stringAvailabilitySlots.Add(availabilitySlot.ToString());
                }
            }


            return new StaffDto
            {
                Id = staff.Id.AsGuid(),
                Name = staff.Name,
                LicenseNumber = staff.LicenseNumber.LicenseNum,
                Email = staff.Email._Email,
                Phone = staff.Phone?.PhoneNum,
                Specialization = staff.Specialization?.Name?.Name,
                AvailabilitySlots = stringAvailabilitySlots,
                Role = staff.Role,
                MechanographicNum = staff.MechanographicNum.ToString(),
                Active = staff.Active
            };
        }

        public Staff ToStaff(CreateStaffDto dto, Specialization specialization, int mecNumSeq, string dns)
        {
            return new Staff(dto, specialization, mecNumSeq, dns);
        }
    }
}