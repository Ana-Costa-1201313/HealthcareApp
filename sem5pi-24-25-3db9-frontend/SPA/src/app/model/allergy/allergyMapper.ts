import { Allergy } from './allergy.model';
import { AllergyDto } from './dto/allergyDto';
import { CreateAllergyDto } from './dto/createAllergyDto';
import { EditAllergyDto } from './dto/editAllergyDto';

export class AllergyMapper {
  static toAllergy(dto: AllergyDto): Allergy {
    return {
      id: dto.id,
      name: dto.name,
      code: dto.code,
      description: dto.description,
    };
  }

  static toAllergyCreate(dto: CreateAllergyDto): Allergy {
    return {
      name: dto.name,
      code: dto.code,
      description: dto.description,
    };
  }

  static toAllergyEdit(dto: EditAllergyDto): Allergy {
    return {
      name: dto.name,
      code: dto.code,
      description: dto.description,
    };
  }
}
