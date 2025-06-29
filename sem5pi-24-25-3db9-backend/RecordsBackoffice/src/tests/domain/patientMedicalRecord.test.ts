import { expect } from "chai";
import { Result } from "../../core/logic/Result";
import { PatientMedicalRecord } from "../../domain/patientMedicalRecord";

describe("Patient Medical Record Test", () => {
    it("should create a record", () => {
        const dto = {
            patientId: "123",
            allergies: ["1","2"],
            medicalConditions: ["3"],
            familyHistory: ["diabetes"],
            freeText: ["muita tosse"], 
        };

        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue().patientId).to.equal("123");
        expect(result.getValue().allergyNames).to.deep.equal(["1","2"]);
        expect(result.getValue().medicalConditionNames).to.deep.equal(["3"]);
        expect(result.getValue().familyHistory).to.deep.equal(["diabetes"]);
        expect(result.getValue().freeText).to.deep.equal(["muita tosse"]);
    });

    it("should fail if patientId is missing", () => {
        const dto = {
          patientId: "",
          allergies: ["1"],
          medicalConditions: ["2"],
          familyHistory: ["heart disease"],
          freeText: ["no relevant information"],
        };
    
        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);
    
        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal("Patient Id is required!");
      });

      it("should add allergy successfully", () => {
        const dto = {
          patientId: "123",
          allergies: [],
          medicalConditions: ["3"],
          familyHistory: ["asthma"],
          freeText: ["needs follow-up"],
        };
    
        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);
        const record = result.getValue();
    
        const addResult = record.addAllergy("4");
    
        expect(addResult.isSuccess).to.be.true;
        expect(record.allergyNames).to.include("4");
      });

      it("should fail to add allergy if it already exists", () => {
        const dto = {
          patientId: "123",
          allergies: ["1"],
          medicalConditions: ["2"],
          familyHistory: ["diabetes"],
          freeText: ["important notes"],
        };
    
        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);
        const record = result.getValue();
    
        const addResult = record.addAllergy("1"); 
    
        expect(addResult.isFailure).to.be.true;
        expect(addResult.error).to.equal("This allergy is already in the patient medical record.");
      });
      it("should remove allergy successfully", () => {
        const dto = {
          patientId: "123",
          allergies: ["1", "2"],
          medicalConditions: ["3"],
          familyHistory: ["heart disease"],
          freeText: ["needs medication update"],
        };
    
        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);
        const record = result.getValue();
    
        const removeResult = record.removeAllergy("1");
    
        expect(removeResult.isSuccess).to.be.true;
        expect(record.allergyNames).to.not.include("1");
      });
    
      it("should fail to remove allergy if not found", () => {
        const dto = {
          patientId: "123",
          allergies: ["2"],
          medicalConditions: ["3"],
          familyHistory: ["allergic rhinitis"],
          freeText: ["no known conditions"],
        };
    
        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);
        const record = result.getValue();
    
        const removeResult = record.removeAllergy("1"); // Allergy does not exist
    
        expect(removeResult.isFailure).to.be.true;
        expect(removeResult.error).to.equal("Allergy not found in the patient medical record.");
      });
      it("should update family history", () => {
        const dto = {
          patientId: "123",
          allergies: [],
          medicalConditions: ["2"],
          familyHistory: ["cancer"],
          freeText: ["initial notes"],
        };
    
        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);
        const record = result.getValue();
    
        record.updateFamilyHistory(["stroke", "hypertension"]);
    
        expect(record.familyHistory).to.deep.equal(["stroke", "hypertension"]);
      });
    
      it("should update free text", () => {
        const dto = {
          patientId: "123",
          allergies: [],
          medicalConditions: ["3"],
          familyHistory: ["asthma"],
          freeText: ["no additional information"],
        };
    
        const result: Result<PatientMedicalRecord> = PatientMedicalRecord.create(dto);
        const record = result.getValue();
    
        record.updateFreeText(["new medical observations"]);
    
        expect(record.freeText).to.deep.equal(["new medical observations"]);
      });
})