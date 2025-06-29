using Backoffice.Domain.Shared;

namespace Backoffice.Domain.OperationRequests
{
    public class OperationRequestId : EntityId
    {
        public OperationRequestId(Guid value) : base(value)
        {
        }

        public OperationRequestId(string value) : base(value)
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