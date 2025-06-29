import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationtypeComponent } from './operationtype.component';
import { OperationTypeService } from '../../services/operationType.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { OperationType } from '../../model/operationType/operationType.model';
import { of, throwError } from 'rxjs';
import { SpecializationService } from '../../services/specialization.service';
import { Specialization } from '../../model/specialization.model';
import { FormArray, FormControl, FormGroup } from '@angular/forms';

describe('OperationtypeComponent', () => {
  let component: OperationtypeComponent;
  let fixture: ComponentFixture<OperationtypeComponent>;
  let service: OperationTypeService;
  let specService: SpecializationService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperationtypeComponent],
      providers: [
        OperationTypeService,
        SpecializationService,
        provideHttpClientTesting(),
        provideHttpClient(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(OperationtypeComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(OperationTypeService);
    specService = fixture.debugElement.injector.get(SpecializationService);
  });

  it('should open details: currentOpType is Equal', () => {

    const opType: OperationType = {
      id: 'id',
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 10,
      surgeryInMinutes: 10,
      cleaningInMinutes: 20,
      requiredStaff: [],
      active: true,
    };

    component.openDetailsModal(opType);

    expect(component.currentOpType).toEqual(opType);
  });

  it('should open details: showDetails is True', () => {

    const opType: OperationType = {
      id: 'id',
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 10,
      surgeryInMinutes: 10,
      cleaningInMinutes: 20,
      requiredStaff: [],
      active: true,
    };

    component.openDetailsModal(opType);

    expect(component.showDetails).toBe(true);
  });

  it('should populate specializations', () => {
    const mockOperationTypes: OperationType[] = [
      {
        id: '1',
        name: 'Surgery A',
        anesthesiaPatientPreparationInMinutes: 30,
        surgeryInMinutes: 60,
        cleaningInMinutes: 15,
        requiredStaff: [{ specialization: 'Nurse', total: 2 }],
        active: true,
      }
    ];

    const mockSpecializations: Specialization[] = [
      { name: 'Nurse', active: true },
      { name: 'Surgeon', active: true }
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypes));
    spyOn(specService, 'getSpecializationList').and.returnValue(of(mockSpecializations));

    component.ngOnInit();

    expect(component.specializations).toEqual(mockSpecializations);
  });

  it('should populate specializationsNames', () => {
    const mockOperationTypes: OperationType[] = [
      {
        id: '1',
        name: 'Surgery A',
        anesthesiaPatientPreparationInMinutes: 30,
        surgeryInMinutes: 60,
        cleaningInMinutes: 15,
        requiredStaff: [{ specialization: 'Nurse', total: 2 }],
        active: true,
      }
    ];

    const mockSpecializations: Specialization[] = [
      { name: 'Nurse', active: true },
      { name: 'Surgeon', active: true }
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypes));
    spyOn(specService, 'getSpecializationList').and.returnValue(of(mockSpecializations));

    component.ngOnInit();

    expect(component.specializationsNames).toEqual(['Nurse', 'Surgeon']);
  });

  it('should populate operationTypeList', () => {
    const mockOperationTypes: OperationType[] = [
      {
        id: '1',
        name: 'Surgery A',
        anesthesiaPatientPreparationInMinutes: 30,
        surgeryInMinutes: 60,
        cleaningInMinutes: 15,
        requiredStaff: [{ specialization: 'Nurse', total: 2 }],
        active: true,
      }
    ];

    const mockSpecializations: Specialization[] = [
      { name: 'Nurse', active: true },
      { name: 'Surgeon', active: true }
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypes));
    spyOn(specService, 'getSpecializationList').and.returnValue(of(mockSpecializations));

    component.ngOnInit();

    expect(component.operationTypeList).toEqual([
      {
        ...mockOperationTypes[0],
        specialization: 'Nurse',
      }
    ]);
  });

  it('should populate filteredOperationTypeList', () => {
    const mockOperationTypes: OperationType[] = [
      {
        id: '1',
        name: 'Surgery A',
        anesthesiaPatientPreparationInMinutes: 30,
        surgeryInMinutes: 60,
        cleaningInMinutes: 15,
        requiredStaff: [{ specialization: 'Nurse', total: 2 }],
        active: true,
      }
    ];

    const mockSpecializations: Specialization[] = [
      { name: 'Nurse', active: true },
      { name: 'Surgeon', active: true }
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypes));
    spyOn(specService, 'getSpecializationList').and.returnValue(of(mockSpecializations));

    component.ngOnInit();

    expect(component.filteredOperationTypeList).toEqual(component.operationTypeList);
  });

  it('should open deactivation confirmation: currentOpType is Equal', () => {
    const opType: OperationType = {
      id: 'id',
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 10,
      surgeryInMinutes: 10,
      cleaningInMinutes: 20,
      requiredStaff: [],
      active: true,
    };

    component.openDeactivateModal(opType);

    expect(component.currentOpType).toEqual(opType);
  });

  it('should open deactivation confirmation: deactivate is True', () => {
    const opType: OperationType = {
      id: 'id',
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 10,
      surgeryInMinutes: 10,
      cleaningInMinutes: 20,
      requiredStaff: [],
      active: true,
    };

    component.openDeactivateModal(opType);

    expect(component.deactivate).toBeTrue();
  });

  it('should close deactivate modal', () => {
    const opType = { id: '1', name: 'Test Operation' } as any;
    component.currentOpType = opType;

    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();

    expect(component.deactivate).toBeFalse();
  });

  it('should deactivate operation type: success message is Equal', () => {
    const opType = { id: '1', name: 'Test Operation' } as any;
    component.currentOpType = opType;

    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();


    const expectedMessage = [
      {
        severity: 'info',
        summary: 'Success!',
        detail: 'The Operation Type "Test Operation" was deactivated with success',
      },
    ];
    expect(component.message).toEqual(expectedMessage);
  });

  it('should deactivate operation type: populate operationTypeList', () => {
    const opType = { id: '1', name: 'Test Operation' } as any;
    component.currentOpType = opType;

    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();

    expect(component.operationTypeList).toEqual([
      {
        ...mockOperationTypeList[0],
        specialization: 'Surgeon',
      },
    ]);
  });

  it('should deactivate operation type: populate filteredOperationTypeList', () => {
    const opType = { id: '1', name: 'Test Operation' } as any;
    component.currentOpType = opType;

    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();

    expect(component.filteredOperationTypeList).toEqual(component.operationTypeList);
  });

  it('should deactivate operation type: deactivateOperationType to have been called with opType id', () => {
    const opType = { id: '1', name: 'Test Operation' } as any;
    component.currentOpType = opType;

    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();

    expect(service.deactivateOperationType).toHaveBeenCalledWith(opType.id);
  });

  it('should deactivate operation type: getOperationTypeList to have been called 1 time', () => {
    const opType = { id: '1', name: 'Test Operation' } as any;
    component.currentOpType = opType;

    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();

    expect(service.getOperationTypeList).toHaveBeenCalledTimes(1);
  });

  it('should not deactivate operation type: deactivateOperationType to have been called 0 times', () => {
    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));
    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();

    expect(service.deactivateOperationType).toHaveBeenCalledTimes(0);
  });

  it('should not deactivate operation type: getOperationTypeList to have been called 0 times', () => {
    spyOn(service, 'deactivateOperationType').and.returnValue(of({} as any));
    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.deactivateOperationType();

    expect(service.getOperationTypeList).toHaveBeenCalledTimes(0);
  });

  it('should open create modal', () => {
    component.openCreateModal();

    expect(component.showCreate).toBeTrue();
  });

  it('should add operation type', () => {
    spyOn(service, 'addOperationType').and.returnValue(of({} as any));
    spyOn(component, 'addRequiredStaff');

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.createOperationTypeForm
      .get('requiredStaff');

    component.addOperationType();

    expect(component.showCreate).toBeFalse();
  });

  it('should send operation type', () => {
    spyOn(service, 'addOperationType').and.returnValue(of({} as any));
    spyOn(component, 'addRequiredStaff').and.callThrough();

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.createOperationTypeForm.setValue({
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 20,
      surgeryInMinutes: 30,
      cleaningInMinutes: 10,
      requiredStaff: []
    });

    component.addRequiredStaff();
    component.requiredStaff.at(0).setValue({
      specialization: 'spec',
      total: 2
    });

    const expectedRequest = {
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 20,
      surgeryInMinutes: 30,
      cleaningInMinutes: 10,
      requiredStaff: [
        {
          specialization: 'spec',
          total: 2
        }
      ]
    };

    component.addOperationType();

    expect(service.addOperationType).toHaveBeenCalledOnceWith(expectedRequest);
  });

  it('should add required staff', () => {
    component.addRequiredStaff();

    expect(component.requiredStaff.length).toBe(1);
  });

  it('should send error 500', () => {
    const error: HttpErrorResponse = { status: 500 } as any;

    component.onFailure(error);

    expect(component.message).toEqual([
      { severity: 'error', summary: 'Failure!', detail: 'Server error' },
    ]);
  });

  it('should send error 400', () => {
    const error: HttpErrorResponse = { status: 400, error: { message: 'abc' } } as any;

    component.onFailure(error);

    expect(component.message).toEqual([
      { severity: 'error', summary: 'Failure!', detail: 'abc' },
    ]);
  });

  it('should send error adding operation type', () => {
    spyOn(service, 'addOperationType').and.returnValue(
      throwError(() => ({
        status: 400,
        error: { message: 'abc' },
      }))
    );

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.createOperationTypeForm.setValue({
      name: 'Test Operation',
      anesthesiaPatientPreparationInMinutes: 20,
      surgeryInMinutes: 30,
      cleaningInMinutes: 10,
      requiredStaff: []
    });

    component.addRequiredStaff();
    component.requiredStaff.at(0).setValue({
      specialization: 'spec',
      total: 2,
    });

    component.addOperationType();

    expect(component.message).toEqual([
      { severity: 'error', summary: 'Failure!', detail: 'abc' },
    ]);
  });

  it('should remove required staff at specified index and be length 2', () => {
    component.addRequiredStaff();
    component.addRequiredStaff();
    component.addRequiredStaff();

    component.removeRequiredStaff(0);
    expect(component.requiredStaff.length).toBe(2);
  });

  it('should remove required staff at specified index and be length 1', () => {
    component.addRequiredStaff();
    component.addRequiredStaff();
    component.addRequiredStaff();

    component.removeRequiredStaff(0);
    component.removeRequiredStaff(0);

    expect(component.requiredStaff.length).toBe(1);
  });

  it('should open update modal', () => {

    const opType: OperationType = {
      id: 'id',
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 10,
      surgeryInMinutes: 10,
      cleaningInMinutes: 20,
      requiredStaff: [],
      active: true,
    };

    component.openUpdateModal(opType);

    expect(component.showUpdate).toBeTrue();
  });

  it('should add a required staff to updateRequiredStaff array: size equal to 0', () => {
    expect(component.updateRequiredStaff.length).toBe(0);
  });

  it('should add a required staff to updateRequiredStaff array: size equal to 1', () => {


    component.addUpdateRequiredStaff();

    expect(component.updateRequiredStaff.length).toBe(1);
  });

  it('should add a required staff to updateRequiredStaff array: specialization of the element equal to', () => {

    component.addUpdateRequiredStaff();

    expect(component.updateRequiredStaff.at(0).get('specialization')?.value).toBe('');
  });

  it('should add a required staff to updateRequiredStaff array: total of the element equal to 1', () => {
    component.addUpdateRequiredStaff();

    expect(component.updateRequiredStaff.at(0).get('total')?.value).toBe(1);
  });

  it('should update operation type', () => {
    spyOn(service, 'updateOperationType').and.returnValue(of({} as any));
    spyOn(component, 'addUpdateRequiredStaff');

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.updateOperationTypeForm
      .get('requiredStaff');

    component.updateOperationType();

    expect(component.showUpdate).toBeFalse();
  });


  it('should send updated operation type: size of required Staff array equal to 1', () => {
    spyOn(service, 'updateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.updateOperationTypeForm.setValue({
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 20,
      surgeryInMinutes: 30,
      cleaningInMinutes: 10,
      requiredStaff: []
    });

    const requiredStaffArray = component.updateOperationTypeForm.get('requiredStaff') as FormArray;
    requiredStaffArray.push(
      new FormGroup({
        specialization: new FormControl('spec'),
        total: new FormControl(2)
      })
    );

    expect(requiredStaffArray.length).toBe(1);
  });

  it('should send updated operation type: updateOperationType been called with', () => {
    spyOn(service, 'updateOperationType').and.returnValue(of({} as any));

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));

    component.updateOperationTypeForm.setValue({
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 20,
      surgeryInMinutes: 30,
      cleaningInMinutes: 10,
      requiredStaff: []
    });

    const requiredStaffArray = component.updateOperationTypeForm.get('requiredStaff') as FormArray;
    requiredStaffArray.push(
      new FormGroup({
        specialization: new FormControl('spec'),
        total: new FormControl(2)
      })
    );

    const expectedRequest = {
      name: 'name',
      anesthesiaPatientPreparationInMinutes: 20,
      surgeryInMinutes: 30,
      cleaningInMinutes: 10,
      requiredStaff: [
        {
          specialization: 'spec',
          total: 2
        }
      ]
    };

    component.updateOperationType();

    expect(service.updateOperationType).toHaveBeenCalledWith(expectedRequest);
  });

  it('should send error updating operation type: erro message equal to', () => {
    spyOn(service, 'updateOperationType').and.returnValue(
      throwError(() => ({
        status: 400,
        error: { message: 'abc' },
      }))
    );

    const mockOperationTypeList: OperationType[] = [
      {
        id: '1',
        name: 'Updated Operation',
        anesthesiaPatientPreparationInMinutes: 10,
        surgeryInMinutes: 20,
        cleaningInMinutes: 30,
        requiredStaff: [{ specialization: 'Surgeon', total: 1 }],
        active: false,
      },
    ];

    spyOn(service, 'getOperationTypeList').and.returnValue(of(mockOperationTypeList));


    component.updateOperationTypeForm.setValue({
      name: 'Test Operation',
      anesthesiaPatientPreparationInMinutes: 20,
      surgeryInMinutes: 30,
      cleaningInMinutes: 10,
      requiredStaff: []
    });

    component.addUpdateRequiredStaff();

    const requiredStaffArray = component.updateOperationTypeForm.get('requiredStaff') as FormArray;

    requiredStaffArray.at(0).setValue({
      specialization: 'spec',
      total: 2,
    });

    component.updateOperationType();

    expect(component.message).toEqual([
      { severity: 'error', summary: 'Failure!', detail: 'abc' },
    ]);
  });

});
