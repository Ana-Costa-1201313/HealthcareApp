import { Result } from "../../core/logic/Result";
import IAllergyDto from "../../dto/IAllergyDto";

export default interface IAllergyService {
  createAllergy(allergyDto: IAllergyDto): Promise<Result<IAllergyDto>>;
  getAllergy(name: string, code: string): Promise<Result<IAllergyDto[]>>;
  editAllergy(
    id: string,
    allergyDto: IAllergyDto
  ): Promise<Result<IAllergyDto>>;
}
