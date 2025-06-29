import { MedicalCondition } from "./medicalCondition.model";
import { MedicalConditionDto } from "./medicalConditionDto.model";

export class MedicalConditionMapper {
    static toMedicalCondition(dto: MedicalConditionDto): MedicalCondition {
      return {
        id: dto.id, 
        name: dto.name, 
        code: dto.code,
        description: dto.description, 
        symptoms: dto.symptoms
      };
    }
  
    static toDto(medicalCondition: MedicalCondition): MedicalConditionDto {
        return {
            id: medicalCondition.id, 
            name: medicalCondition.name, 
            code: medicalCondition.code,
            description: medicalCondition.description, 
            symptoms: medicalCondition.symptoms
          };
    }
  }
  