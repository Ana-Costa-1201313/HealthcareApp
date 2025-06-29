import { response, Router } from "express";
import config from "../../../config";
import IPatientMedicalRecordController from "../../controllers/IControllers/IPatientMedicalRecordController";
import Container from "typedi";
import { celebrate, Joi } from "celebrate";
import { request } from "http";

const route = Router();

export default (app: Router) => {
  app.use("/patientMedicalRecord", route);


  const controller = Container.get(
    config.controllers.patientMedicalRecord.name
  ) as IPatientMedicalRecordController;

  route.post(
    "",
    celebrate({
      body: Joi.object({
        patientId: Joi.string().required(),
        allergies: Joi.array().items(Joi.string()).allow(null, ""),
        medicalConditions: Joi.array().items(Joi.string()).allow(null, ""),
        familyHistory: Joi.array().items(Joi.string()).allow(null, ""),
        freeText: Joi.array().items(Joi.string()).allow(null, "")
      }),
    }),
    (request, response, next) => controller.createMedicalRecord(request, response, next)
  );

  route.get(
    "",
    (request, response, next) => controller.getAllMedicalRecords(request, response, next)
  );

  route.put(
    "/:id/allergies",
    celebrate({
      body: Joi.object({
        allergies: Joi.array().items(Joi.string()).required(),
      }),
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.updateMedicalRecordAllergies(request, response, next)
  );

  route.put(
    "/:id/medical-conditions",
    celebrate({
      body: Joi.object({
        medicalConditions: Joi.array().items(Joi.string()).required(),
      }),
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.updateMedicalRecordMedicalConditions(request, response, next)
  );

  route.put(
    "/:id/family-history",
    celebrate({
      body: Joi.object({
        familyHistory: Joi.array().items(Joi.string()).required(),
      }),
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.updateMedicalRecordFamilyHistory(request, response, next)
  );

route.put(
    "/:id/free-text",
    celebrate({
        body: Joi.object({
            freeText: Joi.array().items(Joi.string()).required(),
        }),
        params: Joi.object({
            id: Joi.string().required(),
        }),
    }),
    (request,response,next) => controller.updateMedicalRecordFreeText(request,response,next)
);

  route.get(
    "/:id/allergies",
    celebrate({
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.getMedicalRecordAllergies(request, response, next)
  );

  route.get(
    "/:id/medical-conditions",
    celebrate({
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.getMedicalRecordMedicalConditions(request, response, next)

  );

  route.get(
    "/:id/family-history",
    celebrate({
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.getMedicalRecordFamilyHistory(request, response, next)
  );

  route.get(
    "/:id/free-text",
    celebrate({
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.getMedicalRecordFreeText(request, response, next)
  );

  route.get(
    "/:id",
    celebrate({
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) => controller.getMedicalRecordById(request, response, next)

  );
}