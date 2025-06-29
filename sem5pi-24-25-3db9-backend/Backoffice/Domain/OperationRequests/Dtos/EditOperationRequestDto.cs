using System.Text.Json.Serialization;

namespace Backoffice.Domain.OperationRequests
{
    public class EditOperationRequestDto
    {
        public string DeadlineDate { get; set; }

        public string Priority { get; set; }
        
        public string Description { get; set; }
    }
}