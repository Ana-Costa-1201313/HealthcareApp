import { Router } from "express";
import Container from "typedi";
import IAllergyController from "../../controllers/IControllers/IAllergyController";
import { celebrate, Joi } from "celebrate";
import config from "../../../config";

const route = Router();

export default (app: Router) => {
  app.use("/allergies", route);

  const controller = Container.get(
    config.controllers.allergy.name
  ) as IAllergyController;

  route.post(
    "",
    celebrate({
      body: Joi.object({
        name: Joi.string().allow(""),
        code: Joi.string().allow(""),
        description: Joi.string().allow(null, ""),
      }),
    }),
    (request, response, next) =>
      controller.createAllergy(request, response, next)
  );

  route.get(
    "",
    celebrate({
      body: Joi.object({}),
    }),
    (request, response, next) => controller.getAllergy(request, response, next)
  );

  route.put(
    "/:id",
    celebrate({
      body: Joi.object({
        name: Joi.string().allow(""),
        code: Joi.string().allow(""),
        description: Joi.string().allow(null, ""),
      }),
      params: Joi.object({
        id: Joi.string().required(),
      }),
    }),
    (request, response, next) =>
      controller.editAllergy(request, response, next)
  );
};
