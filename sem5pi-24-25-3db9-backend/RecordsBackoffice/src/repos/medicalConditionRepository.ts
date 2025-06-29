import { Inject, Service } from "typedi";
import { Document, Model } from "mongoose";
import { Result } from "../core/logic/Result";
import IMedicalConditionRepository from "./IRepos/IMedicalConditionRepository";
import { IMedicalConditionPersistence } from "../persistence/IMedicalConditionPersistence";
import { MedicalCondition } from "../domain/medicalCondition/medicalCondition";
import { MedicalConditionMap } from "../mappers/MedicalConditionMap";

@Service()
export default class MedicalConditionRepository implements IMedicalConditionRepository {
  constructor(
    @Inject("medicalConditionSchema")
    private medicalConditionSchema: Model<IMedicalConditionPersistence & Document>
  ) { }

  public exists(medicalCondition: MedicalCondition): Promise<Result<boolean>> {
    return null;
  }

  public async save(medicalCondition: MedicalCondition): Promise<Result<MedicalCondition>> {
    const queryName = { name: medicalCondition.name };
    const queryCode = { code: medicalCondition.code };

    const medicalConditionDocByName = await this.medicalConditionSchema.findOne(queryName);
    const medicalConditionDocByCode = await this.medicalConditionSchema.findOne(queryCode);

    if (medicalConditionDocByName === null && medicalConditionDocByCode === null) {
      const rawMedicalCondition: any = MedicalConditionMap.toPersistence(medicalCondition);

      const medicalConditionCreated = await this.medicalConditionSchema.create(rawMedicalCondition);

      return Result.ok<MedicalCondition>(medicalCondition);
    }

    if (medicalConditionDocByName != null) {
      return Result.fail<MedicalCondition>(
        "Error: This medical condition name is already in use!"
      );
    }

    if (medicalConditionDocByCode != null) {
      return Result.fail<MedicalCondition>(
        "Error: This medical condition code is already in use!"
      );
    } else {
      return Result.fail<MedicalCondition>("Error: Can't save this data!");
    }
  }

  public async getAll(query: any): Promise<Result<MedicalCondition[]>> {
    try {
      const medicalConditionList = await this.medicalConditionSchema.find(query);

      return Result.ok<MedicalCondition[]>(MedicalConditionMap.toDomainList(medicalConditionList));
    } catch (e) {
      return Result.fail<MedicalCondition[]>("Error: Can't get medical conditions!");
    }
  }
  public async getById(id: string): Promise<Result<MedicalCondition>> {
    try {
      const medicalCondition = await this.medicalConditionSchema.findOne({ id: id });

      if (medicalCondition === null) {
        return Result.fail<MedicalCondition>("Error: This medical condition doesn't exist!");
      }

      return Result.ok<MedicalCondition>(MedicalConditionMap.toDomain(medicalCondition));
    } catch (e) {
      return Result.fail<MedicalCondition>("Error: Can't find this medical condition!");
    }
  }

  public async update(medicalCondition: MedicalCondition): Promise<Result<MedicalCondition>> {

    const queryName = { 
      name: medicalCondition.name,
      id: { $ne: medicalCondition.id.toString() } 
    };

    const medicalConditionName = await this.medicalConditionSchema.findOne(queryName);

    if (medicalConditionName === null) {
      const rawMedicalCondition: any = MedicalConditionMap.toPersistence(medicalCondition);

      const updatedMedicalCondition = await this.medicalConditionSchema.findOneAndUpdate(
        { id: rawMedicalCondition.id },
        rawMedicalCondition,
        { new: true }
      );

      if (!updatedMedicalCondition) {
        return Result.fail<MedicalCondition>(
          "Error: Medical Condition not found or could not be updated!"
        );
      }
      return Result.ok<MedicalCondition>(MedicalConditionMap.toDomain(updatedMedicalCondition));
    }

    if (medicalConditionName != null) {
      return Result.fail<MedicalCondition>(
        "Error: This medical condition name is already in use!"
      );
    }

    return Result.fail<MedicalCondition>("Error: Can't save this data!");


  }
}
