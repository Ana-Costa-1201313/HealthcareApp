import sinon from "sinon";
import { Result } from "../../core/logic/Result";
import { Request, Response } from "express";
import MedicalConditionController from "../../controllers/medicalConditionController";
import IMedicalConditionController from "../../controllers/IControllers/IMedicalConditionController";
import IMedicalConditionDto from "../../dto/IMedicalConditionDto";
import AuthService from "../../services/authService";
import ExternalApiService from "../../services/externalApiService";

describe("Medical Condition Controller Test", () => {
  let service: any;
  let authService: sinon.SinonStubbedInstance<AuthService>;
  let controller: any;
  let mockPostRequest: Partial<Request>;
  let mockGetRequest: Partial<Request>;
  let mockEditRequest: Partial<Request>;
  let mockResponse: Partial<Response>;
  let statusStub: sinon.SinonStub;
  let jsonStub: sinon.SinonStub;

  beforeEach(() => {

    authService = sinon.createStubInstance(AuthService, {
      isAuthorized: sinon.stub().returns(Promise.resolve(true))
    });

    service = {
      createMedicalCondition: sinon.stub(),
      getAll: sinon.stub(),
      updateMedicalCondition: sinon.stub(),
    };

    controller = new MedicalConditionController(service, authService);

    mockPostRequest = {
      body: { name: "name", code: "code", description: "desc" },
    };

    mockGetRequest = {
      query: { name: "name", code: "code" },
    };

    mockEditRequest = {
      params: { id: "1" },
      body: { name: "name", code: "code", description: "desc" },
    };

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

  it("should create medical condition in controller", async () => {
    const dto = { name: "name", code: "code", description: "desc" };

    service.createMedicalCondition.returns(Promise.resolve(Result.ok(dto)));

    await controller.createMedicalCondition(
      mockPostRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 201);
    sinon.assert.calledWith(jsonStub, dto);
  });

  it("should throw error 400 if name is empty", async () => {
    service.createMedicalCondition.returns(
      Promise.resolve(Result.fail("Error: The medical condition must have a name!"))
    );

    await controller.createMedicalCondition(
      mockPostRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: The medical condition must have a name!",
    });
  });

  it("should throw error 400 if code is empty", async () => {
    service.createMedicalCondition.returns(
      Promise.resolve(Result.fail("Error: The medical condition must have a code!"))
    );

    await controller.createMedicalCondition(
      mockPostRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: The medical condition must have a code!",
    });
  });

  it("should throw error 400 if description is empty", async () => {
    service.createMedicalCondition.returns(
      Promise.resolve(Result.fail("Error: The medical condition must have a description!"))
    );

    await controller.createMedicalCondition(
      mockPostRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: The medical condition must have a description!",
    });
  });

  it("should get medical condition in controller", async () => {
    const dto = { name: "name", code: "code", description: "desc", symptoms: ["symptom"] };
    const dto2 = { name: "name2", code: "code2", description: "desc2", symptoms: ["symptom"] };

    const medicalConditions: IMedicalConditionDto[] = [dto, dto2];

    service.getAll.returns(Promise.resolve(Result.ok(medicalConditions)));

    await controller.getAll(
      mockGetRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 200);
    sinon.assert.calledWith(jsonStub, medicalConditions);
  });

  it("should throw error getting medical condition", async () => {
    service.getAll.returns(
      Promise.resolve(
        Result.fail("Error: This medical condition code is already in use!")
      )
    );

    await controller.getAll(
      mockGetRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: This medical condition code is already in use!",
    });
  });

  it("should update medical condition in controller", async () => {
    const dto = { name: "name", code: "code", description: "desc" };

    service.updateMedicalCondition.returns(Promise.resolve(Result.ok(dto)));

    await controller.updateMedicalCondition(
      mockEditRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 200);
    sinon.assert.calledWith(jsonStub, dto);
  });

  it("should throw error while updating allergy in controller", async () => {

    service.updateMedicalCondition.returns(Promise.resolve(Result.fail("Error: The medical condition must have a name!")));

    await controller.updateMedicalCondition(
      mockEditRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: The medical condition must have a name!",
    });
  });
});
