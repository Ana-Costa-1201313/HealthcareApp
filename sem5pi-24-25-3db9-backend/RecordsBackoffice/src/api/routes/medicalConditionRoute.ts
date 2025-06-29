import { Router } from "express";
import Container from "typedi";
import { celebrate, Joi } from "celebrate";
import config from "../../../config";
import IMedicalConditionController from "../../controllers/IControllers/IMedicalConditionController";

const route = Router();

export default (app: Router) => {
  app.use("/MedicalConditions", route);

  const controller = Container.get(
    config.controllers.medicalCondition.name
  ) as IMedicalConditionController;

  route.post(
    "",
    celebrate({
      body: Joi.object({
        name: Joi.string().allow(''),
        code: Joi.string().allow(''),
        description: Joi.string().allow(''),
        symptoms: Joi.array().items(Joi.string()).optional(),
      }),
    }),
    (request, response, next) =>
      controller.createMedicalCondition(request, response, next)
  );

  route.get(
    "",
    celebrate({
      body: Joi.object({}),
    }),
    (request, response, next) => controller.getAll(request, response, next)
  );

  route.put(
    "/:id",
    celebrate({
      body: Joi.object({
        name: Joi.string().allow(""),
        code: Joi.string().allow(""),
        description: Joi.string().allow(""),
        symptoms: Joi.array().items(Joi.string()).optional(),
      }),
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) =>
      controller.updateMedicalCondition(request, response, next)
  );
};
