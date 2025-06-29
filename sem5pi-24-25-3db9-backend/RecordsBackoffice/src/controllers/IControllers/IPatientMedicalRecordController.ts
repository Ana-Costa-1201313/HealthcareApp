import { NextFunction, Request, Response } from "express";

export default interface IPatientMedicalRecordController{
    createMedicalRecord(request: Request, response: Response, next: NextFunction);
    getAllMedicalRecords(request: Request,response:Response, next: NextFunction);
    updateMedicalRecordAllergies(request: Request, response: Response, next: NextFunction);
    updateMedicalRecordMedicalConditions(request: Request, response: Response, next: NextFunction);
    updateMedicalRecordFamilyHistory(request: Request, response: Response, next: NextFunction);
    updateMedicalRecordFreeText(request: Request, response: Response, next: NextFunction);
    getMedicalRecordMedicalConditions(request: Request, response: Response, next: NextFunction);
    getMedicalRecordAllergies(request: Request, response: Response, next: NextFunction);
    getMedicalRecordFamilyHistory(request: Request, response: Response, next: NextFunction);
    getMedicalRecordFreeText(request: Request, response: Response, next: NextFunction);
    getMedicalRecordById(request: Request, response: Response, next: NextFunction);
}