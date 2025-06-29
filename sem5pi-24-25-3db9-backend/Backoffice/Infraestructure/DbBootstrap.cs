using System;
using System.Linq;
using Backoffice.Infraestructure;
using Backoffice.Domain.Specializations;
using Microsoft.Extensions.DependencyInjection;
using Backoffice.Domain.Staffs;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Users;
using Backoffice.Domain.OperationTypes;
using Backoffice.Domain.Patients;
using Backoffice.Domain.OperationRequests;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Backoffice.Domain.Appointments;

namespace Backoffice.Infraestructure
{
    public class DbBootstrap
    {
        private readonly BDContext _context;

        public DbBootstrap(BDContext context)
        {
            _context = context;
        }

        public void UserBootstrap()
        {
            if (_context.Users.Any()) return;

            List<User> users = new List<User>();

            User user1 = new User(Role.Admin, "admin@hotmail.com");
            user1.ActivateUser("AAAAAAAAAAA1!");

            User user2 = new User(Role.Nurse, "N20241@healthcareapp.com");
            user2.ActivateUser("AAAAAAAAAAA1!");

            User user3 = new User(Role.Doctor, "D20232@healthcareapp.com");
            user3.ActivateUser("AAAAAAAAAAA1!");

            User user4 = new User(Role.Doctor, "D20243@healthcareapp.com");
            user4.ActivateUser("AAAAAAAAAAA1!");

            User user5 = new User(Role.Nurse, "N20224@healthcareapp.com");
            user5.ActivateUser("AAAAAAAAAAA1!");

            User user6 = new User(Role.Technician, "T20225@healthcareapp.com");
            user6.ActivateUser("AAAAAAAAAAA1!");

            User user7 = new User(Role.Nurse, "N20236@healthcareapp.com");
            user7.ActivateUser("AAAAAAAAAAA1!");

            User user8 = new User(Role.Patient, "savinho@gmail.com");
            user8.ActivateUser("AAAAAAAAAAA1!");

            User user9 = new User(Role.Patient, "kevinDeBruyne@gmail.com");
            user9.ActivateUser("AAAAAAAAAAA1!");

            User user10 = new User(Role.Patient, "jeremyDoku@gmail.com");
            user10.ActivateUser("AAAAAAAAAAA1!");

            users.Add(user1);
            users.Add(user2);
            users.Add(user3);
            users.Add(user3);
            users.Add(user4);
            users.Add(user5);
            users.Add(user6);
            users.Add(user7);
            users.Add(user8);
            users.Add(user9);
            users.Add(user10);

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        public void SpecializationBootstrap()
        {
            if (_context.Specializations.Any()) return;

            var specializations = new[]
            {
                new Specialization(new SpecializationName("Cardiology"),"A100","A branch of medicine focused on the study, diagnosis, and treatment of disorders of the heart and blood vessels, including conditions like heart disease, arrhythmias, and hypertension."),
                new Specialization(new SpecializationName("Dermatology"),"B100","A branch of medicine that deals with the diagnosis, treatment, and prevention of diseases and conditions of the skin, hair, and nails, such as acne, eczema, psoriasis, and skin cancer."),
                new Specialization(new SpecializationName("Ophthalmology"),"C100","A branch of medicine and surgery that specializes in the diagnosis, treatment, and prevention of diseases and conditions of the eyes and visual system, such as cataracts, glaucoma, and retinal disorders."),
                new Specialization(new SpecializationName("Orthopedics"),"D100","A branch of medicine focused on the diagnosis, treatment, and prevention of disorders and injuries affecting the musculoskeletal system, including bones, joints, muscles, ligaments, and tendons. Common conditions include fractures, arthritis, and spinal disorders."),
                new Specialization(new SpecializationName("Anesthesia"),"E100","A medical specialty focused on the use of medications and techniques to prevent pain and discomfort during surgery or other medical procedures. It can involve general anesthesia (loss of consciousness), regional anesthesia (numbing a specific area), or local anesthesia (numbing a small, specific part of the body)."),
                new Specialization(new SpecializationName("Instrumenting Nurse"),"F100","A surgical nurse responsible for managing and handling surgical instruments during an operation. They ensure the surgeon has the correct tools promptly, maintain sterility in the operating field, and account for all instruments before and after the procedure to ensure patient safety."),
                new Specialization(new SpecializationName("Circulating Nurse"),"G100","A nurse who works outside the sterile field during surgery, responsible for managing the overall environment of the operating room. Their duties include preparing the surgical area, ensuring patient safety, retrieving additional equipment or supplies as needed, documenting the procedure, and facilitating communication between the surgical team and external personnel."),
                new Specialization(new SpecializationName("Nurse Anesthesia"),"H100","A specialized nursing role where a certified registered nurse anesthetist (CRNA) administers anesthesia, monitors patients during procedures, and manages their recovery from anesthesia. CRNAs work closely with surgeons, anesthesiologists, and other healthcare professionals to ensure patient safety and comfort during surgeries or medical procedures."),
                new Specialization(new SpecializationName("Medical Action Assistant"),"I100","A healthcare professional who supports doctors, nurses, and other medical staff by performing administrative and clinical tasks. These may include preparing medical tools, assisting during procedures, maintaining records, scheduling appointments, and ensuring the smooth operation of a healthcare facility. Their role helps improve efficiency and patient care quality."),
                new Specialization(new SpecializationName("X-ray Technician"),"J100","A healthcare professional, also known as a radiologic technologist, who operates X-ray equipment to capture images of the inside of a patient’s body. These images help doctors diagnose and treat medical conditions such as fractures, infections, or diseases. They also ensure patient safety by minimizing radiation exposure and positioning patients correctly for accurate imaging."),
                new Specialization(new SpecializationName("Cleaning"),"K100",null)
            };
            _context.Specializations.AddRange(specializations);
            _context.SaveChanges();
        }

        public void StaffBootstrap()
        {
            if (_context.Staff.Any()) return;

            var cardiology = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Cardiology");
            var dermatology = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Dermatology");
            var ophthalmology = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Ophthalmology");
            var orthopedics = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Orthopedics");
            var cleaning = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Cleaning");
            var nurse = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Nurse Anesthesia");

            if (cardiology == null || dermatology == null || ophthalmology == null || orthopedics == null || cleaning == null)
            {
                throw new InvalidOperationException("The specializations must be created before the staff");
            }

            List<string> AvailabilitySlots = new List<string>();
            AvailabilitySlots.Add("2024 - 10 - 10T12: 00:00 / 2024 - 10 - 11T15: 00:00");
            AvailabilitySlots.Add("2024 - 10 - 14T12: 00:00 / 2024 - 10 - 19T15: 00:00");

            CreateStaffDto dto1 = new CreateStaffDto
            {
                Name = "Ana Costa",
                LicenseNumber = 13557,
                Phone = "929234899",
                Specialization = "Cleaning",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto2 = new CreateStaffDto
            {
                Name = "Maria Silva",
                LicenseNumber = 28234,
                Phone = "929244899",
                Specialization = "Anesthesia",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2023
            };

            CreateStaffDto dto3 = new CreateStaffDto
            {
                Name = "Mário Ferreira",
                LicenseNumber = 39324,
                Phone = "929244299",
                Specialization = "Cardiology",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Doctor,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto4 = new CreateStaffDto
            {
                Name = "Pedro Campos",
                LicenseNumber = 45173,
                Phone = "929244259",
                Specialization = "Dermatology",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2022
            };

            CreateStaffDto dto5 = new CreateStaffDto
            {
                Name = "Rita Sousa",
                LicenseNumber = 45613,
                Phone = "929244339",
                Specialization = null,
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Technician,
                RecruitmentYear = 2022
            };

            CreateStaffDto dto6 = new CreateStaffDto
            {
                Name = "Sofia Ferreira",
                LicenseNumber = 45123,
                Phone = "929245339",
                Specialization = null,
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2023
            };

            CreateStaffDto dto7 = new CreateStaffDto
            {
                Name = "José Matos",
                LicenseNumber = 25113,
                Phone = "923245369",
                Specialization = "Dermatology",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            CreateStaffDto dto8 = new CreateStaffDto
            {
                Name = "António",
                LicenseNumber = 25163,
                Phone = "923249369",
                Specialization = "Cleaning",
                AvailabilitySlots = AvailabilitySlots,
                Role = Role.Nurse,
                RecruitmentYear = 2024
            };

            List<Staff> staffList = new List<Staff>();

            Staff staff1 = new Staff(dto1, cleaning, 1, "healthcareapp.com");
            Staff staff2 = new Staff(dto2, nurse, 2, "healthcareapp.com");
            Staff staff3 = new Staff(dto3, cardiology, 3, "healthcareapp.com");
            Staff staff4 = new Staff(dto4, orthopedics, 4, "healthcareapp.com");
            Staff staff5 = new Staff(dto5, null, 5, "healthcareapp.com");
            Staff staff6 = new Staff(dto6, ophthalmology, 6, "healthcareapp.com");
            Staff staff7 = new Staff(dto7, dermatology, 7, "healthcareapp.com");
            Staff staff8 = new Staff(dto8, cleaning, 8, "healthcareapp.com");

            staff6.Deactivate();

            staffList.Add(staff1);
            staffList.Add(staff2);
            staffList.Add(staff3);
            staffList.Add(staff4);
            staffList.Add(staff5);
            staffList.Add(staff6);
            staffList.Add(staff7);
            staffList.Add(staff8);

            _context.Staff.AddRange(staffList);
            _context.SaveChanges();
        }

        public void OperationTypeBootstrap()
        {
            if (_context.OperationTypes.Any()) return;

            var cardiology = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Cardiology");
            var dermatology = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Dermatology");
            var ophthalmology = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Ophthalmology");
            var orthopedics = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Orthopedics");
            var anaesthetist = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Anesthesia");
            var instrumentingNurse = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Instrumenting Nurse");
            var circulatingNurse = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Circulating Nurse");
            var nurseAnaesthetist = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Nurse Anesthesia");
            var medicalActionAssistant = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Medical Action Assistant");
            var xrayTechnician = _context.Specializations.FirstOrDefault(s => s.Name.Name == "X-ray Technician");
            var cleaning = _context.Specializations.FirstOrDefault(s => s.Name.Name == "Cleaning");

            if (cardiology == null || dermatology == null || ophthalmology == null || orthopedics == null)
            {
                throw new InvalidOperationException("The specializations must be created before the Operation Type");
            }

            var list1 = new List<OperationTypeRequiredStaff>();
            list1.Add(new OperationTypeRequiredStaff(orthopedics, 3));
            list1.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list1.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list1.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list1.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list1.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list1.Add(new OperationTypeRequiredStaff(cleaning, 1));
            OperationType op1 = new OperationType(
                new OperationTypeName("ACL Reconstruction Surgery"),
                new OperationTypeDuration(45, 60, 30),
                list1);

            var list2 = new List<OperationTypeRequiredStaff>();
            list2.Add(new OperationTypeRequiredStaff(orthopedics, 3));
            list2.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list2.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list2.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list2.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list2.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list2.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op2 = new OperationType(
                new OperationTypeName("Knee Replacement Surgery"),
                new OperationTypeDuration(45, 60, 45),
                list2);

            var list3 = new List<OperationTypeRequiredStaff>();
            list3.Add(new OperationTypeRequiredStaff(orthopedics, 3));
            list3.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list3.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list3.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list3.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list3.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list3.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op3 = new OperationType(
                new OperationTypeName("Shoulder Replacement Surgery"),
                new OperationTypeDuration(45, 90, 45),
                list3);

            var list4 = new List<OperationTypeRequiredStaff>();
            list4.Add(new OperationTypeRequiredStaff(orthopedics, 2));
            list4.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list4.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list4.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list4.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list4.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list4.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op4 = new OperationType(
                new OperationTypeName("Hip Replacement Surgery"),
                new OperationTypeDuration(45, 75, 45),
                list4);

            var list5 = new List<OperationTypeRequiredStaff>();
            list5.Add(new OperationTypeRequiredStaff(orthopedics, 2));
            list5.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list5.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list5.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list5.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list5.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list5.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op5 = new OperationType(
                new OperationTypeName("Meniscal injury treatment"),
                new OperationTypeDuration(45, 45, 20),
                list5);

            var list6 = new List<OperationTypeRequiredStaff>();
            list6.Add(new OperationTypeRequiredStaff(orthopedics, 2));
            list6.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list6.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list6.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list6.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list6.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list6.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op6 = new OperationType(
                new OperationTypeName("Rotator cuff repair"),
                new OperationTypeDuration(45, 80, 30),
                list6);

            var list7 = new List<OperationTypeRequiredStaff>();
            list7.Add(new OperationTypeRequiredStaff(orthopedics, 2));
            list7.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list7.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list7.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list7.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list7.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list7.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op7 = new OperationType(
                new OperationTypeName("Ankle ligaments reconstruction or repair"),
                new OperationTypeDuration(30, 45, 20),
                list7);

            var list8 = new List<OperationTypeRequiredStaff>();
            list8.Add(new OperationTypeRequiredStaff(orthopedics, 2));
            list8.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list8.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list8.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list8.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list8.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list8.Add(new OperationTypeRequiredStaff(xrayTechnician, 1));
            list8.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op8 = new OperationType(
                new OperationTypeName("Lumbar discectomy"),
                new OperationTypeDuration(20, 45, 15),
                list8);

            var list9 = new List<OperationTypeRequiredStaff>();
            list9.Add(new OperationTypeRequiredStaff(orthopedics, 1));
            list9.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list9.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list9.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list9.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list9.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list9.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op9 = new OperationType(
                new OperationTypeName("Trigger finger"),
                new OperationTypeDuration(15, 10, 15),
                list9);

            var list10 = new List<OperationTypeRequiredStaff>();
            list10.Add(new OperationTypeRequiredStaff(orthopedics, 1));
            list10.Add(new OperationTypeRequiredStaff(anaesthetist, 1));
            list10.Add(new OperationTypeRequiredStaff(instrumentingNurse, 1));
            list10.Add(new OperationTypeRequiredStaff(circulatingNurse, 1));
            list10.Add(new OperationTypeRequiredStaff(nurseAnaesthetist, 1));
            list10.Add(new OperationTypeRequiredStaff(medicalActionAssistant, 1));
            list10.Add(new OperationTypeRequiredStaff(cleaning, 1));

            OperationType op10 = new OperationType(
                new OperationTypeName("Carpal tunnel syndrome"),
                new OperationTypeDuration(15, 10, 15),
                list10);



            List<OperationType> opTypeList = new List<OperationType>();

            opTypeList.Add(op1);
            opTypeList.Add(op2);
            opTypeList.Add(op3);
            opTypeList.Add(op4);
            opTypeList.Add(op5);
            opTypeList.Add(op6);
            opTypeList.Add(op7);
            opTypeList.Add(op8);
            opTypeList.Add(op9);
            opTypeList.Add(op10);

            _context.OperationTypes.AddRange(opTypeList);
            _context.SaveChanges();
        }

        public void PatientBootstrap()
        {
            if (_context.Patients.Any()) return;

            var patientDto1 = new CreatePatientDto
            {
                FirstName = "Kevin",
                LastName = "DeBruyne",
                FullName = "Kevin DeBruyne",
                Gender = "M",
                DateOfBirth = new DateTime(1991, 6, 28),
                Email = "kevinDeBruyne@gmail.com",
                Phone = "929888771",
                EmergencyContact = "929111211"
            };

            var patientDto2 = new CreatePatientDto
            {
                FirstName = "Jeremy",
                LastName = "Doku",
                FullName = "Jeremy Doke",
                Gender = "M",
                DateOfBirth = new DateTime(2002, 5, 27),
                Email = "jeremyDoku@gmail.com",
                Phone = "929888777",
                EmergencyContact = "929111222"
            };

            var patientDto3 = new CreatePatientDto
            {
                FirstName = "Savio",
                LastName = "Moreira",
                FullName = "Savio Moreira",
                Gender = "M",
                DateOfBirth = new DateTime(2004, 4, 10),
                Email = "savinho@gmail.com",
                Phone = "999888203",
                EmergencyContact = "919111202"
            };

            List<Patient> patientList = new List<Patient>();

            Patient patient1 = new Patient(patientDto1, "202310000001");
            Patient patient2 = new Patient(patientDto2, "202310000002");
            Patient patient3 = new Patient(patientDto3, "202310000003");

            patientList.Add(patient1);
            patientList.Add(patient2);
            patientList.Add(patient3);

            _context.Patients.AddRange(patientList);
            _context.SaveChanges();
        }

        public void OperationRequestBootstrap()
        {
            if (_context.OperationRequests.Any()) return;

            Staff staff1 = _context.Staff.FirstOrDefault(s => s.Name == "Ana Costa");
            Staff staff2 = _context.Staff.FirstOrDefault(s => s.Name == "Maria Silva");
            Staff staff3 = _context.Staff.FirstOrDefault(s => s.Name == "Mário Ferreira");

            if (staff1 == null || staff2 == null || staff3 == null)
            {
                throw new InvalidOperationException("The staff must be created before the Operation Request");
            }

            Patient patient1 = _context.Patients.FirstOrDefault(p => p.FullName == "Kevin DeBruyne");
            Patient patient2 = _context.Patients.FirstOrDefault(p => p.FullName == "Jeremy Doke");
            Patient patient3 = _context.Patients.FirstOrDefault(p => p.FullName == "Savio Moreira");

            if (patient1 == null || patient2 == null || patient3 == null)
            {
                throw new InvalidOperationException("The patients must be created before the Operation Request");
            }

            OperationType op1 = _context.OperationTypes.FirstOrDefault(o => o.Name.Name == "ACL Reconstruction Surgery");
            OperationType op2 = _context.OperationTypes.FirstOrDefault(o => o.Name.Name == "Knee Replacement Surgery");
            OperationType op3 = _context.OperationTypes.FirstOrDefault(o => o.Name.Name == "Shoulder Replacement Surgery");

            if (op1 == null || op2 == null || op3 == null)
            {
                throw new InvalidOperationException("The operation types must be created before the Operation Request");
            }

            List<OperationRequest> opReqList = new List<OperationRequest>();

            List<string> selectedStaffEmpty = new List<string>();

            OperationRequest opReq1 = new OperationRequest(op1, new DateTime(2024, 10, 11), Priority.Elective, patient1, staff1, "The patient needs a knee replacement surgery", selectedStaffEmpty);
            OperationRequest opReq2 = new OperationRequest(op2, new DateTime(2024, 10, 12), Priority.Urgent, patient1, staff1, "The patient needs a knee replacement surgery", selectedStaffEmpty);
            OperationRequest opReq3 = new OperationRequest(op3, new DateTime(2024, 10, 13), Priority.Emergency, patient1, staff1, "The patient needs a shoulder replacement surgery", selectedStaffEmpty);
            OperationRequest opReq4 = new OperationRequest(op1, new DateTime(2024, 10, 14), Priority.Elective, patient2, staff1, "The patient has a torn ACL and needs surgery", selectedStaffEmpty);
            OperationRequest opReq5 = new OperationRequest(op3, new DateTime(2024, 10, 15), Priority.Urgent, patient2, staff1, "The patient needs a shoulder replacement surgery", selectedStaffEmpty);
            OperationRequest opReq6 = new OperationRequest(op2, new DateTime(2024, 10, 16), Priority.Emergency, patient3, staff1, "The patient needs a knee replacement surgery", selectedStaffEmpty);
            OperationRequest opReq7 = new OperationRequest(op1, new DateTime(2024, 10, 17), Priority.Urgent, patient2, staff2, "The patient has a torn ACL and needs surgery", selectedStaffEmpty);
            OperationRequest opReq8 = new OperationRequest(op2, new DateTime(2024, 10, 18), Priority.Emergency, patient1, staff3, "The patient needs a knee replacement surgery", selectedStaffEmpty);


            OperationRequest opReq9 = new OperationRequest(op1, new DateTime(2025, 11, 20), Priority.Urgent, patient1, staff1, "The patient needs a knee replacement surgery", selectedStaffEmpty);
            OperationRequest opReq10 = new OperationRequest(op3, new DateTime(2025, 11, 20), Priority.Elective, patient2, staff2, "The patient needs a shoulder replacement surgery", selectedStaffEmpty);
            OperationRequest opReq11 = new OperationRequest(op3, new DateTime(2024, 11, 15), Priority.Urgent, patient2, staff3, "The patient needs a shoulder replacement surgery", selectedStaffEmpty);
            OperationRequest opReq12 = new OperationRequest(op2, new DateTime(2024, 11, 15), Priority.Elective, patient1, staff1, "The patient needs a knee replacement surgery", selectedStaffEmpty);
            OperationRequest opReq13 = new OperationRequest(op1, new DateTime(2024, 11, 15), Priority.Emergency, patient3, staff2, "The patient needs a knee replacement surgery", selectedStaffEmpty);
            OperationRequest opReq14 = new OperationRequest(op3, new DateTime(2024, 11, 15), Priority.Urgent, patient2, staff3, "The patient needs a shoulder replacement surgery", selectedStaffEmpty);
            OperationRequest opReq15 = new OperationRequest(op3, new DateTime(2024, 11, 15), Priority.Urgent, patient2, staff1, "The patient needs a shoulder replacement surgery", selectedStaffEmpty);
            OperationRequest opReq16 = new OperationRequest(op2, new DateTime(2025, 11, 20), Priority.Emergency, patient3, staff1, "The patient needs a knee replacement surgery", selectedStaffEmpty);



            List<string> selectedStaff =
            [
                staff1.Email._Email,
                staff2.Email._Email,
                staff3.Email._Email
            ];

            opReq2.ChangeStatus(selectedStaff);
            opReq5.ChangeStatus(selectedStaff);
            opReq8.ChangeStatus(selectedStaff);
            opReq9.ChangeStatus(selectedStaff);
            opReq10.ChangeStatus(selectedStaff);
            opReq11.ChangeStatus(selectedStaff);
            opReq12.ChangeStatus(selectedStaff);
            opReq13.ChangeStatus(selectedStaff);
            opReq14.ChangeStatus(selectedStaff);
            opReq15.ChangeStatus(selectedStaff);
            opReq16.ChangeStatus(selectedStaff);

            opReqList.Add(opReq1);
            opReqList.Add(opReq2);
            opReqList.Add(opReq3);
            opReqList.Add(opReq4);
            opReqList.Add(opReq5);
            opReqList.Add(opReq6);
            opReqList.Add(opReq7);
            opReqList.Add(opReq8);
            opReqList.Add(opReq9);
            opReqList.Add(opReq10);
            opReqList.Add(opReq11);
            opReqList.Add(opReq12);
            opReqList.Add(opReq13);
            opReqList.Add(opReq14);
            opReqList.Add(opReq15);
            opReqList.Add(opReq16);

            _context.OperationRequests.AddRange(opReqList);
            _context.SaveChanges();
        }

        public void SurgeryRoomTypeBootstrap()
        {
            // OperatingRoom,
            // ConsultationRoom,
            // ICU

            if (_context.SurgeryRoomTypes.Any()) return;
            SurgeryRoomType s1 = new SurgeryRoomType("OperatingRoom", true, "Room utilized for the realization of surgical proceadures");
            SurgeryRoomType s2 = new SurgeryRoomType("ConsultationRoom", true, "Room utilized for the realization of consltation proceadures");
            SurgeryRoomType s3 = new SurgeryRoomType("ICU", true, "Room utilized for patient recovery");

            List<SurgeryRoomType> surgeryRoomTypes =
            [
                s1,
                s2,
                s3
            ];

            _context.SurgeryRoomTypes.AddRange(surgeryRoomTypes);
            _context.SaveChanges();

        }

        public void SurgeryRoomBootstrap()
        {
            if (_context.SurgeryRooms.Any()) return;

            var timeSlot1 = new TimeSlot("2024-11-17 01:00", "2024-11-17 6:00");
            var timeSlot2 = new TimeSlot("2024-11-17 03:00", "2024-11-17 04:00");
            var timeSlot3 = new TimeSlot("2024-11-18 04:00", "2024-11-18 05:00");
            var timeSlot4 = new TimeSlot("2024-11-18 05:00", "2024-11-18 06:00");
            var timeSlot5 = new TimeSlot("2024-11-19 06:00", "2024-11-19 07:00");

            try
            {
                var type = _context.SurgeryRoomTypes.FirstOrDefault(st => st.Type == "OperatingRoom");
            } catch (Exception){
                throw new Exception("SurgeryRoomType error on bootstrap.");
            }

            SurgeryRoom surgeryRoom1 = new SurgeryRoom(101, _context.SurgeryRoomTypes.FirstOrDefault(st => st.Type == "OperatingRoom"), 10, new List<TimeSlot> { timeSlot1 });
            SurgeryRoom surgeryRoom2 = new SurgeryRoom(102, _context.SurgeryRoomTypes.FirstOrDefault(st => st.Type == "ConsultationRoom"), 10, new List<TimeSlot> { timeSlot2 });
            SurgeryRoom surgeryRoom3 = new SurgeryRoom(103, _context.SurgeryRoomTypes.FirstOrDefault(st => st.Type == "ICU"), 10, new List<TimeSlot> { timeSlot3 });
            SurgeryRoom surgeryRoom4 = new SurgeryRoom(104, _context.SurgeryRoomTypes.FirstOrDefault(st => st.Type == "OperatingRoom"), 10, new List<TimeSlot> { timeSlot4 });
            SurgeryRoom surgeryRoom5 = new SurgeryRoom(105, _context.SurgeryRoomTypes.FirstOrDefault(st => st.Type == "OperatingRoom"), 10, new List<TimeSlot> { timeSlot5 });

            surgeryRoom4.ChangeOccupiedStatus();
            surgeryRoom5.ChangeUnderMaintenanceStatus();

            List<SurgeryRoom> surgeryRoomList = new List<SurgeryRoom>
            {
                surgeryRoom1,
                surgeryRoom2,
                surgeryRoom3,
                surgeryRoom4,
                surgeryRoom5,
            };

            _context.SurgeryRooms.AddRange(surgeryRoomList);
            _context.SaveChanges();
        }


        public void AppointmentBootstrap()
        {
            if (_context.Appointments.Any()) return;

            var operationRequests = _context.OperationRequests.ToList();
            if (!operationRequests.Any())
            {
                throw new InvalidOperationException("OperationRequests must be created before creating Appointments.");
            }

            var surgeryRooms = _context.SurgeryRooms.ToList();
            if (!surgeryRooms.Any())
            {
                throw new InvalidOperationException("SurgeryRooms must be created before creating Appointments.");
            }

            var appointments = new List<Appointment>();

            var opRequest = operationRequests[0];
            var opRequest2 = operationRequests[4];
            var opRequest3 = operationRequests[2];
            var opRequest4 = operationRequests[5];
            var opRequest5 = operationRequests[3];
            var opRequest6 = operationRequests[8];

            var surgeryRoom = surgeryRooms[0];
            var surgeryRoom2 = surgeryRooms[1];
            var surgeryRoom3 = surgeryRooms[0];
            var surgeryRoom4 = surgeryRooms[1];
            var surgeryRoom5 = surgeryRooms[0];
            var surgeryRoom6 = surgeryRooms[1];


            var appointment = new Appointment(
                opRequest,
                surgeryRoom,
                new DateTime(2025, 01, 03, 18, 00, 00)
            );

            var appointment2 = new Appointment(
                opRequest2,
                surgeryRoom2,
                new DateTime(2025, 10, 10, 10, 00, 00)
            );

            var appointment3 = new Appointment(
                opRequest3,
                surgeryRoom3,
                new DateTime(2025, 01, 16, 15, 00, 00)
            );

            var appointment4 = new Appointment(
                opRequest4,
                surgeryRoom4,
                new DateTime(2025, 01, 06, 08, 00, 00)
            );

            var appointment5 = new Appointment(
                opRequest5,
                surgeryRoom5,
                new DateTime(2025, 01, 06, 08, 00, 00)
            );

            var appointment6 = new Appointment(
                opRequest6,
                surgeryRoom6,
                new DateTime(2025, 01, 16, 15, 30, 00)
            );

            appointments.Add(appointment);
            appointments.Add(appointment2);
            appointments.Add(appointment3);
            appointments.Add(appointment4);
            appointments.Add(appointment5);
            appointments.Add(appointment6);


            _context.Appointments.AddRange(appointments);
            _context.SaveChanges();
        }

    }
}
