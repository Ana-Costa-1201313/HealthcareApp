import { Inject, Service } from "typedi";
import IAllergyController from "./IControllers/IAllergyController";
import { Request, Response } from "express";
import IAllergyDto from "../dto/IAllergyDto";
import config from "../../config";
import IAllergyService from "../services/IServices/IAllergyService";
import AuthService from "../services/authService";

@Service()
export default class AllergyController implements IAllergyController {
  constructor(
    @Inject(config.services.allergy.name)
    private allergyService: IAllergyService,
    private authService: AuthService
  ) { }

  public async createAllergy(request: Request, response: Response) {

    try {
      await this.authService.isAuthorized(request, ["Admin"]);
    } catch (error) {
      return response.status(403).json({ message: "Error: User not authorized" });
    }

    try {
      const allergy = await this.allergyService.createAllergy(
        request.body as IAllergyDto
      );

      if (allergy.isFailure) {
        return response.status(400).json({ message: allergy.error });
      }

      const allergyDto = allergy.getValue();

      return response.status(201).json(allergyDto);
    } catch (e) {
      return response.status(400).json("Error: Can't create this allergy!");
    }
  }

  public async getAllergy(request: Request, response: Response) {

    try {
      await this.authService.isAuthorized(request, ["Admin","Doctor"]);
    } catch (error) {
      return response.status(403).json({ message: "Error: User not authorized" });
    }

    try {
      const { name, code } = request.query;

      const allergyList = await this.allergyService.getAllergy(
        name as string,
        code as string
      );

      if (allergyList.isFailure) {
        return response.status(400).json({ message: allergyList.error });
      }

      return response.status(200).json(allergyList.getValue());
    } catch (e) {
      return response.status(400).json("Error: Can't get the allergies!");
    }
  }

  public async editAllergy(request: Request, response: Response) {

    try {
      await this.authService.isAuthorized(request, ["Admin"]);
    } catch (error) {
      return response.status(403).json({ message: "Error: User not authorized" });
    }

    try {
      const { id } = request.params;

      const allergy = await this.allergyService.editAllergy(
        id,
        request.body as IAllergyDto
      );

      if (allergy.isFailure) {
        return response.status(400).json({ message: allergy.error });
      }

      const allergyDto = allergy.getValue();

      return response.status(200).json(allergyDto);
    } catch (e) {
      return response.status(400).json("Error: Can't edit this allergy!");
    }
  }
}
