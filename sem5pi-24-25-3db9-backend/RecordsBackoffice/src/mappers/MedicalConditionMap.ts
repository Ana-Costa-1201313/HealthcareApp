import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import { Mapper } from "../core/infra/Mapper";
import { Result } from "../core/logic/Result";
import { MedicalCondition } from "../domain/medicalCondition/medicalCondition";
import IMedicalConditionDto from "../dto/IMedicalConditionDto";

export class MedicalConditionMap extends Mapper<MedicalCondition> {
  public static toDto(medicalCondition: MedicalCondition): IMedicalConditionDto {
    return {
      id: medicalCondition.id.toString(),
      name: medicalCondition.name,
      code: medicalCondition.code,
      description: medicalCondition.description,
      symptoms: medicalCondition.symptoms,
    } as IMedicalConditionDto;
  }

  public static toDomain(medicalCondition: any): MedicalCondition {
    const medicalConditionOrError = MedicalCondition.create({
      name: medicalCondition.name,
      code: medicalCondition.code,
      description: medicalCondition.description,
      symptoms: medicalCondition.symptoms,
    }, new UniqueEntityID(medicalCondition.id));

    return medicalConditionOrError.isSuccess ? medicalConditionOrError.getValue() : null;
  }

  public static toResultDomain(medicalCondition: any): Result<MedicalCondition> {
    return MedicalCondition.create(medicalCondition);
  }

  public static toPersistence(medicalCondition: MedicalCondition): any {
    return {
      id: medicalCondition.id.toString(),
      name: medicalCondition.name,
      code: medicalCondition.code,
      description: medicalCondition.description,
      symptoms: medicalCondition.symptoms,
    };
  }

  public static toDomainList(medicalConditions: any): MedicalCondition[] {
    const medicalConditionList: MedicalCondition[] = [];

    medicalConditions.forEach((medicalCondition) => {
      medicalConditionList.push(this.toDomain(medicalCondition));
    });

    return medicalConditionList;
  }

  public static toDtoList(medicalConditions: MedicalCondition[]): IMedicalConditionDto[] {
    const medicalConditionListDto: IMedicalConditionDto[] = [];

    medicalConditions.forEach((medicalCondition) => {
      medicalConditionListDto.push(this.toDto(medicalCondition));

    });

    return medicalConditionListDto;
  }
}
