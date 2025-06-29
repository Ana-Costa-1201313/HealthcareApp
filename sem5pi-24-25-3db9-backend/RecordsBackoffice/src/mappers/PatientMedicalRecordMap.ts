import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import { Mapper } from "../core/infra/Mapper";
import { PatientMedicalRecord } from "../domain/patientMedicalRecord";
import IPatientMedicalRecordDto from "../dto/IPatientMedicalRecordDto";

export class PatientMedicalRecordMap extends Mapper<PatientMedicalRecord> {
    public static toDto(record: PatientMedicalRecord): IPatientMedicalRecordDto {
        return {
            id: record.id.toString(),
            patientId: record.patientId,
            allergies: record.allergyNames,
            medicalConditions: record.medicalConditionNames,
            familyHistory: record.familyHistory,
            freeText: record.freeText,
        }as IPatientMedicalRecordDto;
    }

    public static toDomain(record: any): PatientMedicalRecord{
        const recordOrError = PatientMedicalRecord.create(
            {
                patientId: record.patientId,
                allergies: record.allergies,
                medicalConditions: record.medicalConditions,
                familyHistory: record.familyHistory,
                freeText: record.freeText,
            },
            record.id ? new UniqueEntityID(record.id) : undefined
        );
        return recordOrError.isSuccess ? recordOrError.getValue() : null;
    }

    public static toPersistence(record: PatientMedicalRecord): any {
        return{
            id: record.id.toString(),
            patientId: record.patientId,
            allergies: record.allergyNames,
            medicalConditions: record.medicalConditionNames,
            familyHistory: record.familyHistory,
            freeText: record.freeText,
        };
    }

    public static toDtoList(records: PatientMedicalRecord[]): IPatientMedicalRecordDto[]{
        return records.map((patientMedicalRecord) => this.toDto(patientMedicalRecord));
    }

    public static toDomainList(rawRecords: any[]): PatientMedicalRecord[]{
        return rawRecords.map((raw) => this.toDomain(raw));
    }


}