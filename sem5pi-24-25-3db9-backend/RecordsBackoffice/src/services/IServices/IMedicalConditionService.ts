import { Result } from "../../core/logic/Result";
import IMedicalConditionDto from "../../dto/IMedicalConditionDto";

export default interface IMedicalConditionService {
  createMedicalCondition(medicalConditionDto: IMedicalConditionDto): Promise<Result<IMedicalConditionDto>>;
  getAll(name: string, code: string): Promise<Result<IMedicalConditionDto[]>>;
  updateMedicalCondition(id: string, medicalConditionDto: IMedicalConditionDto): Promise<Result<IMedicalConditionDto>>;
}
