public class PlanningOperationRequestDto
    {
        //list of selected staff
        public List<string> staff { get; set; }
        //list of occupied timeslots for a given day for each staff
        public List<string> staff_schedule { get; set; }
        //
        public List<string> timetable { get; set; }
        //list of scheduled operations for the room for a given day
        public string room_schedule { get; set; }
        //operation type
        public List<string> surgery { get; set; }
        //operation id and type
        public List<string> surgery_id { get; set; }
        //staff - operation
        public List<string> assignment_surgery { get; set; }
    }