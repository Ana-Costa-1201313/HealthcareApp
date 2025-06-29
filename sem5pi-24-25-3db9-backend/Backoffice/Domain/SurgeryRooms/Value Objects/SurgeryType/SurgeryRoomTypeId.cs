using Backoffice.Domain.Shared;
using System;
using Newtonsoft.Json;

namespace Backoffice.Domain.SurgeryRooms.ValueObjects
{
    public class SurgeryRoomTypeId : EntityId
    {
        public SurgeryRoomTypeId(Guid value) : base(value)
        {
        }

        public SurgeryRoomTypeId(string value) : base(value)
        {
        }

        override
        protected Object createFromString(String text)
        {
            return new Guid(text);
        }

        override
        public String AsString()
        {
            Guid obj = (Guid)base.ObjValue;
            return obj.ToString();
        }

        public Guid AsGuid()
        {
            return (Guid)base.ObjValue;
        }
    }
}