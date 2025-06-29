import { OperationType } from "./operationType.model";

export type OperationRequest = {
    deadlineDate?: string;
    description?: string;
    doctorId?: string;
    doctorName?: string;
    id?: string;
    opTypeId?: string;
    opTypeName?: OperationType;
    patientId?: string;
    patientName?: string;
    priority?: string;
};