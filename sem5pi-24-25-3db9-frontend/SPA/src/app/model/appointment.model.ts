import { OperationRequest } from "./operationRequest.model";

export type Appointment = {
    appointmentId?: string;
    dateTime?: string;
    opRequestId?: string;
    operationRequest?: OperationRequest;
    status?: string;
    surgeryRoomId?: string;
    surgeryRoomNumber?: string;
};