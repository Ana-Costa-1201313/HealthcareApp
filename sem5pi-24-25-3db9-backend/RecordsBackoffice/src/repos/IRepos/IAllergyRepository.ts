import { Repo } from "../../core/infra/Repo";
import { Result } from "../../core/logic/Result";
import { Allergy } from "../../domain/allergy";

export default interface IAllergyRepository extends Repo<Allergy> {
  save(allergy: Allergy): Promise<Result<Allergy>>;
  get(query: any): Promise<Result<Allergy[]>>;
  findAllergyById(id: string): Promise<Result<Allergy>>;
  update(allergy: Allergy): Promise<Result<Allergy>>;
}
