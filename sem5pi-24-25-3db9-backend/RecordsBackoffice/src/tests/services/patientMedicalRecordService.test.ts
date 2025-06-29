import { expect } from "chai";
import sinon from "sinon";
import { PatientMedicalRecordMap } from "../../mappers/PatientMedicalRecordMap";
import PatientMedicalRecordService from "../../services/patientMedicalRecordService";
import IPatientMedicalRecordDto from "../../dto/IPatientMedicalRecordDto";
import { PatientMedicalRecord } from "../../domain/patientMedicalRecord";
import { Result } from "../../core/logic/Result";
import { UniqueEntityID } from "../../core/domain/UniqueEntityID";

describe("PatientMedicalRecordService Tests", () => {
    let repo: any;
    let service: any;
    let domainStub: sinon.SinonStub 
    let dtoStub: sinon.SinonStub
    let dtoListStub: sinon.SinonStub;

    beforeEach(() => {
        domainStub = sinon.stub(PatientMedicalRecordMap, "toDomain").returns({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"]
        });

        dtoStub = sinon.stub(PatientMedicalRecordMap, "toDto").returns({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"]
        });

        dtoListStub = sinon.stub(PatientMedicalRecordMap, "toDtoList").returns([
            {
              patientId: "patientId1",
              allergies: ["allergy1"],
              medicalConditions: ["condition1"],
              familyHistory: ["history1"],
              freeText: ["note1"]
            },
            {
              patientId: "patientId2",
              allergies: ["allergy2"],
              medicalConditions: ["condition2"],
              familyHistory: ["history2"],
              freeText: ["note2"]
            }
          ]);
        
          repo = {
            save: sinon.stub(),
            getById: sinon.stub(),
            getAll: sinon.stub(),
            updateAllergies: sinon.stub(),
            updateMedicalConditions: sinon.stub(),
            getByPatientId: sinon.stub(),
          };

          service = new PatientMedicalRecordService(repo);
    });

    afterEach(() => {
        sinon.restore();
    });

    it("should create a patient medical record", async () => {
        const dto: IPatientMedicalRecordDto = {
          patientId: "patientId",
          allergies: ["allergy1"],
          medicalConditions: ["condition1"],
          familyHistory: ["history1"],
          freeText: ["note1"],
        };
    
        const record = PatientMedicalRecord.create(dto).getValue();

        repo.getByPatientId.returns(Promise.resolve(Result.fail("Record does not exist")));
       
        repo.save.returns(Promise.resolve(Result.ok(record)));
    
        const result = await service.createMedicalRecord(dto);
    
        sinon.assert.calledOnce(repo.save);
        expect(result.isSuccess).to.be.true;
        expect(result.getValue().patientId).to.equal("patientId");
      });

      it("should fail when creating a patient medical record", async () => {
        const dto: IPatientMedicalRecordDto = {
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"]
        };
    
        domainStub.returns(null); 
    
        const result = await service.createMedicalRecord(dto);
    
        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Error: Can't create a patient medical record");
    });

    it("should fail when creating a patient medical record with invalid data", async () => {
        const dto: IPatientMedicalRecordDto = {
            patientId: "", 
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"]
        };
    
        domainStub.returns(null);  
    
        const result = await service.createMedicalRecord(dto);
    
        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Error: Can't create a patient medical record");
    });

    it("should get all patient medical records", async () => {
        const records = [
            PatientMedicalRecord.create({
                patientId: "patientId1",
                allergies: ["allergy1"],
                medicalConditions: ["condition1"],
                familyHistory: ["history1"],
                freeText: ["note1"]
            }).getValue(),
            PatientMedicalRecord.create({
                patientId: "patientId2",
                allergies: ["allergy2"],
                medicalConditions: ["condition2"],
                familyHistory: ["history2"],
                freeText: ["note2"]
            }).getValue()
        ];
    
        repo.getAll.returns(Promise.resolve(Result.ok(records)));
    
        const result = await service.getAllMedicalRecords();
    
        sinon.assert.calledOnce(repo.getAll);
        expect(result.isSuccess).to.be.true;
        expect(result.getValue().length).to.equal(2);
        expect(result.getValue()[0].patientId).to.equal("patientId1");
        expect(result.getValue()[1].patientId).to.equal("patientId2");
    });
    
    it("should return error if no patient medical records found", async () => {
        
        repo.getAll.returns(Promise.resolve(Result.fail("No medical records found")));
    
        const result = await service.getAllMedicalRecords();
    
        sinon.assert.calledOnce(repo.getAll);
        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("No medical records found");
    });

    it("should fail to update allergies when record does not exist", async () => {
        const patientId = "invalidPatientId";
        const newAllergies = ["allergy2", "allergy3"];

        repo.getById.returns(Promise.resolve(Result.fail("Patient medical record not found")));

        const result = await service.updateMedicalRecordAllergies(patientId, newAllergies);

        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Patient medical record not found");
    });
    
    it("should get medical conditions for a patient", async () => {
        const patientId = "patientId";

        const record = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1", "condition2"],
            familyHistory: ["history1"],
            freeText: ["note1"]
        }).getValue();

        repo.getById.returns(Promise.resolve(Result.ok(record)));

        const result = await service.getMedicalRecordMedicalConditions(patientId);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.deep.equal(["condition1", "condition2"]);
    });
   
    it("should fail to get medical conditions when record does not exist", async () => {
        const patientId = "invalidPatientId";

        repo.getById.returns(Promise.resolve(Result.fail("Patient medical record not found")));

        const result = await service.getMedicalRecordMedicalConditions(patientId);

        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Patient medical record not found");
    });

    it("should get family history for a patient", async () => {
        const patientId = "patientId";

        const record = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1", "history2"],
            freeText: ["note1"]
        }).getValue();

        repo.getById.returns(Promise.resolve(Result.ok(record)));

        const result = await service.getMedicalRecordFamilyHistory(patientId);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.deep.equal(["history1", "history2"]);
    });

    it("should fail to get family history when record does not exist", async () => {
        const patientId = "invalidPatientId";

        repo.getById.returns(Promise.resolve(Result.fail("Patient medical record not found")));

        const result = await service.getMedicalRecordFamilyHistory(patientId);

        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Patient medical record not found");
    }
    );

    it("should update family history for a patient medical record", async () => {
        const patientId = "patientId";
        const initialRecord = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1", "history2"],
            freeText: ["note1"]
        }, new UniqueEntityID("1")).getValue();

        const updatedRecord = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"]
        }, new UniqueEntityID("1")).getValue();

        repo.getById = sinon.stub().withArgs(patientId).returns(Promise.resolve(Result.ok(initialRecord)));

        repo.updateFamilyHistory = sinon.stub()
        .withArgs(patientId, sinon.match.array.deepEquals(["history1"]))
        .returns(Promise.resolve(Result.ok(updatedRecord)));

        const newFamilyHistory = ["history1"];
        const result = await service.updateMedicalRecordFamilyHistory(patientId, newFamilyHistory);

        sinon.assert.calledOnce(repo.getById);
        sinon.assert.calledWith(repo.updateFamilyHistory, patientId, newFamilyHistory);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue().familyHistory).to.deep.equal(newFamilyHistory);
    });

    it("should get free text for a patient", async () => {
        const patientId = "patientId";

        const record = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1", "note2"]
        }).getValue();

        repo.getById.returns(Promise.resolve(Result.ok(record)));

        const result = await service.getMedicalRecordFreeText(patientId);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.deep.equal(["note1", "note2"]);
    });

    it("should fail to get free text when record does not exist", async () => {
        const patientId = "invalidPatientId";

        repo.getById.returns(Promise.resolve(Result.fail("Patient medical record not found")));

        const result = await service.getMedicalRecordFreeText(patientId);

        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Patient medical record not found");
    });

    it("should update free text for a patient medical record", async () => {
        const patientId = "patientId";
        const initialRecord = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1", "note2"],
        }, new UniqueEntityID("1")).getValue();
    
        const updatedRecord = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"],
        }, new UniqueEntityID("1")).getValue();
    
        repo.getById = sinon.stub().withArgs(patientId).returns(Promise.resolve(Result.ok(initialRecord)));

        repo.updateFreeText = sinon.stub()
            .withArgs(patientId, sinon.match.array.deepEquals(["note1"]))
            .returns(Promise.resolve(Result.ok(updatedRecord)));
    
        const newFreeText = ["note1"];
        const result = await service.updateMedicalRecordFreeText(patientId, newFreeText);
    
        sinon.assert.calledOnce(repo.getById);
        sinon.assert.calledWith(repo.updateFreeText, patientId, newFreeText);
    
        expect(result.isSuccess).to.be.true;
        expect(result.getValue().freeText).to.deep.equal(newFreeText);
    });

    it("should update allergies for a patient medical record", async () => {
        const patientId = "patientId";
        const initialRecord = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1", "allergy2"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"],
        },new UniqueEntityID("1")).getValue();
    
        const updatedRecord = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1"],
            medicalConditions: ["condition1"],
            familyHistory: ["history1"],
            freeText: ["note1"],
        },new UniqueEntityID("1")).getValue();
    
        repo.getById.returns(Promise.resolve(Result.ok(initialRecord)));
    
        
        repo.updateAllergies.returns(Promise.resolve(Result.ok(updatedRecord)));
    
        const newAllergies = ["allergy1"];
        const result = await service.updateMedicalRecordAllergies(patientId, newAllergies);
    
      
        sinon.assert.calledOnce(repo.getById);
        sinon.assert.calledWith(repo.updateAllergies, patientId, newAllergies);
    
        expect(result.isSuccess).to.be.true;
        expect(result.getValue().allergies).to.deep.equal(newAllergies);
    });
    
    it("should get allergies for a patient", async () => {
        const patientId = "patientId";
    
        const record = PatientMedicalRecord.create({
            patientId: "patientId",
            allergies: ["allergy1", "allergy2"],
            medicalConditions: ["condition1", "condition2"],
            familyHistory: ["history1"],
            freeText: ["note1"]
        }).getValue();
    
        repo.getById.returns(Promise.resolve(Result.ok(record)));
    
        const result = await service.getMedicalRecordAllergies(patientId);
    
        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.deep.equal(["allergy1", "allergy2"]);
    });

    it("should fail to get allergies when record does not exist", async () => {
        const patientId = "invalidPatientId";
    
        repo.getById.returns(Promise.resolve(Result.fail("Patient medical record not found")));
    
        const result = await service.getMedicalRecordAllergies(patientId);
    
        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Patient medical record not found");
    });
})
