import { Inject, Service } from "typedi";
import IPatientMedicalRecordController from "./IControllers/IPatientMedicalRecordController";
import config from "../../config";
import IPatientMedicalRecordService from "../services/IServices/IPatientMedicalRecordService";
import { Request, Response } from "express";
import IPatientMedicalRecordDto from "../dto/IPatientMedicalRecordDto";

@Service()
export default class PatientMedicalRecordController implements IPatientMedicalRecordController{
    constructor(
        @Inject(config.services.patientMedicalRecord.name)
        private patientMedicalRecordService: IPatientMedicalRecordService
    ) {}

    //Criar o medical record
    public async createMedicalRecord(request: Request, response: Response){
        try{
            const recordDto = request.body as IPatientMedicalRecordDto;
            const result  = await this.patientMedicalRecordService.createMedicalRecord(recordDto);

            if(result.isFailure)
                return response.status(400).json({message: result.error});
            return response.status(201).json(result.getValue());
        }catch (e){
            return response.status(400).json("Error: Can't create this medical record!")
        }
    }

    //Obter todos os patient medical records
    public async getAllMedicalRecords(request: Request,response:Response){
        try{
            const result = await this.patientMedicalRecordService.getAllMedicalRecords();

            if(result.isFailure)
                return response.status(400).json({message: result.error})

            return response.status(200).json(result.getValue());
        }catch (e){
            return response.status(400).json("Error: Can't get medical records!");
        }
    }

    //Update das alergias de um paciente
    public async updateMedicalRecordAllergies(request: Request, response: Response) {
        try {
          const { id } = request.params;
          const allergyNames: string[] = request.body.allergies;
    
          const result = await this.patientMedicalRecordService.updateMedicalRecordAllergies(id, allergyNames);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't update allergies for this medical record!");
        }
      }

      //Update das medical conditions de um paciente
      public async updateMedicalRecordMedicalConditions(request: Request, response: Response) {
        try {
          const { id } = request.params;
          const medicalConditionIds: string[] = request.body.medicalConditions;
    
          const result = await this.patientMedicalRecordService.updateMedicalRecordMedicalConditions(id, medicalConditionIds);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't update medical conditions for this medical record!");
        }
      }

      //Update do historico familiar de um paciente
      public async updateMedicalRecordFamilyHistory(request: Request, response: Response) {
        try {
          const { id } = request.params;
          const familyHistory: string[] = request.body.familyHistory;
    
          const result = await this.patientMedicalRecordService.updateMedicalRecordFamilyHistory(id, familyHistory);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't update family history for this medical record!");
        }
      }

      //Update do free text de um paciente
      public async updateMedicalRecordFreeText(request: Request, response: Response) {
        try {
          const { id } = request.params;
          const freeText: string[] = request.body.freeText;
    
          const result = await this.patientMedicalRecordService.updateMedicalRecordFreeText(id, freeText);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't update free text for this medical record!");
        }
      }
      
      //Obter as medical Conditions de um paciente
      public async getMedicalRecordMedicalConditions(request: Request, response: Response) {
        try {
          const { id } = request.params;
    
          const result = await this.patientMedicalRecordService.getMedicalRecordMedicalConditions(id);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't get medical conditions for this patient!");
        }
      }
    
      // Obter alergias de um paciente
      public async getMedicalRecordAllergies(request: Request, response: Response) {
        try {
          const { id } = request.params;
    
          const result = await this.patientMedicalRecordService.getMedicalRecordAllergies(id);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't get allergies for this patient!");
        }
      }

      //Obter o historico familiar de um paciente
      public async getMedicalRecordFamilyHistory(request: Request, response: Response) {
        try {
          const { id } = request.params;
    
          const result = await this.patientMedicalRecordService.getMedicalRecordFamilyHistory(id);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't get family history for this patient!");
        }
      }

      //Obter o free text de um paciente
      public async getMedicalRecordFreeText(request: Request, response: Response) {
        try {
          const { id } = request.params;
    
          const result = await this.patientMedicalRecordService.getMedicalRecordFreeText(id);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't get free text for this patient!");
        }
      }

      public async getMedicalRecordById(request: Request, response: Response) {
        try {
          const { id } = request.params;
    
          const result = await this.patientMedicalRecordService.getMedicalRecordById(id);
    
          if (result.isFailure) {
            return response.status(400).json({ message: result.error });
          }
    
          return response.status(200).json(result.getValue());
        } catch (e) {
          return response.status(400).json("Error: Can't get medical record with that email!");
        }
      }
    }