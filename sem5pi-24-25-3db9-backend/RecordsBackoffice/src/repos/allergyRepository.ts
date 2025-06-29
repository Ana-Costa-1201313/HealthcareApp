import { Inject, Service } from "typedi";
import IAllergyRepository from "./IRepos/IAllergyRepository";
import { Document, Model } from "mongoose";
import { IAllergyPersistence } from "../persistence/IAllergyPersistence";
import { Allergy } from "../domain/allergy";
import { AllergyMap } from "../mappers/AllergyMap";
import { Result } from "../core/logic/Result";

@Service()
export default class AllergyRepository implements IAllergyRepository {
  constructor(
    @Inject("allergySchema")
    private allergySchema: Model<IAllergyPersistence & Document>
  ) {}

  public exists(allergy: Allergy): Promise<Result<boolean>> {
    return null;
  }

  public async save(allergy: Allergy): Promise<Result<Allergy>> {
    const queryName = { name: allergy.name };
    const queryCode = { code: allergy.code };

    const allergyDocByName = await this.allergySchema.findOne(queryName);
    const allergyDocByCode = await this.allergySchema.findOne(queryCode);

    if (allergyDocByName === null && allergyDocByCode === null) {
      const rawAllergy: any = AllergyMap.toPersistence(allergy);

      const allergyCreated = await this.allergySchema.create(rawAllergy);

      return Result.ok<Allergy>(AllergyMap.toDomain(allergyCreated));
    }

    if (allergyDocByName != null) {
      return Result.fail<Allergy>(
        "Error: This allergy name is already in use!"
      );
    }

    if (allergyDocByCode != null) {
      return Result.fail<Allergy>(
        "Error: This allergy code is already in use!"
      );
    } else {
      return Result.fail<Allergy>("Error: Can't save this data!");
    }
  }

  public async get(query: any): Promise<Result<Allergy[]>> {
    try {
      const allergyList = await this.allergySchema.find(query);

      return Result.ok<Allergy[]>(AllergyMap.toDomainList(allergyList));
    } catch (e) {
      return Result.fail<Allergy[]>("Error: Can't get allergies!");
    }
  }

  public async findAllergyById(id: string): Promise<Result<Allergy>> {
    try {
      const allergy = await this.allergySchema.findById(id);

      if (allergy === null) {
        return Result.fail<Allergy>("Error: This allergy doesn't exist!");
      }

      return Result.ok<Allergy>(AllergyMap.toDomain(allergy));
    } catch (e) {
      return Result.fail<Allergy>("Error: Can't find this allergy!");
    }
  }

  public async update(allergy: Allergy): Promise<Result<Allergy>> {
    const queryName = {
      name: allergy.name,
      _id: { $ne: allergy.id.toString() },
    };
    const queryCode = {
      code: allergy.code,
      _id: { $ne: allergy.id.toString() },
    };

    const allergyDocByName = await this.allergySchema.findOne(queryName);
    const allergyDocByCode = await this.allergySchema.findOne(queryCode);

    if (allergyDocByName === null && allergyDocByCode === null) {
      const rawAllergy: any = AllergyMap.toPersistence(allergy);

      const updatedAllergy = await this.allergySchema.findByIdAndUpdate(
        rawAllergy.id,
        rawAllergy,
        { new: true }
      );

      if (!updatedAllergy) {
        return Result.fail<Allergy>(
          "Error: Allergy not found or could not be updated!"
        );
      }
      return Result.ok<Allergy>(AllergyMap.toDomain(updatedAllergy));
    }

    if (allergyDocByName != null) {
      return Result.fail<Allergy>(
        "Error: This allergy name is already in use!"
      );
    }

    if (allergyDocByCode != null) {
      return Result.fail<Allergy>(
        "Error: This allergy code is already in use!"
      );
    } else {
      return Result.fail<Allergy>("Error: Can't save this data!");
    }
  }
}
