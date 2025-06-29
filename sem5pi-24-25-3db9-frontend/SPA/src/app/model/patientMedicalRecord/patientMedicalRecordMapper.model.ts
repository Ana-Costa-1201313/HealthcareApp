import { PatientMedicalRecord } from "./patientMedicalRecord.model";
import { PatientMedicalRecordDto } from "./patientMedicalRecordDto.model";

export class PatientMedicalRecordMapper {
    static toMedicalRecord(dto: PatientMedicalRecordDto): PatientMedicalRecord {
        return {
            id: dto.id,
            patientId: dto.patientId,
            allergies: dto.allergies,
            medicalConditions: dto.medicalConditions,
            familyHistory: dto.familyHistory,
            freeText: dto.freeText
        };
    }

    static toDto(record: PatientMedicalRecord): PatientMedicalRecordDto{
        return {
            id: record.id,
            patientId: record.patientId,
            allergies: record.allergies,
            medicalConditions: record.medicalConditions,
            familyHistory: record.familyHistory,
            freeText: record.freeText
        };
    }
}