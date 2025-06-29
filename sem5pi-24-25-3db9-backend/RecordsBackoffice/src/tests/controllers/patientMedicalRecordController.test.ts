import sinon from "sinon";
import { Request, Response } from "express";
import PatientMedicalRecordController from "../../controllers/patientMedicalRecordController";
import IPatientMedicalRecordDto from "../../dto/IPatientMedicalRecordDto";
import { Result } from "../../core/logic/Result";
import { update } from "lodash";

describe("PatientMedicalRecordController Tests", () => {
  let service: any;
  let controller: any;
  let mockRequest: Partial<Request>;
  let mockResponse: Partial<Response>;
  let statusStub: sinon.SinonStub;
  let jsonStub: sinon.SinonStub;

    beforeEach(() => {
        service = {
            createMedicalRecord: sinon.stub(),
            getAllMedicalRecords: sinon.stub(),
            updateMedicalRecordAllergies: sinon.stub(),
            updateMedicalRecordMedicalConditions: sinon.stub(),
            updateMedicalRecordFamilyHistory: sinon.stub(),
            updateMedicalRecordFreeText: sinon.stub(),
            getMedicalRecordMedicalConditions: sinon.stub(),
            getMedicalRecordAllergies: sinon.stub(),
        };

        controller = new PatientMedicalRecordController(service);

        jsonStub = sinon.stub();
        statusStub = sinon.stub().returns({ json: jsonStub });

      mockResponse = {
        status: statusStub,
        json: jsonStub,
      } as Partial<Response>;
  });

    afterEach(() => {
        sinon.restore();
      });
      it("should create a medical record successfully", async () => {
        const recordDto: IPatientMedicalRecordDto = {
          patientId: "patient-id-123",
          allergies: ["peanut", "shellfish"],
          medicalConditions: ["asthma", "diabetes"],
          familyHistory: ["history1"],
          freeText: ["Patient is allergic to peanuts"],
        };
    
        mockRequest = {
          body: recordDto,
        };
    
        service.createMedicalRecord.resolves(Result.ok(recordDto));
    
        await controller.createMedicalRecord(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 201);
        sinon.assert.calledWith(jsonStub, recordDto);
      });
     
      it("should return 400 when createMedicalRecord fails", async () => {
        const errorMessage = "Error: Invalid medical record data";
        mockRequest = {
          body: {},
        };
    
        service.createMedicalRecord.resolves(Result.fail(errorMessage));
    
        await controller.createMedicalRecord(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 400);
        sinon.assert.calledWith(jsonStub, { message: errorMessage });
      });

      it("should get all medical records successfully", async () => {
        const records = [
          { patientId: "1", allergies: ["peanut"], medicalConditions: ["asthma"] },
          { patientId: "2", allergies: ["shellfish"], medicalConditions: ["diabetes"] },
        ];
    
        service.getAllMedicalRecords.resolves(Result.ok(records));
    
        await controller.getAllMedicalRecords(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 200);
        sinon.assert.calledWith(jsonStub, records);
      });

      it("should return 400 when getAllMedicalRecords fails", async () => {
        const errorMessage = "Error: Cannot retrieve records";
        service.getAllMedicalRecords.resolves(Result.fail(errorMessage));
    
        await controller.getAllMedicalRecords(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 400);
        sinon.assert.calledWith(jsonStub, { message: errorMessage });
      });

      it("should update medical record allergies successfully", async () => {
        const id = "patient-id-123";
        const allergies = ["dust", "pollen"];
        const updatedRecord = { id, allergies };
    
        mockRequest = {
          params: { id },
          body: { allergies },
        };
    
        service.updateMedicalRecordAllergies.resolves(Result.ok(updatedRecord));
    
        await controller.updateMedicalRecordAllergies(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 200);
        sinon.assert.calledWith(jsonStub, updatedRecord);
      });

      it("should return 400 when updateMedicalRecordAllergies fails", async () => {
        const id = "patient-id-123";
        const allergies = ["dust", "pollen"];
        const errorMessage = "Error: Cannot update allergies";
    
        mockRequest = {
          params: { id },
          body: { allergies },
        };
    
        service.updateMedicalRecordAllergies.resolves(Result.fail(errorMessage));
    
        await controller.updateMedicalRecordAllergies(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 400);
        sinon.assert.calledWith(jsonStub, { message: errorMessage });
      });

      it("should get medical conditions successfully", async () => {
        const id = "patient-id-123";
        const medicalConditions = ["asthma", "diabetes"];
    
        mockRequest = {
          params: { id },
        };
    
        service.getMedicalRecordMedicalConditions.resolves(Result.ok(medicalConditions));
    
        await controller.getMedicalRecordMedicalConditions(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 200);
        sinon.assert.calledWith(jsonStub, medicalConditions);
      });

      it("should return 400 when getMedicalRecordMedicalConditions fails", async () => {
        const id = "patient-id-123";
        const errorMessage = "Error: Cannot retrieve medical conditions";
    
        mockRequest = {
          params: { id },
        };
    
        service.getMedicalRecordMedicalConditions.resolves(Result.fail(errorMessage));
    
        await controller.getMedicalRecordMedicalConditions(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 400);
        sinon.assert.calledWith(jsonStub, { message: errorMessage });
      });

      it("should update medical record free text successfully", async () => {
        const id = "patient-id-123";
        const freeText = ["Patient is allergic to peanuts"];
        const updatedRecord = { id, freeText };
    
        mockRequest = {
          params: { id },
          body: { freeText },
        };
    
        service.updateMedicalRecordFreeText.resolves(Result.ok(updatedRecord));
    
        await controller.updateMedicalRecordFreeText(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 200);
        sinon.assert.calledWith(jsonStub, updatedRecord);
      });

      it("should return 400 when updateMedicalRecordFreeText fails", async () => {
        const id = "patient-id-123";
        const freeText = ["Patient is allergic to peanuts"];
        const errorMessage = "Error: Cannot update free text";
    
        mockRequest = {
          params: { id },
          body: { freeText },
        };
    
        service.updateMedicalRecordFreeText.resolves(Result.fail(errorMessage));
    
        await controller.updateMedicalRecordFreeText(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 400);
        sinon.assert.calledWith(jsonStub, { message: errorMessage });
      });

      it("should update medical record family history successfully", async () => {
        const id = "patient-id-123";
        const familyHistory = ["Diabetes"];
        const updatedRecord = { id, familyHistory };
    
        mockRequest = {
          params: { id },
          body: { familyHistory },
        };
    
        service.updateMedicalRecordFamilyHistory.resolves(Result.ok(updatedRecord));
    
        await controller.updateMedicalRecordFamilyHistory(mockRequest as Request, mockResponse as Response);
    
        sinon.assert.calledWith(statusStub, 200);
        sinon.assert.calledWith(jsonStub, updatedRecord);
      });

      it("should return 400 when updateMedicalRecordFamilyHistory fails", async () => {
        const id = "patient-id-123";
        const familyHistory = ["Diabetes"];
    
        mockRequest = {
          params: { id },
          body: { familyHistory },
        };
    
        service.updateMedicalRecordFamilyHistory.resolves(Result.fail("Error: Can't update family history for this medical record!"));
    
        await controller.updateMedicalRecordFamilyHistory(mockRequest as Request, mockResponse as Response);

        sinon.assert.calledWith(statusStub, 400);
        sinon.assert.calledWith(jsonStub, { message: "Error: Can't update family history for this medical record!" });
      });
});