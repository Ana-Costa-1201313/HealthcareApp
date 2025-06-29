import { RequiredStaff } from "./requiredStaff.model";

export type OperationTypeDto = {
    id?: string;
    name?: string;
    anesthesiaPatientPreparationInMinutes?: number;
    surgeryInMinutes?: number;
    cleaningInMinutes?: number;
    requiredStaff?: RequiredStaff[];
};