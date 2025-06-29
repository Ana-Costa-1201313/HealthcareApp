import { AggregateRoot } from "../core/domain/AggregateRoot";
import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import { Result } from "../core/logic/Result";
import IAllergyDto from "../dto/IAllergyDto";

interface AllergyProps {
  name: string;
  code: string;
  description: string;
}

export class Allergy extends AggregateRoot<AllergyProps> {
  get id(): UniqueEntityID {
    return this._id;
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

  private constructor(props: AllergyProps, id?: UniqueEntityID) {
    super(props, id);
  }

  public static create(
    allergyDto: IAllergyDto,
    id?: UniqueEntityID
  ): Result<Allergy> {
    const name = allergyDto.name;
    const code = allergyDto.code;
    const description = allergyDto.description;

    if (!!name === false || name.trim().length === 0) {
      return Result.fail<Allergy>("Error: The allergy must have a name!");
    }

    if (!!code === false || code.trim().length === 0) {
      return Result.fail<Allergy>("Error: The allergy must have a code!");
    } else {
      const allergy = new Allergy(
        {
          name: name,
          code: code,
          description: description,
        },
        id
      );
      return Result.ok<Allergy>(allergy);
    }
  }

  public edit(allergyDto: IAllergyDto): Result<void> {
    const name = allergyDto.name;
    const code = allergyDto.code;
    const description = allergyDto.description;

    if (!!name === false || name.trim().length === 0) {
      return Result.fail<void>("Error: The allergy must have a name!");
    }

    if (!!code === false || code.trim().length === 0) {
      return Result.fail<void>("Error: The allergy must have a code!");
    }

    this.props.name = name;
    this.props.code = code;
    this.props.description = description;

    return Result.ok<void>();
  }
}
