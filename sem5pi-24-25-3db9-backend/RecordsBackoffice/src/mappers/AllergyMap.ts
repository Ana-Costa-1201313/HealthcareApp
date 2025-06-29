import { forEach } from "lodash";
import { Mapper } from "../core/infra/Mapper";
import { Result } from "../core/logic/Result";
import { Allergy } from "../domain/allergy";
import IAllergyDto from "../dto/IAllergyDto";
import { UniqueEntityID } from "../core/domain/UniqueEntityID";

export class AllergyMap extends Mapper<Allergy> {
  public static toDto(allergy: Allergy): IAllergyDto {
    return {
      id: allergy.id.toString(),
      name: allergy.name,
      code: allergy.code,
      description: allergy.description,
    } as IAllergyDto;
  }

  public static toDtoList(allergies: Allergy[]): IAllergyDto[] {
    const allergyListDto: IAllergyDto[] = [];

    allergies.forEach((allergy) => {
      try {
        allergyListDto.push(this.toDto(allergy));
      } catch (e) {}
    });

    return allergyListDto;
  }

  public static toDomain(allergy: any): Allergy {
    const allergyOrError = Allergy.create(
      allergy,
      new UniqueEntityID(allergy._id)
    );

    return allergyOrError.isSuccess ? allergyOrError.getValue() : null;
  }

  public static toDomainList(allergies: any): Allergy[] {
    const allergyList: Allergy[] = [];

    allergies.forEach((allergy) => {
      allergyList.push(this.toDomain(allergy));
    });

    return allergyList;
  }

  public static toPersistence(allergy: Allergy): any {
    return {
      id: allergy.id.toString(),
      name: allergy.name,
      code: allergy.code,
      description: allergy.description,
    };
  }
}
