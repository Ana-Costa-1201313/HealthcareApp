import { expect } from "chai";
import { Result } from "../../core/logic/Result";
import { MedicalCondition } from "../../domain/medicalCondition/medicalCondition";
import IMedicalConditionDto from "../../dto/IMedicalConditionDto";

describe("Medical Condition Test", () => {
  it("should create medical condition", () => {
    const dto = {
      name: "name",
      code: "code",
      description: "desc",
      symptoms: []
    };

    const result: Result<MedicalCondition> = MedicalCondition.create(dto);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue().name).to.equal("name");
    expect(result.getValue().code).to.equal("code");
    expect(result.getValue().description).to.equal("desc");
    expect(result.getValue().symptoms.length).to.equal(0);

  });

  it("should send error if medical condition name is empty", () => {
    const dto = {
      name: "",
      code: "code",
      description: "desc",
      symptoms: []
    };

    const result: Result<MedicalCondition> = MedicalCondition.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a name!");
  });

  it("should send error if medical condition name is null", () => {
    const dto = {
      name: null,
      code: "code",
      description: "desc",
      symptoms: []
    };

    const result: Result<MedicalCondition> = MedicalCondition.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a name!");
  });

  it("should send error if medical condition code is empty", () => {
    const dto = {
      name: "name",
      code: "",
      description: "desc",
      symptoms: []
    };

    const result: Result<MedicalCondition> = MedicalCondition.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a code!");
  });

  it("should send error if medical condition code is null", () => {
    const dto = {
      name: "name",
      code: null,
      description: "desc",
      symptoms: []
    };

    const result: Result<MedicalCondition> = MedicalCondition.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a code!");
  });

  it("should send error if medical condition description is empty", () => {
    const dto = {
      name: "name",
      code: "code",
      description: "",
      symptoms: []
    };

    const result: Result<MedicalCondition> = MedicalCondition.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a description!");
  });

  it("should send error if medical condition description is null", () => {
    const dto = {
      name: "name",
      code: "code",
      description: null,
      symptoms: []
    };

    const result: Result<MedicalCondition> = MedicalCondition.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a description!");
  });

  let medicalCondition: MedicalCondition;

  beforeEach(() => {
    const initialDto: IMedicalConditionDto = {
      name: "Initial Name",
      code: "Initial Code",
      description: "Initial Description",
      symptoms: ["Initial Symptom"],
    };

    const result = MedicalCondition.create(initialDto);
    medicalCondition = result.getValue();
  });

  it("should update medical condition successfully", () => {
    const dto: IMedicalConditionDto = {
      name: "Updated Name",
      code: "Initial Code",
      description: "Updated Description",
      symptoms: ["Updated Symptom 1", "Updated Symptom 2"],
    };

    const result = medicalCondition.update(dto);

    expect(result.isSuccess).to.be.true;
    expect(medicalCondition.name).to.equal(dto.name);
    expect(medicalCondition.description).to.equal(dto.description);
    expect(medicalCondition.symptoms).to.deep.equal(dto.symptoms);
  });

  it("should set symptoms to empty array if symptoms is null", () => {
    const dto: IMedicalConditionDto = {
      name: "Updated Name",
      code: "Initial Code",
      description: "Updated Description",
      symptoms: null,
    };

    const result = medicalCondition.update(dto);

    expect(result.isSuccess).to.be.true;
    expect(medicalCondition.symptoms).to.deep.equal([]);
  });

  it("should send error if name is empty", () => {
    const dto: IMedicalConditionDto = {
      name: "",
      code: "Initial Code",
      description: "Updated Description",
      symptoms: ["Symptom"],
    };

    const result = medicalCondition.update(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a name!");
  });

  it("should send error if name is null", () => {
    const dto: IMedicalConditionDto = {
      name: null,
      code: "Initial Code",
      description: "Updated Description",
      symptoms: ["Symptom"],
    };

    const result = medicalCondition.update(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a name!");
  });

  it("should send error if description is empty", () => {
    const dto: IMedicalConditionDto = {
      name: "Updated Name",
      code: "Initial Code",
      description: "",
      symptoms: ["Symptom"],
    };

    const result = medicalCondition.update(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a description!");
  });

  it("should send error if description is null", () => {
    const dto: IMedicalConditionDto = {
      name: "Updated Name",
      code: "Initial Code",
      description: null,
      symptoms: ["Symptom"],
    };

    const result = medicalCondition.update(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The medical condition must have a description!");
  });
});
