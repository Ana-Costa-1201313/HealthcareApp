import dotenv from "dotenv";
import medicalCondition from "./src/api/routes/medicalConditionRoute";

// Set the NODE_ENV to 'development' by default
process.env.NODE_ENV = process.env.NODE_ENV || "development";

const envFound = dotenv.config();
if (!envFound) {
  // This error should crash whole process

  throw new Error("⚠️  Couldn't find .env file  ⚠️");
}

export default {
  /**
   * Your favorite port : optional change to 4000 by JRT
   */
  port: parseInt(process.env.PORT, 10) || 4000,

  /**
   * That long string from mlab
   */
  databaseURL:
    process.env.MONGODB_URI ||
    "mongodb://mongoadmin:6f271fd90312e23f439557f8@vsgate-s1.dei.isep.ipp.pt:10932",

  /**
   * Your secret sauce
   */
  jwtSecret:
    process.env.JWT_SECRET || "my sakdfho2390asjod$%jl)!sdjas0i secret",

  /**
   * Used by winston logger
   */
  logs: {
    level: process.env.LOG_LEVEL || "info",
  },

  /**
   * API configs
   */
  api: {
    prefix: "/api",
  },

  controllers: {
    allergy: {
      name: "AllergyController",
      path: "../controllers/allergyController",
    },
    medicalCondition: {
      name: "MedicalConditionController",
      path: "../controllers/medicalConditionController",
    },
    patientMedicalRecord: {
      name: "PatientMedicalRecordController",
      path: "../controllers/patientMedicalRecordController",
    }
  },

  repos: {
    allergy: {
      name: "AllergyRepository",
      path: "../repos/allergyRepository",
    },
    medicalCondition: {
      name: "MedicalConditionRepository",
      path: "../repos/medicalConditionRepository",
    },
    patientMedicalRecord: {
      name: "PatientMedicalRecordRepository",
      path: "../repos/patientMedicalRecordRepository"
    }
  },

  services: {
    allergy: {
      name: "AllergyService",
      path: "../services/allergyService",
    },
    medicalCondition: {
      name: "MedicalConditionService",
      path: "../services/medicalConditionService",
    },
    patientMedicalRecord: {
      name: "PatientMedicalRecordService",
      path: "../services/patientMedicalRecordService"
    },
    auth: {
      name: "AuthService",
      path: "../services/authService",
    },
    externalApi: {
      name: "ExternalApiService",
      path: "../services/externalApiService",
    }
  },
};
