export const environment = {
  apiUrl: 'https://localhost:5001/api',
  authApiUrl: 'https://localhost:7058',
  recordsBackofficeApiUrl: 'http://localhost:4000/api',
  endpoints: {
    staff: '/Staff',
    login: '/Auth/Login/loginDto',
    registerNewPatientUser: '/users/createPatient',
    totalRecordsStaff: '/totalRecords',
    patient: '/Patient',
    operationTypes: '/OperationTypes',
    operationRequests: '/OperationRequest',
    specialization: '/Specializations',
    rooms: '/SurgeryRoom',
    operationRequest: '/OperationRequest',
    operationRequestFilterByStatus: "/OperationRequest/list/?status=Picked",
    planning: "/Appointment/planning",
    appointment: "/Appointment",
    patientprofile: "/Patient",
    allergies: "/allergies",
    medicalCondition: "/MedicalConditions",
    patientMedicalRecord: "/patientMedicalRecord"
  },
};
