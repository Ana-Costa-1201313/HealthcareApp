    export type PatientMedicalRecord = {
        id?: string
        patientId?: string; 
        allergies?: string[];
        medicalConditions?: string[];
        familyHistory?: string[];
        freeText?: string[];
    }