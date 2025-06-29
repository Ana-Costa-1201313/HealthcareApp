import { OperationType } from "./operationType/operationType.model";

export type OperationRequest = {
    deadlineDate: string | null | undefined;
    description: string | null | undefined;
    doctorId: string | null | undefined;
    doctorName: string | null | undefined;
    id: string | null | undefined;
    opTypeId: string | null | undefined;
    opTypeName: OperationType | null | undefined;
    patientId: string | null | undefined;
    patientName: string | null | undefined;
    priority: string | null | undefined;
    status: string | null | undefined;
    selectedStaff: string[] | null | undefined;
}