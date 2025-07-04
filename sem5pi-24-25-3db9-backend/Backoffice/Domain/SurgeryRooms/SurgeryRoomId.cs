using Backoffice.Domain.Shared;

namespace Backoffice.Domain.SurgeryRooms
{
    public class SurgeryRoomId : EntityId
    {
        public SurgeryRoomId(Guid value) : base(value)
        {
        }

        public SurgeryRoomId(string value) : base(value)
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