import { Repo } from "../../core/infra/Repo";
import { Result } from "../../core/logic/Result";
import { PatientMedicalRecord } from "../../domain/patientMedicalRecord";

export default interface IPatientMedicalRecordRepository extends Repo<PatientMedicalRecord>{
    save(record: PatientMedicalRecord): Promise<Result<PatientMedicalRecord>>;
    getAll(): Promise<Result<PatientMedicalRecord[]>>;
    getById(id: string): Promise<Result<PatientMedicalRecord>>;
    getAllergiesFromRecord(recordId: string): Promise<Result<string[]>>;
    getMedicalConditionsFromRecord(recordId: string): Promise<Result<string[]>>;
    getFamilyHistoryFromRecord(recordId: string): Promise<Result<string[]>>;
    getFreeTextFromRecord(recordId: string): Promise<Result<string[]>>;
    updateAllergies(recordId: string, updatedAllergyNames: string[]): Promise<Result<PatientMedicalRecord>>;
    updateMedicalConditions(recordId: string, updatedMedicalConditions: string[]): Promise<Result<PatientMedicalRecord>>;
    updateFamilyHistory(recordId: string, updatedFamilyHistory: string[]): Promise<Result<PatientMedicalRecord>>;
    updateFreeText(recordId: string, updatedFreeText: string[]): Promise<Result<PatientMedicalRecord>>;
    getByPatientId(id: string): Promise<Result<PatientMedicalRecord>>;

}