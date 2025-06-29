export interface IPatientMedicalRecordPersistence {
    id: string;
    patientId: string;
    allergies: string[];
    medicalConditions: string[];
    familyHistory: string[];
    freeText: string[];
}