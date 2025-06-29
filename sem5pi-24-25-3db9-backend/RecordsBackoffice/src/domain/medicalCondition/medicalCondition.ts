import { AggregateRoot } from "../../core/domain/AggregateRoot";
import { UniqueEntityID } from "../../core/domain/UniqueEntityID";
import { Result } from "../../core/logic/Result";
import IMedicalConditionDto from "../../dto/IMedicalConditionDto";
import { MedicalConditionId } from "./medicalConditionId";

interface MedicalConditionProps {
  name: string;
  code: string;
  description: string;
  symptoms: string[];
}

export class MedicalCondition extends AggregateRoot<MedicalConditionProps> {
  get id(): UniqueEntityID {
    return this._id;
  }

  get medicalConditionId(): MedicalConditionId {
    return MedicalConditionId.caller(this.id);
  }

  get name(): string {
    return this.props.name;
  }

  get code(): string {
    return this.props.code;
  }

  get description(): string {
    return this.props.description;
  }

  get symptoms(): string[] {
    return this.props.symptoms;
  }

  private constructor(props: MedicalConditionProps, id?: UniqueEntityID) {
    super(props, id);
  }

  public static create(medicalConditionDto: IMedicalConditionDto, id?: UniqueEntityID): Result<MedicalCondition> {
    const name = medicalConditionDto.name;
    const code = medicalConditionDto.code;
    const description = medicalConditionDto.description;
    const symptoms = medicalConditionDto.symptoms;

    if (!!name === false || name.trim().length === 0) {
      return Result.fail<MedicalCondition>("Error: The medical condition must have a name!");
    }

    if (!!code === false || code.trim().length === 0) {
      return Result.fail<MedicalCondition>("Error: The medical condition must have a code!");
    }

    if (!!description === false || description.trim().length === 0) {
      return Result.fail<MedicalCondition>("Error: The medical condition must have a description!");
    }

    // if (!!familyHistory === false || familyHistory.length === 0) {
    //   return Result.fail<MedicalCondition>("Error: The medical condition must have a family history!");
    // }

    const medicalCondition = new MedicalCondition({
      name: name,
      code: code,
      description: description,
      symptoms: symptoms,
    }, id);
    return Result.ok<MedicalCondition>(medicalCondition);
  }

  public update(medicalConditionDto: IMedicalConditionDto): Result<void> {
    const name = medicalConditionDto.name;
    const description = medicalConditionDto.description;
    const symptoms = medicalConditionDto.symptoms || [];

    if (!!name === false || name.trim().length === 0) {
      return Result.fail<void>("Error: The medical condition must have a name!");
    }

    if (!!description === false || description.trim().length === 0) {
      return Result.fail<void>("Error: The medical condition must have a description!");
    }

    this.props.name = name;
    this.props.description = description;
    this.props.symptoms = symptoms;

    return Result.ok<void>();
  }
}
