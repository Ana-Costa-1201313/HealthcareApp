using Backoffice.Domain.Shared;
using Backoffice.Domain.Specializations;

namespace Backoffice.Domain.Staffs
{
    public class Staff : Entity<StaffId>, IAggregateRoot
    {
        public string Name { get; private set; }

        public LicenseNumber LicenseNumber { get; private set; }

        public Email Email { get; private set; }

        public PhoneNumber Phone { get; private set; }

        public Specialization Specialization { get; private set; }

        public List<AvailabilitySlot> AvailabilitySlots { get; private set; }

        public Role Role { get; private set; }

        public int MecNumSequence { get; private set; }

        public MechanographicNumber MechanographicNum { get; private set; }

        public bool Active { get; private set; }

        private Staff()
        {

        }

        public Staff(CreateStaffDto dto, Specialization specialization, int mecNumSeq, string dns)
        {
            this.Id = new StaffId(Guid.NewGuid());

            if (string.IsNullOrEmpty(dto.Name))
            {
                throw new BusinessRuleValidationException("Error: The staff must have a name!");
            }
            this.Name = dto.Name;

            this.LicenseNumber = new LicenseNumber(dto.LicenseNumber);

            this.Phone = new PhoneNumber(dto.Phone);

            this.Specialization = specialization;

            this.AvailabilitySlots = new List<AvailabilitySlot>();

            if (dto.AvailabilitySlots == null)
            {
                this.AvailabilitySlots = null;
            }
            else
            {
                foreach (string slotString in dto.AvailabilitySlots)
                {
                    this.AvailabilitySlots.Add(AvailabilitySlot.CreateAvailabilitySlot(slotString));
                }
            }

            if (dto.Role != Role.Doctor && dto.Role != Role.Nurse && dto.Role != Role.Technician)
            {
                throw new BusinessRuleValidationException("Error: The staff role must be one of the following: Doctor, Nurse or Tech!");
            }
            this.Role = dto.Role;

            this.MecNumSequence = mecNumSeq;

            this.MechanographicNum = new MechanographicNumber(dto.Role.ToString(), dto.RecruitmentYear, MecNumSequence);

            this.Email = new Email(MechanographicNum + "@" + dns);

            this.Active = true;
        }

        public void Deactivate()
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: This Staff profile is already deactivated!");
            }

            this.Name = "Deactivated Staff";
            this.LicenseNumber = new LicenseNumber(this.Id.GetHashCode());
            this.Phone = null;
            this.Specialization = null;
            this.AvailabilitySlots = null;
            this.Active = false;
        }

        public void Edit(EditStaffDto dto, Specialization specialization)
        {
            if (dto.Phone == null)
            {
                throw new BusinessRuleValidationException("Error: The staff must have a phone number!");
            }
            this.Phone = new PhoneNumber(dto.Phone);

            this.Specialization = specialization;

            List<AvailabilitySlot> list = new List<AvailabilitySlot>();

            if (dto.AvailabilitySlots == null)
            {
                this.AvailabilitySlots = null;
            }
            else
            {
                foreach (string slotString in dto.AvailabilitySlots)
                {
                    list.Add(AvailabilitySlot.CreateAvailabilitySlot(slotString));
                }
                this.AvailabilitySlots = list;
            }
        }

        public void PartialEdit(EditStaffDto dto, Specialization specialization)
        {
            if (dto.Phone != null)
            {
                this.Phone = new PhoneNumber(dto.Phone);
            }

            if (dto.Specialization != null)
            {
                this.Specialization = specialization;
            }

            if (dto.AvailabilitySlots != null)
            {
                List<AvailabilitySlot> list = new List<AvailabilitySlot>();

                foreach (string s in dto.AvailabilitySlots)
                {
                    list.Add(AvailabilitySlot.CreateAvailabilitySlot(s));
                }
                this.AvailabilitySlots = list;
            }
        }
    }
}