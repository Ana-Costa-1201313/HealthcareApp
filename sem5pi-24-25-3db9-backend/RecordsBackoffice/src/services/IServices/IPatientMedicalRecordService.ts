import { Result } from "../../core/logic/Result";
import IPatientMedicalRecordDto from "../../dto/IPatientMedicalRecordDto";

export default interface IPatientMedicalRecordService {
    createMedicalRecord(dto: IPatientMedicalRecordDto): Promise<Result<IPatientMedicalRecordDto>>;
    updateMedicalRecordAllergies(id: string, allergyNames: string[]): Promise<Result<IPatientMedicalRecordDto>>;
    updateMedicalRecordMedicalConditions(id: string, medicalConditionIds: string[]): Promise<Result<IPatientMedicalRecordDto>>;
    updateMedicalRecordFamilyHistory(id: string, familyHistory: string[]): Promise<Result<IPatientMedicalRecordDto>>;
    updateMedicalRecordFreeText(id: string, freeText: string[]): Promise<Result<IPatientMedicalRecordDto>>;
    getAllMedicalRecords(): Promise<Result<IPatientMedicalRecordDto[]>>;
    getMedicalRecordMedicalConditions(id: string): Promise<Result<string[]>>;
    getMedicalRecordAllergies(id: string): Promise<Result<string[]>>;
    getMedicalRecordFamilyHistory(id: string): Promise<Result<string[]>>;
    getMedicalRecordFreeText(id: string): Promise<Result<string[]>>;
    getMedicalRecordById(id: string): Promise<Result<IPatientMedicalRecordDto>>;
}