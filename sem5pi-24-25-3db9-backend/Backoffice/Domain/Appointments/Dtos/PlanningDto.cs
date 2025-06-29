using Backoffice.Domain.OperationRequests;

public class PlanningDto
    {
        public string planType { get; set; }
        public string selectedRoomNumber { get; set; }
        public DateTime date { get; set; }
        public List<OperationRequestDto> selectedOpRequests { get; set; }
    }