import { NextFunction, Request, Response } from "express";

export default interface IAllergyController {
  createAllergy(request: Request, response: Response, next: NextFunction);
  getAllergy(request: Request, response: Response, next: NextFunction);
  editAllergy(request: Request, response: Response, next: NextFunction);
}
