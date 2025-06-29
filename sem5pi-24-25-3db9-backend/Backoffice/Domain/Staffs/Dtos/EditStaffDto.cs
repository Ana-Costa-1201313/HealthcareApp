namespace Backoffice.Domain.Staffs
{
    public class EditStaffDto
    {
        public Guid Id { get; set; }

        public string Phone { get; set; }

        public string Specialization { get; set; }

        public List<string> AvailabilitySlots { get; set; }
    }
}