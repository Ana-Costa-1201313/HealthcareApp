import { expect } from "chai";
import sinon from "sinon";
import { AllergyMap } from "../../mappers/AllergyMap";
import AllergyService from "../../services/allergyService";
import { Result } from "../../core/logic/Result";
import { Allergy } from "../../domain/allergy";
import { UniqueEntityID } from "../../core/domain/UniqueEntityID";

describe("Allergy Service Test", () => {
  let repo: any;
  let service: any;
  let domainStub: sinon.SinonStub;
  let dtoStub: sinon.SinonStub;
  let dtoListStub: sinon.SinonStub;

  beforeEach(() => {
    domainStub = sinon.stub(AllergyMap, "toDomain").returns({
      name: "name",
      code: "code",
      description: "desc",
    });

    dtoStub = sinon.stub(AllergyMap, "toDto").returns({
      name: "name",
      code: "code",
      description: "desc",
    });

    dtoListStub = sinon.stub(AllergyMap, "toDtoList").returns(
      {
        name: "name",
        code: "code",
        description: "desc",
      },
      { name: "name2", code: "code2", description: "desc2" }
    );

    repo = {
      save: sinon.stub(),
      get: sinon.stub(),
      findAllergyById: sinon.stub(),
      update: sinon.stub(),
    };
    service = new AllergyService(repo);
  });

  afterEach(() => {
    sinon.restore();
  });

  it("should create allergy in service", async () => {
    const dto = { name: "name", code: "code", description: "desc" };
    const allergy = Allergy.create(dto).getValue();

    repo.save.returns(Promise.resolve(Result.ok(allergy)));

    const result = await service.createAllergy(dto);

    sinon.assert.calledOnce(domainStub);
    sinon.assert.calledOnce(dtoStub);
    sinon.assert.calledOnce(repo.save);

    expect(result.isSuccess).to.be.true;
  });

  it("should throw error if name is empty", async () => {
    const dto = { name: "", code: "code", description: "desc" };

    domainStub.returns(Result.fail("Name cannot be empty"));

    repo.save.resolves(Result.fail("Name cannot be empty"));

    const result = await service.createAllergy(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Name cannot be empty");
  });

  it("should throw error if code is empty", async () => {
    const dto = { name: "name", code: "", description: "desc" };

    domainStub.returns(Result.fail("Code cannot be empty"));

    repo.save.resolves(Result.fail("Code cannot be empty"));

    const result = await service.createAllergy(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Code cannot be empty");
  });

  it("should throw error if name is null", async () => {
    const dto = { name: null, code: "code", description: "desc" };

    domainStub.returns(Result.fail("Name cannot be empty"));

    repo.save.resolves(Result.fail("Name cannot be empty"));

    const result = await service.createAllergy(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Name cannot be empty");
  });

  it("should throw error if code is null", async () => {
    const dto = { name: "name", code: null, description: "desc" };

    domainStub.returns(Result.fail("Code cannot be empty"));

    repo.save.resolves(Result.fail("Code cannot be empty"));

    const result = await service.createAllergy(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Code cannot be empty");
  });

  it("should throw error if name already exists", async () => {
    const dto = { name: "name", code: "code", description: "desc" };

    repo.save.returns(
      Promise.resolve(
        Result.fail("Error: This allergy name is already in use!")
      )
    );

    const result = await service.createAllergy(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal(
      "Error: This allergy name is already in use!"
    );
  });

  it("should throw error if code already exists", async () => {
    const dto = { name: "name", code: "code", description: "desc" };

    repo.save.returns(
      Promise.resolve(
        Result.fail("Error: This allergy code is already in use!")
      )
    );

    const result = await service.createAllergy(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal(
      "Error: This allergy code is already in use!"
    );
  });

  it("should get allergies", async () => {
    const dto1 = { name: "name", code: "code", description: "desc" };
    const dto2 = { name: "name2", code: "code2", description: "desc2" };

    const allergy1 = Allergy.create(dto1).getValue();
    const allergy2 = Allergy.create(dto2).getValue();

    const allergies: Allergy[] = [allergy1, allergy2];

    repo.get.resolves(Result.ok(allergies));

    const result = await service.getAllergy();

    sinon.assert.calledOnce(repo.get);
    sinon.assert.calledOnce(dtoListStub);

    expect(result.isSuccess).to.be.true;
  });

  it("should throw error getting allergies", async () => {
    repo.get.resolves(Result.fail("Error: Can't get allergies!"));

    const result = await service.getAllergy();

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: Can't get allergies!");
  });

  it("should edit allergy in service", async () => {
    const dto = { name: "name", code: "code", description: "desc" };
    const newDto = { name: "name2", code: "code2", description: "desc" };
    const allergy = Allergy.create(dto, new UniqueEntityID("1")).getValue();
    const newAllergy = Allergy.create(newDto, new UniqueEntityID("1")).getValue();

    repo.findAllergyById.returns(Promise.resolve(Result.ok(allergy)));
    
    repo.update.returns(Promise.resolve(Result.ok(newAllergy)));

    dtoStub.returns(newDto);

    const result = await service.editAllergy("1", newDto);

    sinon.assert.calledOnce(dtoStub);
    sinon.assert.calledOnce(repo.update);
    sinon.assert.calledOnce(repo.findAllergyById);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue().name).to.equal("name2");
    expect(result.getValue().code).to.equal("code2");
    expect(result.getValue().description).to.equal("desc");
  });

  it("should throw error editing allergy in service", async () => {
    const dto = { name: "name", code: "code", description: "desc" };
    const newDto = { name: "", code: "code2", description: "desc" };
    const allergy = Allergy.create(dto, new UniqueEntityID("1")).getValue();

    repo.findAllergyById.returns(Promise.resolve(Result.ok(allergy)));
    
    dtoStub.returns(newDto);

    const result = await service.editAllergy("1", newDto);

    sinon.assert.calledOnce(repo.findAllergyById);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal("Error: The allergy must have a name!");
  });
});
