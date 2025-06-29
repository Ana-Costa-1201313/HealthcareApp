import { AggregateRoot } from "../core/domain/AggregateRoot";
import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import { Result } from "../core/logic/Result";
import IPatientMedicalRecordDto from "../dto/IPatientMedicalRecordDto";

interface PatientMedicalRecordProps{
    patientId: string;
    allergyNames: string[];
    medicalConditionNames: string[];
    familyHistory: string[];
    freeText: string[];
}
export class PatientMedicalRecord extends AggregateRoot<PatientMedicalRecordProps> {
    get id(): UniqueEntityID{
        return this._id;
    }

    get patientId(): string{
        return this.props.patientId;
    }

    get allergyNames(): string[]{
        return this.props.allergyNames;
    }

    get medicalConditionNames(): string[]{
        return this.props.medicalConditionNames;
    }

    get familyHistory(): string[]{
        return this.props.familyHistory;
    }

    get freeText(): string[]{
        return this.props.freeText;
    }

    public addAllergy(allergyName: string): Result<void> {
            if(this.props.allergyNames.includes(allergyName)){
                return Result.fail<void>("This allergy is already in the patient medical record.");
            }
            this.props.allergyNames.push(allergyName);
            return Result.ok<void>();
    }

    public removeAllergy(allergyName: string): Result<void>{
        const index = this.props.allergyNames.indexOf(allergyName);
        if(index === -1)
            return Result.fail<void>("Allergy not found in the patient medical record.");
        this.props.allergyNames.splice(index,1);
        return Result.ok<void>();
    }

    public addMedicalCondition(mCondition: string): Result<void> {
        if(this.props.medicalConditionNames.includes(mCondition))
            return Result.fail<void>("This medical condition is already in the patient medical record.");
        this.props.medicalConditionNames.push(mCondition);
        return Result.ok<void>();
    }

    public removeMedicalCondition(mCondition: string): Result<void> {
        const index = this.props.medicalConditionNames.indexOf(mCondition);
        if(index === -1)
            return Result.fail<void>("Medical Condition not found in the patient medical record.")
        this.props.medicalConditionNames.splice(index,1);
        return Result.ok<void>();
    }

    public updateFamilyHistory(familyHistory: string[]): void{
        this.props.familyHistory = familyHistory;
    }

    public updateFreeText(freeText: string[]): void{
        this.props.freeText = freeText;
    }

    private constructor(props: PatientMedicalRecordProps, id?: UniqueEntityID){
        super(props,id);
    }

    public static create(
        dto:IPatientMedicalRecordDto,
        id?: UniqueEntityID): Result<PatientMedicalRecord>{
            if(!dto.patientId || dto.patientId.trim().length === 0)
                return Result.fail<PatientMedicalRecord>("Patient Id is required!");
            dto.allergies = dto.allergies || [];
            dto.medicalConditions = dto.medicalConditions || [];
            dto.familyHistory = dto.familyHistory || [];
            dto.freeText = dto.freeText || [];

            const record = new PatientMedicalRecord(
                {
                    patientId: dto.patientId,
                    allergyNames: dto.allergies,
                    medicalConditionNames: dto.medicalConditions,
                    familyHistory: dto.familyHistory,
                    freeText: dto.freeText
                },
                id
            );
            return Result.ok<PatientMedicalRecord>(record);
        }
        
}
