using System;
using Backoffice.Domain.Shared;


namespace Backoffice.Domain.SurgeryRooms.ValueObjects
{
    public class SurgeryRoomType : Entity<SurgeryRoomTypeId>, IAggregateRoot
    {
        public string Type { get; private set; }
        public string Description { get; set; }

        public bool fitForSurgery { get; set; }

        public SurgeryRoomTypeId surgeryRoomTypeId;

        // Constructor for EF Core
        private SurgeryRoomType() { }

        public SurgeryRoomType(string type, bool? fitForSurgery, string description)
        {
            if (type == null || fitForSurgery == null) throw new ArgumentNullException("Type/fitForSurgery cannot be null");

            this.Type = type;

            this.fitForSurgery = fitForSurgery.Value;

            this.Id = new SurgeryRoomTypeId(Guid.NewGuid());

            if (description != null) this.Description = description;
        }

        
        public override string ToString(){
            return this.Type;
        }
    }
}