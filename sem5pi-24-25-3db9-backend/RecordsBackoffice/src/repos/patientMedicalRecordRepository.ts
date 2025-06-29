import { Inject, Service } from "typedi";
import { Document, Model } from "mongoose";
import IPatientMedicalRecordRepository from "./IRepos/IPatientMedicalRecordRepository";
import { IPatientMedicalRecordPersistence } from "../persistence/IPatientMedicalRecordPersistence";
import { Result } from "../core/logic/Result";
import { PatientMedicalRecord } from "../domain/patientMedicalRecord";
import { PatientMedicalRecordMap } from "../mappers/PatientMedicalRecordMap";
import { IAllergyPersistence } from "../persistence/IAllergyPersistence";
import { IMedicalConditionPersistence } from "../persistence/IMedicalConditionPersistence";

@Service()
export default class PatientMedicalRecordRepository implements IPatientMedicalRecordRepository{
    constructor(
        @Inject("patientMedicalRecordSchema")
        private patientMedicalRecordSchema: Model<IPatientMedicalRecordPersistence & Document>,

        @Inject("allergySchema")
        private allergySchema: Model<IAllergyPersistence & Document>,

        @Inject("medicalConditionSchema")
        private medicalConditionSchema: Model<IMedicalConditionPersistence & Document>

    ) {}

    public exists(t: PatientMedicalRecord): Promise<Result<boolean>> {
        return null;
    }

    public async save(record: PatientMedicalRecord): Promise<Result<PatientMedicalRecord>> {
        const query = {id: record.id.toString()};
        const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

        if(!existingRecord){
            const rawRecord = PatientMedicalRecordMap.toPersistence(record);
            const createdRecord = await this.patientMedicalRecordSchema.create(rawRecord);
            return Result.ok<PatientMedicalRecord>(record);
        }
        
        return Result.fail<PatientMedicalRecord>("Error: Record Already Exists!");
    }

    public async getAll(): Promise<Result<PatientMedicalRecord[]>> {
        try{
            const records = await this.patientMedicalRecordSchema.find();

            return Result.ok<PatientMedicalRecord[]>(
                PatientMedicalRecordMap.toDomainList(records)
            );

        }catch (error){
            return Result.fail<PatientMedicalRecord[]>("Error: Can't get patient medical records!")
        }
    }

    public async getById(id: string): Promise<Result<PatientMedicalRecord>>{
        try{
            const record = await this.patientMedicalRecordSchema.findOne({id});
            if(!record)
                return Result.fail<PatientMedicalRecord>("Error: No record found with this id!");
            return Result.ok<PatientMedicalRecord>(PatientMedicalRecordMap.toDomain(record));
        }catch (error){
            return Result.fail<PatientMedicalRecord>("Error: Can't find this patient medical record");
        }
    }
    // Obter todas as alergias de um determinado medical record
   public async getAllergiesFromRecord(recordId: string): Promise<Result<string[]>> {
    const query = { id: recordId};
    const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

    if(!existingRecord) 
        return Result.fail<string[]>("Error: No record found with this id!");
    
    return Result.ok<string[]>(existingRecord.allergies);
   }

   // Obter todas as medical conditions de um determinado medical record
   public async getMedicalConditionsFromRecord(recordId: string): Promise<Result<string[]>>{
    const query = {id: recordId};
    const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

    if(!existingRecord)
        return Result.fail<string[]>("Error: No record found with this id!");
    return Result.ok<string[]>(existingRecord.medicalConditions);
   }

    // Obter todo o family history de um determinado medical record
    public async getFamilyHistoryFromRecord(recordId: string): Promise<Result<string[]>> {
        const query = {id: recordId};
        const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

        if(!existingRecord)
            return Result.fail<string[]>("Error: No record found with this id!");
        return Result.ok<string[]>(existingRecord.familyHistory);
    }

    // Obter todo o free text de um determinado medical record
    public async getFreeTextFromRecord(recordId: string): Promise<Result<string[]>> {
        const query = {id: recordId};
        const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

        if(!existingRecord)
            return Result.fail<string[]>("Error: No record found with this id!");
        return Result.ok<string[]>(existingRecord.freeText);
    }

