export type CreatePickedOperationRequest = {
    opTypeName: string;
    deadlineDate: string;
    priority: string;
    patientEmail: string;
    doctorEmail: string;
    description: string;
    staffList: string[];
    };