import { Inject, Service } from "typedi";
import IPatientMedicalRecordService from "./IServices/IPatientMedicalRecordService";
import config from "../../config";
import IPatientMedicalRecordDto from "../dto/IPatientMedicalRecordDto";
import { Result } from "../core/logic/Result";
import { PatientMedicalRecordMap } from "../mappers/PatientMedicalRecordMap";
import IPatientMedicalRecordRepository from "../repos/IRepos/IPatientMedicalRecordRepository";

@Service()
export default class PatientMedicalRecordService implements IPatientMedicalRecordService {

    constructor(
        @Inject(config.repos.patientMedicalRecord.name) private recordRepo: IPatientMedicalRecordRepository
    ) { }


    public async createMedicalRecord(dto: IPatientMedicalRecordDto): Promise<Result<IPatientMedicalRecordDto>> {
        const record = PatientMedicalRecordMap.toDomain(dto);

        if (record == null)
            return Result.fail<IPatientMedicalRecordDto>("Error: Can't create a patient medical record");

        const existingRecord = await this.recordRepo.getByPatientId(record.patientId);

        if (existingRecord.isSuccess) {
            return Result.fail<IPatientMedicalRecordDto>("Error: Email is already in use");
        }

        const savedRecord = await this.recordRepo.save(record);

        if (savedRecord.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(savedRecord.errorValue());

        const recordDto = PatientMedicalRecordMap.toDto(savedRecord.getValue());
        return Result.ok<IPatientMedicalRecordDto>(recordDto);
    }

    public async updateMedicalRecordAllergies(id: string, allergyNames: string[]): Promise<Result<IPatientMedicalRecordDto>> {
        const record = await this.recordRepo.getById(id);

        if (record.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(record.errorValue());

        const updatedRecord = await this.recordRepo.updateAllergies(id, allergyNames);

        if (updatedRecord.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(updatedRecord.errorValue());

        const recordDto = PatientMedicalRecordMap.toDto(updatedRecord.getValue());
        return Result.ok<IPatientMedicalRecordDto>(recordDto);

    }

    public async updateMedicalRecordMedicalConditions(id: string, medicalConditionNames: string[]): Promise<Result<IPatientMedicalRecordDto>> {
        const record = await this.recordRepo.getById(id);

        if (record.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(record.errorValue());
        const updatedRecord = await this.recordRepo.updateMedicalConditions(id, medicalConditionNames);

        if (updatedRecord.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(updatedRecord.errorValue());

        const recordDto = PatientMedicalRecordMap.toDto(updatedRecord.getValue());
        return Result.ok<IPatientMedicalRecordDto>(recordDto);
    }

    public async updateMedicalRecordFamilyHistory(id: string, familyHistory: string[]): Promise<Result<IPatientMedicalRecordDto>> {
        
        const record = await this.recordRepo.getById(id);

        if (record.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(record.errorValue());

        const updatedRecord = await this.recordRepo.updateFamilyHistory(id, familyHistory);

        if (updatedRecord.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(updatedRecord.errorValue());


        const recordDto = PatientMedicalRecordMap.toDto(updatedRecord.getValue());
        return Result.ok<IPatientMedicalRecordDto>(recordDto);
    }

    public async updateMedicalRecordFreeText(id: string, freeText: string[]): Promise<Result<IPatientMedicalRecordDto>> {
        //console.log("Fetching record for patientId:", id);
        const record = await this.recordRepo.getById(id);
        //console.log("Fetched record:", record);

        if(record.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(record.errorValue());

        //console.log("Updating record free text:", freeText);
        const updatedRecord = await this.recordRepo.updateFreeText(id,freeText);
        //console.log("Updated result:", updatedRecord);

        
        if(updatedRecord.isFailure)
            return Result.fail<IPatientMedicalRecordDto>(updatedRecord.errorValue());
        
        const recordDto = PatientMedicalRecordMap.toDto(updatedRecord.getValue());
        return Result.ok<IPatientMedicalRecordDto>(recordDto);
    }

    public async getAllMedicalRecords(): Promise<Result<IPatientMedicalRecordDto[]>> {
        const records = await this.recordRepo.getAll();

        if (records.isFailure)
            return Result.fail<IPatientMedicalRecordDto[]>(records.errorValue());

        const recordsDto = PatientMedicalRecordMap.toDtoList(records.getValue());
        return Result.ok<IPatientMedicalRecordDto[]>(recordsDto);
    }

    public async getMedicalRecordMedicalConditions(id: string): Promise<Result<string[]>> {
        const record = await this.recordRepo.getById(id);

        if (record.isFailure)
            return Result.fail<string[]>(record.errorValue());
        return Result.ok<string[]>(record.getValue().medicalConditionNames);
    }

    public async getMedicalRecordAllergies(id: string): Promise<Result<string[]>> {
        const record = await this.recordRepo.getById(id);

        if (record.isFailure)
            return Result.fail<string[]>(record.errorValue());
        return Result.ok<string[]>(record.getValue().allergyNames);
    }

    public async getMedicalRecordFamilyHistory(id: string): Promise<Result<string[]>> {
        const record = await this.recordRepo.getById(id);

        if (record.isFailure)
            return Result.fail<string[]>(record.errorValue());
        return Result.ok<string[]>(record.getValue().familyHistory);
    }

    public async getMedicalRecordById(id: string): Promise<Result<IPatientMedicalRecordDto>> {
        const record = await this.recordRepo.getByPatientId(id);

        if (record.isFailure) {
            return Result.fail<IPatientMedicalRecordDto>(record.errorValue());
        }

        return Result.ok<IPatientMedicalRecordDto>(PatientMedicalRecordMap.toDto(record.getValue()));
    }

    public async getMedicalRecordFreeText(id: string): Promise<Result<string[]>> {
        const record = await this.recordRepo.getById(id);

        if(record.isFailure)
            return Result.fail<string[]>(record.errorValue());
        return Result.ok<string[]>(record.getValue().freeText);
    }

}