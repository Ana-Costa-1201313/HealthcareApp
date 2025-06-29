import { Inject, Service } from "typedi";
import { Result } from "../core/logic/Result";
import config from "../../config";
import IMedicalConditionService from "./IServices/IMedicalConditionService";
import IMedicalConditionRepository from "../repos/IRepos/IMedicalConditionRepository";
import IMedicalConditionDto from "../dto/IMedicalConditionDto";
import { MedicalConditionMap } from "../mappers/MedicalConditionMap";

@Service()
export default class MedicalConditionService implements IMedicalConditionService {
  constructor(
    @Inject(config.repos.medicalCondition.name) private medicalConditionRepo: IMedicalConditionRepository
  ) { }

  public async createMedicalCondition(dto: IMedicalConditionDto): Promise<Result<IMedicalConditionDto>> {
    const medicalCondition = MedicalConditionMap.toResultDomain(dto);

    if (medicalCondition.isFailure) {
      return Result.fail<IMedicalConditionDto>(medicalCondition.errorValue());
    }

    const savedMedicalCondition = await this.medicalConditionRepo.save(medicalCondition.getValue());

    if (savedMedicalCondition.isFailure) {
      return Result.fail<IMedicalConditionDto>(savedMedicalCondition.errorValue());
    }

    const medicalConditionDto = MedicalConditionMap.toDto(savedMedicalCondition.getValue());

    return Result.ok<IMedicalConditionDto>(medicalConditionDto);
  }

  public async getAll(name?: string, code?: string): Promise<Result<IMedicalConditionDto[]>> {
    const query: any = {};

    if (name) {
      query["name"] = { $regex: name, $options: "i" };
    }

    if (code) {
      query["code"] = { $regex: code, $options: "i" };
    }

    const medicalConditionList = await this.medicalConditionRepo.getAll(query);

    if (medicalConditionList.isFailure) {
      return Result.fail<IMedicalConditionDto[]>(medicalConditionList.errorValue());
    }

    const medicalConditionListDto = MedicalConditionMap.toDtoList(medicalConditionList.getValue());

    return Result.ok<IMedicalConditionDto[]>(medicalConditionListDto);
  }

  public async updateMedicalCondition(id: string, dto: IMedicalConditionDto): Promise<Result<IMedicalConditionDto>> {
    const medicalCondition = await this.medicalConditionRepo.getById(id);

    if (medicalCondition.isFailure) {
      return Result.fail<IMedicalConditionDto>(medicalCondition.errorValue());
    }

    const updatedMedicalCondition = medicalCondition.getValue().update(dto);

    if (updatedMedicalCondition.isFailure) {
      return Result.fail<IMedicalConditionDto>(updatedMedicalCondition.errorValue());
    }

    const savedMedicalCondition = await this.medicalConditionRepo.update(medicalCondition.getValue());

    if (savedMedicalCondition.isFailure) {
      return Result.fail<IMedicalConditionDto>(savedMedicalCondition.errorValue());
    }

    const medicalConditionDto = MedicalConditionMap.toDto(savedMedicalCondition.getValue());

    return Result.ok<IMedicalConditionDto>(medicalConditionDto);
  }
}
