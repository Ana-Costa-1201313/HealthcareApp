import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationrequestComponent } from './operationrequest.component';

import { OperationRequestService } from '../../services/operationRequest.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { OperationRequest } from '../../model/operationRequest.model';
import { of } from 'rxjs';
import { OperationType } from '../../model/operationType/operationType.model';

import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';


describe('OperationrequestComponent', () => {
  let component: OperationrequestComponent;
  let fixture: ComponentFixture<OperationrequestComponent>;
  let service: OperationRequestService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperationrequestComponent],
      providers: [
        OperationRequestService,
        provideHttpClientTesting(),
        provideHttpClient(),
      ],
    })
      .compileComponents();

    fixture = TestBed.createComponent(OperationrequestComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(OperationRequestService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should open create modal', () => {
    component.openCreateModal();

    expect(component.showCreate).toBeTrue();
  });

  it('should open update modal', () => {

    const opReq: OperationRequest = {
      opTypeName: {
        id: 'id',
        name: 'name',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 10,
        cleaningInMinutes: 20,
        requiredStaff: [],
        active: true,
      } as OperationType,

      opTypeId: 'opTypeId',
      id: 'id',
      deadlineDate: 'deadlineDate',
      priority: 'priority',
      patientId: 'patientId',
      patientName: 'patientName',
      doctorId: 'doctorId',
      doctorName: 'doctorName',
      status: 'status',
      description: 'description',
      selectedStaff: [],
    };

    component.openUpdateModal(opReq);

    expect(component.showUpdate).toBeTrue();
  });

  it('should close deactivate modal', () => {
    const opReq = { id: '1', name: 'Test Operation' } as any;
    component.currentOpRequest = opReq;

    spyOn(service, 'deactivateOperationRequest').and.returnValue(of({} as any));

    const mockOperationReqList: OperationRequest[] = [
      {
        opTypeName: {
          id: 'id',
          name: 'name',
          anesthesiaPatientPreparationInMinutes: 10,
          surgeryInMinutes: 10,
          cleaningInMinutes: 20,
          requiredStaff: [],
          active: true,
        } as OperationType,

        opTypeId: 'opTypeId',
        id: 'id',
        deadlineDate: 'deadlineDate',
        priority: 'priority',
        patientId: 'patientId',
        patientName: 'patientName',
        doctorId: 'doctorId',
        doctorName: 'doctorName',
        status: 'status',
        description: 'description',
        selectedStaff: [],
      }
    ];

  spyOn(service, 'getOperationRequestList').and.returnValue(of(mockOperationReqList));

  component.deactivateOperationRequest();

  expect(component.deactivate).toBeFalse();
});
});