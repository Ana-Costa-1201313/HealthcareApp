using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Appointments
{
    public class AppointmentId : EntityId
    {
        public AppointmentId(Guid value) : base(value)
        {
        }

        public AppointmentId(string value) : base(value)
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