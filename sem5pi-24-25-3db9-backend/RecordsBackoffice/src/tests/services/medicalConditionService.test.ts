import { expect } from "chai";
import sinon from "sinon";
import { AllergyMap } from "../../mappers/AllergyMap";
import AllergyService from "../../services/allergyService";
import { Result } from "../../core/logic/Result";
import { Allergy } from "../../domain/allergy";
import { MedicalConditionMap } from "../../mappers/MedicalConditionMap";
import MedicalConditionService from "../../services/medicalConditionService";
import { MedicalCondition } from "../../domain/medicalCondition/medicalCondition";
import { update } from "lodash";
import { UniqueEntityID } from "../../core/domain/UniqueEntityID";

describe("Medical Condition Service Test", () => {
    let repo: any;
    let service: any;
    let resultDomainStub: sinon.SinonStub;
    let dtoStub: sinon.SinonStub;
    let dtoListStub: sinon.SinonStub;

    beforeEach(() => {
        resultDomainStub = sinon
            .stub(MedicalConditionMap, "toResultDomain")
            .returns(Result.ok({} as MedicalConditionMap));
        dtoStub = sinon.stub(MedicalConditionMap, "toDto").returns({
            name: "name",
            code: "code",
            description: "desc",
            symptoms: []
        });

        dtoListStub = sinon.stub(MedicalConditionMap, "toDtoList").returns(
            {
                name: "name",
                code: "code",
                description: "desc",
                symptoms: ["symptom"]
            },
            {
                name: "name2",
                code: "code2",
                description: "desc2",
                symptoms: ["symptom"]
            }
        );

        repo = { save: sinon.stub(), getAll: sinon.stub(), getById: sinon.stub(), update: sinon.stub() };
        service = new MedicalConditionService(repo);
    });

    afterEach(() => {
        sinon.restore();
    });

    it("should create medical condition in service", async () => {
        const dto = { name: "name", code: "code", description: "desc", symptoms: [] };
        const medicalCondition = MedicalCondition.create(dto).getValue();

        repo.save.returns(Promise.resolve(Result.ok(medicalCondition)));

        const result = await service.createMedicalCondition(dto);

        sinon.assert.calledOnce(resultDomainStub);
        sinon.assert.calledOnce(dtoStub);
        sinon.assert.calledOnce(repo.save);

        expect(result.isSuccess).to.be.true;
    });

    it("should throw error if name is empty", async () => {
        const dto = { name: "", code: "code", description: "desc", symptoms: [] };

        resultDomainStub.returns(Result.fail("Name cannot be empty"));

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Name cannot be empty");
    });

    it("should throw error if code is empty", async () => {
        const dto = { name: "name", code: "", description: "desc", symptoms: [] };

        resultDomainStub.returns(Result.fail("Code cannot be empty"));

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Code cannot be empty");
    });

    it("should throw error if description is empty", async () => {
        const dto = { name: "", code: "code", description: "desc", symptoms: [] };

        resultDomainStub.returns(Result.fail("Description cannot be empty"));

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Description cannot be empty");
    });

    it("should throw error if name is null", async () => {
        const dto = { name: null, code: "code", description: "desc", symptoms: [] };

        resultDomainStub.returns(Result.fail("Name cannot be empty"));

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Name cannot be empty");
    });

    it("should throw error if code is null", async () => {
        const dto = { name: "name", code: null, description: "desc", symptoms: [] };

        resultDomainStub.returns(Result.fail("Code cannot be empty"));

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Code cannot be empty");
    });

    it("should throw error if description is null", async () => {
        const dto = { name: null, code: "code", description: "desc", symptoms: [] };

        resultDomainStub.returns(Result.fail("Description cannot be empty"));

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Description cannot be empty");
    });

    it("should throw error if name already exists", async () => {
        const dto = { name: "name", code: "code", description: "desc", symptoms: [] };

        repo.save.returns(
            Promise.resolve(
                Result.fail("Error: This medical condition name is already in use!")
            )
        );

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal(
            "Error: This medical condition name is already in use!"
        );
    });

    it("should throw error if code already exists", async () => {
        const dto = { name: "name", code: "code", description: "desc", symptoms: [] };

        repo.save.returns(
            Promise.resolve(
                Result.fail("Error: This medical condition code is already in use!")
            )
        );

        const result = await service.createMedicalCondition(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal(
            "Error: This medical condition code is already in use!"
        );
    });

    it("should get medical conditions", async () => {
        const dto1 = { name: "name", code: "code", description: "desc", symptoms: ["symptom"] };
        const dto2 = { name: "name2", code: "code2", description: "desc2", symptoms: ["symptom"] };

        const medicalCondition1 = MedicalCondition.create(dto1).getValue();
        const medicalCondition2 = MedicalCondition.create(dto2).getValue();

        const medicalConditions: MedicalCondition[] = [medicalCondition1, medicalCondition2];

        repo.getAll.resolves(Result.ok(medicalConditions));

        const result = await service.getAll();

        sinon.assert.calledOnce(repo.getAll);
        sinon.assert.calledOnce(dtoListStub);

        expect(result.isSuccess).to.be.true;
    });

    it("should throw error getting medical conditions", async () => {
        repo.getAll.resolves(Result.fail("Error: Can't get medical conditions!"));

        const result = await service.getAll();

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Error: Can't get medical conditions!");
    });

    it("should update medical condition in service", async () => {
        const dto = { name: "name", code: "code", description: "desc", symptoms: [] };
        const newDto = { name: "name2", code: "code2", description: "desc", symptoms: ["fever"] };
        const medicalCondition = MedicalCondition.create(dto, new UniqueEntityID("1")).getValue();
        const newMedicalCondition = MedicalCondition.create(newDto, new UniqueEntityID("1")).getValue();

        repo.getById.returns(Promise.resolve(Result.ok(medicalCondition)));

        repo.update.returns(Promise.resolve(Result.ok(newMedicalCondition)));

        dtoStub.returns(newDto);

        const result = await service.updateMedicalCondition("1", newDto);

        sinon.assert.calledOnce(dtoStub);
        sinon.assert.calledOnce(repo.update);
        sinon.assert.calledOnce(repo.getById);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue().name).to.equal("name2");
        expect(result.getValue().code).to.equal("code2");
        expect(result.getValue().description).to.equal("desc");
        expect(result.getValue().symptoms.length).to.equal(1);
        expect(result.getValue().symptoms[0]).to.equal("fever");
    });

    it("should throw error updating medical condition with empty name in service", async () => {
        const dto = { name: "name", code: "code", description: "desc", symptoms: [] };
        const newDto = { name: "", code: "code2", description: "desc", symptoms: ["fever"] };
        const medicalCondition = MedicalCondition.create(dto, new UniqueEntityID("1")).getValue();

        repo.getById.returns(Promise.resolve(Result.ok(medicalCondition)));

        dtoStub.returns(newDto);

        const result = await service.updateMedicalCondition("1", newDto);

        sinon.assert.calledOnce(repo.getById);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Error: The medical condition must have a name!");
    });

    it("should throw error updating medical condition with empty description in service", async () => {
        const dto = { name: "name", code: "code", description: "desc", symptoms: [] };
        const newDto = { name: "name2", code: "code2", description: "", symptoms: [] };
        const medicalCondition = MedicalCondition.create(dto, new UniqueEntityID("1")).getValue();

        repo.getById.returns(Promise.resolve(Result.ok(medicalCondition)));

        dtoStub.returns(newDto);

        const result = await service.updateMedicalCondition("1", newDto);

        sinon.assert.calledOnce(repo.getById);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Error: The medical condition must have a description!");
    });
});
