import { Inject, Service } from "typedi";
import { Result } from "../core/logic/Result";
import IAllergyService from "./IServices/IAllergyService";
import config from "../../config";
import IAllergyRepository from "../repos/IRepos/IAllergyRepository";
import { AllergyMap } from "../mappers/AllergyMap";
import IAllergyDto from "../dto/IAllergyDto";

@Service()
export default class AllergyService implements IAllergyService {
  constructor(
    @Inject(config.repos.allergy.name) private allergyRepo: IAllergyRepository
  ) {}

  public async createAllergy(dto: IAllergyDto): Promise<Result<IAllergyDto>> {
    const allergy = AllergyMap.toDomain(dto);

    if (allergy == null) {
      return Result.fail<IAllergyDto>("Error: Can't create the allergy!");
    }

    const savedAllergy = await this.allergyRepo.save(allergy);

    if (savedAllergy.isFailure) {
      return Result.fail<IAllergyDto>(savedAllergy.errorValue());
    }

    const allergyDto = AllergyMap.toDto(savedAllergy.getValue());

    return Result.ok<IAllergyDto>(allergyDto);
  }

  public async getAllergy(
    name?: string,
    code?: string
  ): Promise<Result<IAllergyDto[]>> {
    const query: any = {};

    if (name) {
      query["name"] = { $regex: name, $options: "i" };
    }

    if (code) {
      query["code"] = code;
    }

    const allergyList = await this.allergyRepo.get(query);

    if (allergyList.isFailure) {
      return Result.fail<IAllergyDto[]>(allergyList.errorValue());
    }

    const allergyListDto = AllergyMap.toDtoList(allergyList.getValue());

    return Result.ok<IAllergyDto[]>(allergyListDto);
  }

  public async editAllergy(
    id: string,
    dto: IAllergyDto
  ): Promise<Result<IAllergyDto>> {
    const allergy = await this.allergyRepo.findAllergyById(id);

    if (allergy.isFailure) {
      return Result.fail<IAllergyDto>(allergy.errorValue());
    }

    const editedAllergy = allergy.getValue().edit(dto);

    if (editedAllergy.isFailure) {
      return Result.fail<IAllergyDto>(editedAllergy.errorValue());
    }

    const savedAllergy = await this.allergyRepo.update(allergy.getValue());

    if (savedAllergy.isFailure) {
      return Result.fail<IAllergyDto>(savedAllergy.errorValue());
    }

    const allergyDto = AllergyMap.toDto(savedAllergy.getValue());

    return Result.ok<IAllergyDto>(allergyDto);
  }
}