   //update da parte das alergias de um dado medical record
   public async updateAllergies(recordId: string, updatedAllergyNames: string[]): Promise<Result<PatientMedicalRecord>>{
    const query = {id: recordId};
    const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

    if(!existingRecord)
        return Result.fail<PatientMedicalRecord>("Error: No Record found with this id!");

    const validAllergies = await this.allergySchema.find({ name: {$in: updatedAllergyNames} }).exec();
    if(validAllergies.length !== updatedAllergyNames.length) {
        return Result.fail<PatientMedicalRecord>("Error: One or more invalid allergies!");
    }

    existingRecord.allergies = updatedAllergyNames;
    const updatedRecord = await existingRecord.save();

    return Result.ok<PatientMedicalRecord>(PatientMedicalRecordMap.toDomain(updatedRecord));
   }

   
   //update da parte das medicalConditions de um dado medical record
   public async updateMedicalConditions(recordId: string, updatedMedicalConditions: string[]): Promise<Result<PatientMedicalRecord>> {
    const query = {id: recordId};
    const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

    if(!existingRecord)
        return Result.fail<PatientMedicalRecord>("Error: No Record found with this id!");

    const validMedicalConditions = await this.medicalConditionSchema.find({name: {$in: updatedMedicalConditions} }).exec();
    if(validMedicalConditions.length !== updatedMedicalConditions.length){
        return Result.fail<PatientMedicalRecord>("Error: One or more invalid medical conditions!");
    }
    
    existingRecord.medicalConditions = updatedMedicalConditions;
    const updatedRecord = await existingRecord.save();

    return Result.ok<PatientMedicalRecord>(PatientMedicalRecordMap.toDomain(updatedRecord));
   }

    //update do family history de um dado medical record
    public async updateFamilyHistory(recordId: string, updatedFamilyHistory: string[]): Promise<Result<PatientMedicalRecord>> {
        const query = {id: recordId};
        const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

        if(!existingRecord)
            return Result.fail<PatientMedicalRecord>("Error: No Record found with this id!");

        // Validation: Check for empty or whitespace-only entries
        const hasInvalidEntries = updatedFamilyHistory.some(text => !text.trim());
        if (hasInvalidEntries) {
            return Result.fail<PatientMedicalRecord>("Error: Family history entries cannot be empty or whitespace!");
        }

        // Update the record
        existingRecord.familyHistory = updatedFamilyHistory;
        const updatedRecord = await existingRecord.save();
        return Result.ok<PatientMedicalRecord>(PatientMedicalRecordMap.toDomain(updatedRecord));
    }

    //update do free text de um dado medical record
    public async updateFreeText(recordId: string, updatedFreeText: string[]): Promise<Result<PatientMedicalRecord>> {
        const query = {id: recordId};
        const existingRecord = await this.patientMedicalRecordSchema.findOne(query);

        if(!existingRecord)
            return Result.fail<PatientMedicalRecord>("Error: No Record found with this id!");

        // Validation: Check for empty or whitespace-only entries
        const hasInvalidEntries = updatedFreeText.some(text => !text.trim());
        if (hasInvalidEntries) {
            return Result.fail<PatientMedicalRecord>("Error: Free text entries cannot be empty or whitespace!");
        }

        // Update the record
        existingRecord.freeText = updatedFreeText;
        const updatedRecord = await existingRecord.save();

        return Result.ok<PatientMedicalRecord>(PatientMedicalRecordMap.toDomain(updatedRecord));
   }

   public async getByPatientId(id: string): Promise<Result<PatientMedicalRecord>>{
    try{
        const record = await this.patientMedicalRecordSchema.findOne({ patientId: id });
        if(!record)
            return Result.fail<PatientMedicalRecord>("Error: No record found with this email!");
        return Result.ok<PatientMedicalRecord>(PatientMedicalRecordMap.toDomain(record));
    }catch (error){
        return Result.fail<PatientMedicalRecord>("Error: Can't find this patient medical record");
    }
}

}