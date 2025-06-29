import sinon from "sinon";
import { Result } from "../../core/logic/Result";
import AllergyController from "../../controllers/allergyController";
import { Request, response, Response } from "express";
import { request } from "http";
import IAllergyDto from "../../dto/IAllergyDto";
import { UniqueEntityID } from "../../core/domain/UniqueEntityID";
import { Allergy } from "../../domain/allergy";
import AuthService from "../../services/authService";
import ExternalApiService from "../../services/externalApiService";

describe("Allergy Controller Test", () => {
  let service: any;
  let authService: sinon.SinonStubbedInstance<AuthService>;
  let controller: any;
  let mockRequest: Partial<Request>;
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
      createAllergy: sinon.stub(),
      getAllergy: sinon.stub(),
      editAllergy: sinon.stub(),
    };

    controller = new AllergyController(service, authService);

    mockRequest = {
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

  it("should create allergy in controller", async () => {
    const dto = { name: "name", code: "code", description: "desc" };

    service.createAllergy.returns(Promise.resolve(Result.ok(dto)));

    await controller.createAllergy(
      mockRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 201);
    sinon.assert.calledWith(jsonStub, dto);
  });

  it("should throw error 400 if name is empty", async () => {
    service.createAllergy.returns(
      Promise.resolve(Result.fail("Error: The allergy must have a name!"))
    );

    await controller.createAllergy(
      mockRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: The allergy must have a name!",
    });
  });

  it("should throw error 400 if code is empty", async () => {
    service.createAllergy.returns(
      Promise.resolve(Result.fail("Error: The allergy must have a code!"))
    );

    await controller.createAllergy(
      mockRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: The allergy must have a code!",
    });
  });

  it("should get allergy in controller", async () => {
    const dto = { name: "name", code: "code", description: "desc" };
    const dto2 = { name: "name2", code: "code2", description: "desc2" };

    const allergies: IAllergyDto[] = [dto, dto2];

    service.getAllergy.returns(Promise.resolve(Result.ok(allergies)));

    await controller.getAllergy(
      mockGetRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 200);
    sinon.assert.calledWith(jsonStub, allergies);
  });

  it("should throw error getting allergy", async () => {
    service.getAllergy.returns(
      Promise.resolve(
        Result.fail("Error: This allergy code is already in use!")
      )
    );

    await controller.getAllergy(
      mockGetRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: This allergy code is already in use!",
    });
  });

  it("should edit allergy in controller", async () => {
    const dto = { name: "name", code: "code", description: "desc" };

    service.editAllergy.returns(Promise.resolve(Result.ok(dto)));

    await controller.editAllergy(
      mockEditRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 200);
    sinon.assert.calledWith(jsonStub, dto);
  });

  it("should throw error editing allergy in controller", async () => {
    const dto = { name: "", code: "code", description: "desc" };

    service.editAllergy.returns(Promise.resolve(Result.fail("Error: The allergy must have a name!")));

    await controller.editAllergy(
      mockEditRequest as Request,
      mockResponse as Response
    );

    sinon.assert.calledWith(statusStub, 400);
    sinon.assert.calledWith(jsonStub, {
      message: "Error: The allergy must have a name!",
    });
  });
});
