import { Repo } from "../../core/infra/Repo";
import { Result } from "../../core/logic/Result";
import { MedicalCondition } from "../../domain/medicalCondition/medicalCondition";

export default interface IMedicalConditionRepository extends Repo<MedicalCondition> {
  save(medicalCondition: MedicalCondition): Promise<Result<MedicalCondition>>;
  getAll(query: any): Promise<Result<MedicalCondition[]>>;
  getById(id: string): Promise<Result<MedicalCondition>>;
  update(medicalCondition: MedicalCondition): Promise<Result<MedicalCondition>>;
}
