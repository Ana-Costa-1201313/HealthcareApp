import mongoose from "mongoose";
import { IAllergyPersistence } from "../IAllergyPersistence";

const AllergySchema = new mongoose.Schema(
  {
    name: { type: String, unique: true },
    code: { type: String, unique: true },
    description: { type: String, default: null },
  },
  {
    timestamps: true,
  }
);

export default mongoose.model<IAllergyPersistence & mongoose.Document>(
  "Allergy",
  AllergySchema
);
