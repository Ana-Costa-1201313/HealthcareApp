import { expect } from "chai";
import { Result } from "../../core/logic/Result";
import { Allergy } from "../../domain/allergy";

describe("Allergy Test", () => {
  it("should create allergy", () => {
    const dto = {
      name: "name1",
      code: "code1",
      description: "desc",
    };

    const result: Result<Allergy> = Allergy.create(dto);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue().name).to.equal("name1");
    expect(result.getValue().code).to.equal("code1");
    expect(result.getValue().description).to.equal("desc");
  });

  it("should send error if allergy name is empty", () => {
    const dto = {
      name: "",
      code: "code1",
      description: "desc",
    };

    const result: Result<Allergy> = Allergy.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The allergy must have a name!");
  });

  it("should send error if allergy name is null", () => {
    const dto = {
      name: null,
      code: "code1",
      description: "desc",
    };

    const result: Result<Allergy> = Allergy.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The allergy must have a name!");
  });

  it("should send error if allergy code is empty", () => {
    const dto = {
      name: "name1",
      code: "",
      description: "desc",
    };

    const result: Result<Allergy> = Allergy.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The allergy must have a code!");
  });

  it("should send error if allergy code is null", () => {
    const dto = {
      name: "name1",
      code: null,
      description: "desc",
    };

    const result: Result<Allergy> = Allergy.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The allergy must have a code!");
  });
});
