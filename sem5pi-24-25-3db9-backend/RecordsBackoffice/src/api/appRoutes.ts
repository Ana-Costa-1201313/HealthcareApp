import { Router } from "express";
import allergyRoute from "./routes/allergyRoute";
import medicalConditionRoute from "./routes/medicalConditionRoute";
import patientMedicalRecordRoute from "./routes/patientMedicalRecordRoute";

export function getAppRoutes() {
  const app = Router();

  allergyRoute(app);
  medicalConditionRoute(app);
  patientMedicalRecordRoute(app);
  
  return app;
}
