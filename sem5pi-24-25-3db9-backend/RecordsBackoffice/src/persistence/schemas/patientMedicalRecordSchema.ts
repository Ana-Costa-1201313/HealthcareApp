import mongoose from "mongoose";
import { IPatientMedicalRecordPersistence } from "../IPatientMedicalRecordPersistence";

const PatientMedicalRecordSchema = new mongoose.Schema(
    {
        id: {type: String, unique:true},
        patientId: {type: String, required: true},
        allergies: [{type: String, required: true}],
        medicalConditions: [{type: String, required: true}],
        familyHistory: [{type:String, required: true}],
        freeText: [{type: String, required: true}],
    },
    {
        timestamps: true,
    }
);

    export default mongoose.model<IPatientMedicalRecordPersistence & mongoose.Document>(
        "PatientMedicalRecord",
        PatientMedicalRecordSchema
    );
    