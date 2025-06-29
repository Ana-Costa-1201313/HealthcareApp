import expressLoader from "./express";
import dependencyInjectorLoader from "./dependencyInjector";
import mongooseLoader from "./mongoose";
import Logger from "./logger";

import config from "../../config";

export default async ({ expressApp }) => {
  const mongoConnection = await mongooseLoader();
  Logger.info("DB loaded and connected!");

  const allergySchema = {
    name: "allergySchema",
    schema: "../persistence/schemas/allergySchema",
  };

  const allergyController = {
    name: config.controllers.allergy.name,
    path: config.controllers.allergy.path,
  };

  const allergyService = {
    name: config.services.allergy.name,
    path: config.services.allergy.path,
  };

  const allergyRepo = {
    name: config.repos.allergy.name,
    path: config.repos.allergy.path,
  };

  const medicalConditionSchema = {
    name: "medicalConditionSchema",
    schema: "../persistence/schemas/medicalConditionSchema",
  };

  const medicalConditionController = {
    name: config.controllers.medicalCondition.name,
    path: config.controllers.medicalCondition.path,
  };


  const medicalConditionService = {
    name: config.services.medicalCondition.name,
    path: config.services.medicalCondition.path,
  };

  const medicalConditionRepo = {
    name: config.repos.medicalCondition.name,
    path: config.repos.medicalCondition.path,
  };

  const patientMedicalRecordSchema = {
    name: "patientMedicalRecordSchema",
    schema: "../persistence/schemas/patientMedicalRecordSchema"
  };

  const patientMedicalRecordController = {
    name: config.controllers.patientMedicalRecord.name,
    path: config.controllers.patientMedicalRecord.path,
  };

  const patientMedicalRecordService = {
    name: config.services.patientMedicalRecord.name,
    path: config.services.patientMedicalRecord.path,
  };

  const patientMedicalRecordRepo = {
    name: config.repos.patientMedicalRecord.name,
    path: config.repos.patientMedicalRecord.path,
  };

  const authService = {
    name: config.services.auth.name,
    path: config.services.auth.path,
  };

  const externalApiService = {
    name: config.services.externalApi.name,
    path: config.services.externalApi.path,
  };

  await dependencyInjectorLoader({
    mongoConnection,
    schemas: [allergySchema, medicalConditionSchema, patientMedicalRecordSchema],
    controllers: [allergyController, medicalConditionController, patientMedicalRecordController],
    repos: [allergyRepo, medicalConditionRepo, patientMedicalRecordRepo],
    services: [allergyService, medicalConditionService, patientMedicalRecordService, authService, externalApiService],
  });

  Logger.info("Schemas, Controllers, Repositories, Services, etc. loaded");

  await expressLoader({ app: expressApp });
  Logger.info("Express loaded");
};
