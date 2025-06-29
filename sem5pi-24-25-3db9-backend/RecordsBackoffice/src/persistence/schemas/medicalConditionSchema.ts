import mongoose from "mongoose";
import { IMedicalConditionPersistence } from "../IMedicalConditionPersistence";

const MedicalConditionSchema = new mongoose.Schema(
  {
    id: { type: String, unique: true},
    name: { type: String, unique: true },
    code: { type: String, unique: true },
    description: { type: String },
    symptoms: [{ type: String }],
  },
  {
    timestamps: true,
  }
);

export default mongoose.model<IMedicalConditionPersistence & mongoose.Document>(
  "MedicalCondition",
  MedicalConditionSchema
);
