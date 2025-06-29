import { RequiredStaff } from "./requiredStaff.model";

export type OperationType = {
    id?: string;
    name?: string;
    anesthesiaPatientPreparationInMinutes?: number;
    surgeryInMinutes?: number;
    cleaningInMinutes?: number;
    requiredStaff?: RequiredStaff[];
    active?: boolean;
    specialization?: string; //for filter, because is a nested object inside required staff
}