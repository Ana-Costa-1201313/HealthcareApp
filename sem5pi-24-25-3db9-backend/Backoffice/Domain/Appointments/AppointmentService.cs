using System.Text;
using Backoffice.Domain.Logs;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.Patients;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.Appointments;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Azure;
using Backoffice.Domain.OperationTypes;

namespace Backoffice.Domain.Appointments
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _repo;
        private readonly IOperationRequestRepository _opreqrepo;
        private readonly IOperationTypeRepository _optyperepo;
        private readonly IStaffRepository _staffrepo;
        private readonly ISurgeryRoomRepository _roomrepo;
        private readonly IPatientRepository _patientrepo;
        private readonly ILogRepository _logrepo;

        public AppointmentService(IUnitOfWork unitOfWork, IAppointmentRepository repo,
                                        IOperationRequestRepository opreqrepo,
                                        IStaffRepository doctorrepo, ISurgeryRoomRepository roomrepo,
                                        IPatientRepository patientrepo, ILogRepository logrepo, IOperationTypeRepository optyperepo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _opreqrepo = opreqrepo;
            _roomrepo = roomrepo;
            _staffrepo = doctorrepo;
            _patientrepo = patientrepo;
            _logrepo = logrepo;
            _optyperepo = optyperepo;
        }

        public async Task<List<AppointmentDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllWithDetailsAsync();

            List<AppointmentDto> listDto = new List<AppointmentDto>();

            if (list.IsNullOrEmpty()) return listDto;

            foreach (var item in list)
            {
                listDto.Add(AppointmentMapper.ToDto(item));
            }

            return listDto;
            //return null;
        }

        public async Task<List<AppointmentWithDetailsDto>> GetAllWithDetailsAsync()
        {
            var list = await this._repo.GetAllWithDetailsAsync();

            List<AppointmentWithDetailsDto> listDto = new List<AppointmentWithDetailsDto>();
            List<OperationRequest> opreqs = new List<OperationRequest>();

            if (list.IsNullOrEmpty()) return listDto;

            foreach (var item in list)
            {
                opreqs.Add(await _opreqrepo.GetOpRequestByIdWithDetailsAsync(item.OperationRequestId));
            }

            for (int i = 0; i < list.Count; i++)
            {
                listDto.Add(AppointmentMapper.ToDtoWithDetails(list[i], list[i].OperationRequest, opreqs[i].OpType));
            }

            return listDto;
            //return null;
        }

        public async Task<AppointmentDto> GetByIdAsync(Guid id)
        {
            var appointment = await _repo.GetWithDetailsAsync(new AppointmentId(id));

            if (appointment == null) return null;

            return AppointmentMapper.ToDto(appointment);
        }

        public async Task<List<AppointmentDto>> GetAllAsDoctorAsync(string doctorEmail)
        {
            Staff doctor = await _staffrepo.GetStaffByEmailAsync(new Email(doctorEmail));

            var list = await this._repo.GetAllAsDoctorAsync(doctor.Id);

            List<AppointmentDto> listDto = new List<AppointmentDto>();

            //if (list == null) throw new BusinessRuleValidationException("Error: The doctor doesn't have any appointments.");

            foreach (var item in list)
            {
                listDto.Add(AppointmentMapper.ToDto(item));
            }

            return listDto;
        }

        public async Task<AppointmentWithDetailsDto> GetByIdWithDetailsAsync(Guid id)
        {
            var appointment = await _repo.GetWithDetailsAsync(new AppointmentId(id));
            var opreq = await _opreqrepo.GetOpRequestByIdWithDetailsAsync(appointment.OperationRequestId);

            if (appointment == null) return null;

            return AppointmentMapper.ToDtoWithDetails(appointment, opreq, opreq.OpType);
        }

        public async Task<AppointmentWithDetailsDto> CreateAsync(CreateAppointmentDto dto)
        {
            OperationRequest operationRequest = await _opreqrepo.GetOpRequestByIdWithDetailsAsync(new OperationRequestId(dto.OpRequestId));
            if (operationRequest == null) throw new BusinessRuleValidationException("Error: The operation request doesn't exist.");

            var surgeryRoom = await _roomrepo.GetSurgeryRoomByRoomNumberAsync(int.Parse(dto.SurgeryRoomNumber));
            if (surgeryRoom == null) throw new BusinessRuleValidationException("Error: The surgery room doesn't exist.");

            var appointment = AppointmentMapper.ToDomain(dto, operationRequest, surgeryRoom);

            await _repo.AddAsync(appointment);

            Patient patient = await _patientrepo.GetWithDetailsAsync(operationRequest.PatientId);

            patient.AddAppointment(appointment);

            //await this._logrepo.AddAsync(new Log(patient.ToJSON(operationTypeName.Name, patientName, doctorName), LogType.Update, LogEntity.OperationRequest, opReq.Id));

            await _unitOfWork.CommitAsync();

            return AppointmentMapper.ToDtoWithDetails(appointment, operationRequest, operationRequest.OpType);
        }

        public async Task<AppointmentDto> CreateAsync2(CreateAppointmentDto dto)
        {
            OperationRequest operationRequest = await _opreqrepo.GetOpRequestByIdWithDetailsAsync(new OperationRequestId(dto.OpRequestId));
            if (operationRequest == null) throw new BusinessRuleValidationException("Error: The operation request doesn't exist.");

            var surgeryRoom = await _roomrepo.GetSurgeryRoomByRoomNumberAsync(int.Parse(dto.SurgeryRoomNumber));
            if (surgeryRoom == null) throw new BusinessRuleValidationException("Error: The surgery room doesn't exist.");

            var appointment = AppointmentMapper.ToDomain(dto, operationRequest, surgeryRoom);

            await _repo.AddAsync(appointment);

            Patient patient = await _patientrepo.GetWithDetailsAsync(operationRequest.PatientId);

            patient.AddAppointment(appointment);

            //await this._logrepo.AddAsync(new Log(patient.ToJSON(operationTypeName.Name, patientName, doctorName), LogType.Update, LogEntity.OperationRequest, opReq.Id));

            await _unitOfWork.CommitAsync();

            return AppointmentMapper.ToDto(appointment);
        }

        public async Task<List<AppointmentDto>> /* Task<string> */ /*Task<List<CreateAppointmentDto>>*/ CreatePlanningAsync(PlanningDto dtos)
        {
            using (HttpClient client = new HttpClient())
            {
                List<AppointmentDto> appointments = new List<AppointmentDto>();

                string prologString = await CreatePrologStringAsync(dtos);
                
                // Prepare the content of the POST request
                StringContent content = new StringContent(prologString, Encoding.UTF8, "text/plain");

                // Send the POST request to the Prolog server
                HttpResponseMessage response = await client.PostAsync("http://localhost:8080/atualizarBC", content);

                // Ensure the request was successful
                //response.EnsureSuccessStatusCode();

                // Read the response content

                string queryBody = "?day=" + dtos.date.ToString("yyyyMMdd") + "&room=a" + dtos.selectedRoomNumber;
                //string queryBody = "?day=20241028&room=or1";
                //HttpResponseMessage response2 = new HttpResponseMessage();

                switch (dtos.planType)
                {
                    case "best":
                        //HttpResponseMessage
                        response = await client.GetAsync("http://localhost:8080/brute_force" + queryBody);
                        break;
                    case "heuristic1":
                        response = await client.GetAsync("http://localhost:8080/obtain_heuristic_solution" + queryBody);
                        break;
                    case "heuristic2":
                        response = await client.GetAsync("http://localhost:8080/obtain_heuristic_occupied_solution" + queryBody);
                        break;
                }

                //response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                PlanningAppointmentDto planningDto = new PlanningAppointmentDto();

                string responseContent = RemoveHeaderLine(responseBody);
                
                try
                {
                    planningDto = JsonConvert.DeserializeObject<PlanningAppointmentDto>(responseContent);
                }
                catch (Exception)
                {
                    throw new Exception("Error deserializing.");
                }

                List<CreateAppointmentDto> appointmentsDto = ConvertPrologAgendaToAppointments(planningDto, dtos);

                foreach (CreateAppointmentDto createAppointmentDto in appointmentsDto)
                {
                    AppointmentDto appointment = await CreateAsync2(createAppointmentDto);
                    appointments.Add(appointment);
                }

                //List<OperationRequest> opreqs = new List<OperationRequest>();

                foreach (CreateAppointmentDto appDto in appointmentsDto)
                {
                    OperationRequest opreq = await _opreqrepo.GetOpRequestByIdWithDetailsAsync(new OperationRequestId(appDto.OpRequestId));

                    foreach (string staffEmail in opreq.SelectedStaff)
                    {
                        Staff staffTemp = await _staffrepo.GetStaffByEmailAsync(new Email(staffEmail));
                        Staff staff = await _staffrepo.GetByIdWithDetailsAsync(staffTemp.Id);
                        List<AvailabilitySlot> slots = staff.AvailabilitySlots;

                        if (DateTime.TryParse(appDto.DateTime, out DateTime appointmentDateTime))
                        {
                            // Find and remove availability slots that match the criteria

                            slots.RemoveAll(slot =>
                                appointmentDateTime >= slot.StartTime && appointmentDateTime <= slot.EndTime);

                            staff.PartialEdit(new EditStaffDto { AvailabilitySlots = slots.Select(slot => slot.ToString()).ToList() },
                                staff.Specialization);
                        }
                        else
                        {
                            // Handle invalid DateTime format in appDto.DateTime
                            throw new ArgumentException($"Invalid DateTime format in appDto.DateTime: {appDto.DateTime}");
                        }
                    }
                    
                    SurgeryRoom surgeryRoom = await _roomrepo.GetSurgeryRoomByRoomNumberAsync(int.Parse(appDto.SurgeryRoomNumber));

                    if (surgeryRoom != null)
                    {
                        OperationType operationType = await _optyperepo.GetByIdWithDetailsSuperAsync(opreq.OpType.Id);

                        TimeSpan totalOperationDuration = operationType.Duration.AnesthesiaPatientPreparationInMinutes +
                            operationType.Duration.SurgeryInMinutes + operationType.Duration.CleaningInMinutes;

                        DateTime appointmentDateTime = DateTime.Parse(appDto.DateTime);

                        double totalMinutes = totalOperationDuration.TotalMinutes;
                        
                        DateTime maintenanceEnd = appointmentDateTime.AddMinutes(totalMinutes);

                        TimeSlot maintenanceSlot = new TimeSlot(appDto.DateTime, maintenanceEnd.ToString());

                        surgeryRoom.AddMaintenanceSlot(maintenanceSlot);
                    }

                    else
                    {
                        // Handle invalid SurgeryRoomNumber in appDto.SurgeryRoomNumber
                        throw new ArgumentException($"Invalid SurgeryRoomNumber in appDto.SurgeryRoomNumber: {appDto.SurgeryRoomNumber}");
                    }

                    await _unitOfWork.CommitAsync(); 
                } 

                //return responseBody;
                //return prologString;
                //return responseContent;
                return appointments;
                //return appointmentsDto;
            }
        }

        //return "Failed to create planning";

        public async Task<AppointmentWithDetailsDto> UpdateAsync(Guid id, UpdateAppointmentDto dto)
        {
            if (id == Guid.Empty)
            {
                throw new BusinessRuleValidationException("Error: Invalid AppointmentId value provided!");
            }

            var appointment = await _repo.GetWithDetailsAsync(new AppointmentId(id));

            if (appointment == null)
            {
                throw new BusinessRuleValidationException("Appointment not found");
            }

            int roomNumber;
            SurgeryRoom surgeryRoom = null;


            if (dto.SurgeryRoomNumber != null)
            {
                try
                {
                    roomNumber = int.Parse(dto.SurgeryRoomNumber);
                }
                catch (Exception)
                {
                    throw new BusinessRuleValidationException("Error: Invalid Surgery Room Number value provided!");
                }
                surgeryRoom = await _roomrepo.GetSurgeryRoomByRoomNumberAsync(roomNumber);

                if (surgeryRoom == null)
                {
                    throw new BusinessRuleValidationException("Error: No Surgery Room found with the given number!");
                }
            }

            AppointmentStatus status = AppointmentStatus.Null;

            if (dto.AppointmentStatus != null)
            {
                try
                {
                    status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), dto.AppointmentStatus, true);
                }
                catch (ArgumentException)
                {
                    throw new BusinessRuleValidationException("Error: Invalid Status value provided!");
                }
            }

            DateTime dateTime = default;

            if (dto.DateTime != null)
            {
                try
                {
                    dateTime = DateTime.Parse(dto.DateTime);
                }
                catch (Exception)
                {
                    throw new BusinessRuleValidationException("Error: Invalid DateTime value provided!");
                }
                if (dateTime < DateTime.Now)
                {
                    throw new BusinessRuleValidationException("Error: DateTime can't be in the past!");
                }
            }

            List<Staff> selectedStaff = new List<Staff>();
            List<string> selectedStaffIds = new List<string>();

            foreach (string staffEmail in dto.StaffList)
            {
                Staff staff = await this._staffrepo.GetStaffByEmailAsync(new Email(staffEmail));
                selectedStaff.Add(staff);
            }

            foreach (Staff staff in selectedStaff)
            {
                selectedStaffIds.Add(staff.Id.AsString());
            }

            appointment.Update(dateTime, surgeryRoom, status);

            OperationRequest operationRequest = await _opreqrepo.GetOpRequestByIdWithDetailsAsync(appointment.OperationRequestId);

            appointment.OperationRequest.UpdateOperationRequest(appointment.DateTime, appointment.OperationRequest.Priority, appointment.OperationRequest.Description, dto.StaffList);

            await _unitOfWork.CommitAsync();

            return AppointmentMapper.ToDtoWithDetails(appointment, appointment.OperationRequest, operationRequest.OpType);
        }
        public async Task<List<AppointmentWithDetailsDto>> GetByDateAsync(string date)
        {
            List<AppointmentWithDetailsDto> appointmentsDtos = new List<AppointmentWithDetailsDto>();

            if (!DateTime.TryParse(date, out var parsedDate))
            {
                throw new BusinessRuleValidationException("Invalid date format");
            }

            var appointments = await _repo.GetByDateWithDetailsAsync(parsedDate);

            if (appointments == null || !appointments.Any())
            {
                return null;
            }

            List<OperationRequest> opreqs = new List<OperationRequest>();

            if (appointments.IsNullOrEmpty()) return appointmentsDtos;

            foreach (var item in appointments)
            {
                opreqs.Add(await _opreqrepo.GetOpRequestByIdWithDetailsAsync(item.OperationRequestId));
            }

            for (int i = 0; i < appointments.Count; i++)
            {
                appointmentsDtos.Add(AppointmentMapper.ToDtoWithDetails(appointments[i], appointments[i].OperationRequest, opreqs[i].OpType));
            }

            return appointmentsDtos;
        }

        public async Task<string> CreatePrologStringAsync(PlanningDto dtos)
        {
            StringBuilder prologString = new StringBuilder();
            StringBuilder staffBuilder = new StringBuilder();                   //X
            StringBuilder agenda_staffBuilder = new StringBuilder();            //X
            StringBuilder agenda_operationroomBuilder = new StringBuilder();    //X
            StringBuilder timetableBuilder = new StringBuilder();               //X
            StringBuilder surgeryBuilder = new StringBuilder();                 //X
            StringBuilder surgery_idBuilder = new StringBuilder();              //X
            StringBuilder assignment_surgeryBuilder = new StringBuilder();      //X

            List<Staff> staffList = new List<Staff>();
            //List<string> specializations = new List<string>();
            List<OperationRequest> opReqList = new List<OperationRequest>();
            List<OperationRequestId> opReqIdList = new List<OperationRequestId>();

            bool hasAvaiabilitySlotInThatDay = false;

            int roomNumber = int.Parse(dtos.selectedRoomNumber);
            SurgeryRoom surgeryRoom = await _roomrepo.GetSurgeryRoomByRoomNumberAsync(roomNumber);

            List<string> staffs = new List<string>();

            string date = dtos.date.ToString("yyyyMMdd");

            agenda_operationroomBuilder.Append("agenda_operation_room(a")
                .Append(dtos.selectedRoomNumber + ",")
                .Append(date + ",[");

            if (!surgeryRoom.MaintenanceSlots.IsNullOrEmpty())
            {
                foreach (TimeSlot slot in surgeryRoom.MaintenanceSlots)
                {
                    int j = 1;

                    if (slot.StartTime.DayOfYear == dtos.date.DayOfYear)
                    {
                        agenda_operationroomBuilder.Append(ConvertTimeSlotToPrologString(slot) + ",maint" + j + "),");
                    }
                }

                //agenda_operationroomBuilder.Remove(agenda_operationroomBuilder.Length - 1, 1);
            }

            agenda_operationroomBuilder.Append("]).\n");

            foreach (OperationRequestDto opReqDto in dtos.selectedOpRequests)
            {
                opReqIdList.Add(new OperationRequestId(opReqDto.Id));

                foreach (string staffEmail in opReqDto.SelectedStaff)
                {
                    if (!staffs.Contains(staffEmail))
                    {
                        staffs.Add(staffEmail);
                    }
                }
            }

            foreach (string staffEmail in staffs)
            {
                Staff staffTemp = await _staffrepo.GetStaffByEmailAsync(new Email(staffEmail));
                staffList.Add(await _staffrepo.GetByIdWithDetailsAsync(staffTemp.Id));

                /*if (!specializations.Contains(staffList.Last().Specialization.Name.Name))
                {
                    specializations.Add(staffList.Last().Specialization.Name.Name);
                }*/
            }

            opReqList = await _opreqrepo.GetListByIdWithDetailsAsync(opReqIdList);

            foreach (Staff staff in staffList)
            {
                int i = 1;
                string specializationProlog = "orthopaedist";

                staffBuilder.Append("staff('")
                    .Append(ConvertEmail(staff.Email.ToString()) + "','")
                    .Append(staff.Role.ToString().ToLower() + "','");

                if (staff.Specialization.Name.Name.Contains("Anesthesia"))
                {
                    specializationProlog = "anesthesia";
                }
                else if (staff.Specialization.Name.Name.Contains("Cleaning"))
                {
                    specializationProlog = "cleaning";
                }

                staffBuilder.Append(specializationProlog + "',['type']).\n");

                agenda_staffBuilder.Append("agenda_staff('")
                    .Append(ConvertEmail(staff.Email.ToString()) + "',")
                    .Append(date + ",[");

                foreach (AvailabilitySlot slot in staff.AvailabilitySlots)
                {
                    List<AvailabilitySlot> invertedSlots = new List<AvailabilitySlot>();

                    if (slot.StartTime.DayOfYear == dtos.date.DayOfYear)
                    {
                        hasAvaiabilitySlotInThatDay = true;

                        timetableBuilder.Append("timetable('")
                            .Append(ConvertEmail(staff.Email.ToString()) + "',")
                            .Append(date + ",")
                            .Append(ConvertTimeSlotToPrologString(slot) + ")).\n");

                        invertedSlots = InvertTimeSlot(slot);

                        List<string> timeSlots = new List<string>();

                        foreach (AvailabilitySlot timeSlot in invertedSlots)
                        {
                            timeSlots.Add(ConvertTimeSlotToPrologString(timeSlot));
                        }

                        agenda_staffBuilder.Append(timeSlots.ElementAt(0))
                            .Append(",'m0" + i + "'),")
                            .Append(timeSlots.ElementAt(1) + ",'c0" + i + "')");
                        i++;
                    }
                }

                if (!hasAvaiabilitySlotInThatDay)
                {
                    timetableBuilder.Append("timetable('")
                        .Append(ConvertEmail(staff.Email.ToString()) + "',")
                        .Append(date + ",")
                        .Append("(0,1440)).\n");
                }

                agenda_staffBuilder.Append("]).\n");
            }

            foreach (OperationRequest opReq in opReqList)
            {
                surgeryBuilder.Append("surgery(").Append("'type',")
                    .Append(ConvertDateTimeToPrologString(opReq.OpType.Duration.AnesthesiaPatientPreparationInMinutes) + ",")
                    .Append(ConvertDateTimeToPrologString(opReq.OpType.Duration.SurgeryInMinutes) + ",")
                    .Append(ConvertDateTimeToPrologString(opReq.OpType.Duration.CleaningInMinutes) + ").\n");

                surgery_idBuilder.Append("surgery_id('a")
                    .Append(opReq.Id.AsString() + "',")
                    .Append("'type').\n");

                foreach (string staffEmail in opReq.SelectedStaff)
                {
                    assignment_surgeryBuilder.Append("assignment_surgery('a")
                        .Append(opReq.Id.AsString() + "','")
                        .Append(ConvertEmail(staffEmail) + "').\n");
                }
            }

            prologString.Append(staffBuilder.ToString()).Append(agenda_staffBuilder.ToString()).Append(agenda_operationroomBuilder.ToString()).Append(timetableBuilder.ToString()).Append(surgeryBuilder.ToString()).Append(surgery_idBuilder.ToString()).Append(assignment_surgeryBuilder.ToString());

            return prologString.ToString();
        }

        //public async Task<AppointmentDto> UpdateAsync(UpdateAppointmentDto dto)
        public List<AvailabilitySlot> InvertTimeSlot(AvailabilitySlot timeSlot)
        {
            var startOfDay = new AvailabilitySlot(timeSlot.StartTime.Date.ToString(), timeSlot.StartTime.ToString());
            var endOfDay = new AvailabilitySlot(timeSlot.EndTime.ToString(), timeSlot.EndTime.Date.AddHours(23).AddMinutes(59).ToString());

            return new List<AvailabilitySlot> { startOfDay, endOfDay };
        }

        public string ConvertTimeSlotToPrologString(AvailabilitySlot timeSlot)
        {
            StringBuilder prologString = new StringBuilder();
            int startTime = timeSlot.StartTime.Hour * 60 + timeSlot.StartTime.Minute;
            int endTime = timeSlot.EndTime.Hour * 60 + timeSlot.EndTime.Minute;
            prologString.Append("(").Append(startTime).Append(",").Append(endTime);
            return prologString.ToString();
        }

        public string ConvertTimeSlotToPrologString(TimeSlot timeSlot)
        {
            StringBuilder prologString = new StringBuilder();
            int startTime = timeSlot.StartTime.Hour * 60 + timeSlot.StartTime.Minute;
            int endTime = timeSlot.EndTime.Hour * 60 + timeSlot.EndTime.Minute;
            prologString.Append("(").Append(startTime).Append(",").Append(endTime);
            return prologString.ToString();
        }

        public string ConvertDateTimeToPrologString(TimeSpan time)
        {
            int totalMinutes = (int)time.TotalMinutes;

            return totalMinutes.ToString();

        }

        public List<CreateAppointmentDto> ConvertPrologAgendaToAppointments(PlanningAppointmentDto agenda, PlanningDto planningDto)
        {
            List<CreateAppointmentDto> appointments = new List<CreateAppointmentDto>();

            foreach (string appointment in agenda.AgendaRoom)
            {
                var parts = appointment.Split(",");
                int totalMinutes = int.Parse(parts[0]);
                int minutes = totalMinutes % 60;
                int hours = totalMinutes / 60;
                string guidString = parts[2].Substring(2);
                guidString = guidString.Substring(0, guidString.Length - 1);

                DateTime appointmentDate = planningDto.date.Date.AddHours(hours).AddMinutes(minutes);

                CreateAppointmentDto dto = new CreateAppointmentDto
                {
                    DateTime = appointmentDate.ToString(),
                    OpRequestId = guidString,
                    SurgeryRoomNumber = planningDto.selectedRoomNumber
                };

                appointments.Add(dto);
            }

            return appointments;
        }

        static string ConvertEmail(string input)
        {
            // Split the string at '@' to isolate the part before the domain
            string beforeAt = input.Split('@')[0];

            // Convert the string to lowercase
            return beforeAt.ToLower();
        }

        static string RemoveHeaderLine(string input)
        {
            return input.Replace("Content-type: application/json; charset=UTF-8", string.Empty);
        }
    }
}