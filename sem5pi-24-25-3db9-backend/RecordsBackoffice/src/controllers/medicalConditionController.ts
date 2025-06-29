import { Inject, Service } from "typedi";
import { Request, Response } from "express";
import config from "../../config";
import IMedicalConditionController from "./IControllers/IMedicalConditionController";
import IMedicalConditionService from "../services/IServices/IMedicalConditionService";
import IMedicalConditionDto from "../dto/IMedicalConditionDto";
import AuthService from "../services/authService";

@Service()
export default class MedicalConditionController implements IMedicalConditionController {
  constructor(
    @Inject(config.services.medicalCondition.name)
    private medicalConditionService: IMedicalConditionService,
    private authService: AuthService
  ) { }

  public async createMedicalCondition(request: Request, response: Response) {

    try {
      await this.authService.isAuthorized(request, ["Admin"]);
    } catch (error) {
      return response.status(403).json({ message: "Error: User not authorized" });
    }

    try {
      const medicalCondition = await this.medicalConditionService.createMedicalCondition(
        request.body as IMedicalConditionDto
      );

      if (medicalCondition.isFailure) {
        return response.status(400).json({ message: medicalCondition.error });
      }

      const medicalConditionDto = medicalCondition.getValue();

      return response.status(201).json(medicalConditionDto);
    } catch (e) {
      return response.status(400).json("Error: Can't create this medical condition!");
    }
  }

  public async getAll(request: Request, response: Response) {

    try {
      await this.authService.isAuthorized(request, ["Admin","Doctor"]);
    } catch (error) {
      console.log("end");
      return response.status(403).json({ message: "Error: User not authorized" });
    }

    try {
      const { name, code } = request.query;

      const medicalConditionList = await this.medicalConditionService.getAll(name as string, code as string);

      if (medicalConditionList.isFailure) {
        return response.status(400).json({ message: medicalConditionList.error });
      }

      return response.status(200).json(medicalConditionList.getValue());
    } catch (e) {
      return response.status(400).json("Error: Can't retrieve medical conditions!");
    }
  }

  public async updateMedicalCondition(request: Request, response: Response) {

    try {
      await this.authService.isAuthorized(request, ["Admin"]);
    } catch (error) {
      return response.status(403).json({ message: "Error: User not authorized" });
    }

    try {
      const { id } = request.params;

      const medicalCondition = await this.medicalConditionService.updateMedicalCondition(
        id,
        request.body as IMedicalConditionDto
      );

      if (medicalCondition.isFailure) {
        return response.status(400).json({ message: medicalCondition.error });
      }

      const medicalConditionDto = medicalCondition.getValue();

      return response.status(200).json(medicalConditionDto);
    } catch (e) {
      return response.status(400).json("Error: Can't update this medical condition!");
    }
  }
}
