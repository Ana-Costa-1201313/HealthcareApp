import { NextFunction, Request, Response } from "express";

export default interface IMedicalConditionController {
  createMedicalCondition(request: Request, response: Response, next: NextFunction);
  getAll(request: Request, response: Response, next: NextFunction);
  updateMedicalCondition(request: Request, response: Response, next: NextFunction);
}
