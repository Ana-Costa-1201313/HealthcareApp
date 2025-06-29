import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentComponent } from './appointment.component';
import { AppointmentService } from '../../services/appointment.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { Appointment } from '../../model/appointment.model';
import { AppointmentDto } from '../../model/appointmentDto.model';
import { of, throwError } from 'rxjs';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { OperationType } from '../../model/operationType/operationType.model';
import { OperationRequest } from '../../model/operationRequest.model';
import { CreateAppointment } from '../../model/createAppointment.model';
import { EditAppointment } from '../../model/editAppointmentDto.model';

describe('AppointmentComponent', () => {
    let component: AppointmentComponent;
    let fixture: ComponentFixture<AppointmentComponent>;
    let service: AppointmentService;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AppointmentComponent],
            providers: [
                AppointmentService,
                provideHttpClientTesting(),
                provideHttpClient(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(AppointmentComponent);
        component = fixture.componentInstance;
        service = fixture.debugElement.injector.get(AppointmentService);
    });

    function createMockAppointments(): Appointment[] {
        return [
            {
                operationRequest: {
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
                    id: 'opReqId',
                    deadlineDate: 'deadlineDate',
                    priority: 'priority',
                    patientId: 'patientId',
                    patientName: 'patientName',
                    doctorId: 'doctorId',
                    doctorName: 'doctorName',
                    status: 'status',
                    description: 'description',
                    selectedStaff: ['staff'],
                } as OperationRequest,

                appointmentId: 'id',
                dateTime: 'dateTime',
                opRequestId: 'opReqId',
                status: 'Scheduled',
                surgeryRoomId: 'sugeryRoomId',
                surgeryRoomNumber: 'surgeyRoomNumber',
            },
            {
                operationRequest: {
                    opTypeName: {
                        id: 'id2',
                        name: 'name2',
                        anesthesiaPatientPreparationInMinutes: 10,
                        surgeryInMinutes: 10,
                        cleaningInMinutes: 20,
                        requiredStaff: [],
                        active: true,
                    } as OperationType,

                    opTypeId: 'opTypeId2',
                    id: 'opReqId2',
                    deadlineDate: 'deadlineDate2',
                    priority: 'priority2',
                    patientId: 'patientId2',
                    patientName: 'patientName2',
                    doctorId: 'doctorId2',
                    doctorName: 'doctorName2',
                    status: 'status2',
                    description: 'description2',
                    selectedStaff: ['staff2'],
                } as OperationRequest,

                appointmentId: 'id2',
                dateTime: 'dateTime2',
                opRequestId: 'opReqId2',
                status: 'Scheduled',
                surgeryRoomId: 'sugeryRoomId2',
                surgeryRoomNumber: 'surgeyRoomNumber2',
            },
        ];
    }

    it('should open details', () => {

        const appointment: Appointment = createMockAppointments()[0];

        component.openDetailsModal(appointment);

        expect(component.currentAppointment).toEqual(appointment);
    });

    it('should load Appointment list', () => {
        const mockAppointments: Appointment[] = createMockAppointments();

        spyOn(service, 'getAppointmentListWithDetails').and.returnValue(of(mockAppointments));

        component.ngOnInit();

        expect(component.appointmentList).toEqual(mockAppointments);
    });

    it('should open create modal', () => {
        component.openCreateModal();

        expect(component.showCreate).toBeTrue();
    });

    it('should add appointment', () => {
        spyOn(service, 'addAppointment').and.returnValue(of({} as any));

        const mockAppointment: CreateAppointment = {
            opRequestId: 'opReqId',
            dateTime: 'dateTime',
            surgeryRoomNumber: 'surgeryRoomNumber',
        };

        component.addAppointment();

        expect(component.showCreate).toBeFalse();
    });

    it('should send the appointment', () => {
        const mockAppointment: CreateAppointment = {
            opRequestId: 'opReqId',
            dateTime: '2023-12-29T10:00:00',
            surgeryRoomNumber: 'surgeryRoomNumber',
        };

        const returnedAppointment = {
            ...mockAppointment,
            appointmentId: 'newId'
        };

        spyOn(service, 'addAppointment').and.returnValue(of(returnedAppointment));

        component.createAppointmentForm.setValue({
            selectedOpRequest: { id: mockAppointment.opRequestId },
            opRequestId: mockAppointment.opRequestId,
            dateTime: mockAppointment.dateTime,
            surgeryRoomNumber: mockAppointment.surgeryRoomNumber,
        });

        component.addAppointment();

        expect(component.showCreate).toBeFalse();
        expect(service.addAppointment).toHaveBeenCalledWith(mockAppointment);
        expect(component.appointmentList).toContain(returnedAppointment);
        expect(component.filteredAppointmentList).toContain(returnedAppointment);
    });

    it('should open update modal', () => {
        const appointment: Appointment = createMockAppointments()[0];

        component.openUpdateModal(appointment);

        expect(component.showUpdate).toBeTrue();
    });

    it('should update appointment', () => {
        spyOn(service, 'updateAppointment').and.returnValue(of({} as any));

        const mockAppointments: Appointment[] = 
        [createMockAppointments()[0]];

        spyOn(service, 'getAppointmentListWithDetails').and.returnValue(of(mockAppointments));

        component.updateAppointment();

        expect(component.showUpdate).toBeFalse();
    });

    it('should update the appointment', () => {
        // Arrange
        const mockUpdatedAppointment: EditAppointment = {
            dateTime: '2023-12-30T14:00:00',
            surgeryRoomNumber: 'updatedSurgeryRoomNumber',
            staffList: ['staff1', 'staff2'],
        };
    
        const mockAppointmentList: Appointment[] = [
            {
                operationRequest: {
                    opTypeName: {
                        id: 'id1',
                        name: 'name1',
                        anesthesiaPatientPreparationInMinutes: 10,
                        surgeryInMinutes: 10,
                        cleaningInMinutes: 20,
                        requiredStaff: [],
                        active: true,
                    } as OperationType,
                    opTypeId: 'opTypeId1',
                    id: 'opReqId1',
                    deadlineDate: 'deadlineDate1',
                    priority: 'priority1',
                    patientId: 'patientId1',
                    patientName: 'patientName1',
                    doctorId: 'doctorId1',
                    doctorName: 'doctorName1',
                    status: 'status1',
                    description: 'description1',
                    selectedStaff: ['staff1'],
                } as OperationRequest,
                appointmentId: 'id1',
                dateTime: '2023-12-29T10:00:00',
                opRequestId: 'opReqId1',
                status: 'Scheduled',
                surgeryRoomId: 'surgeryRoomId1',
                surgeryRoomNumber: 'surgeryRoomNumber1',
            },
            // Add more mock appointments if necessary
        ];
    
        spyOn(service, 'updateAppointment').and.returnValue(of({} as any));
        spyOn(service, 'getAppointmentListWithDetails').and.returnValue(of(mockAppointmentList));
    
        component.updateAppointmentForm = new FormGroup({
            dateTime: new FormControl(mockUpdatedAppointment.dateTime, Validators.required),
            surgeryRoomNumber: new FormControl(mockUpdatedAppointment.surgeryRoomNumber, Validators.required),
            staffList: new FormControl(mockUpdatedAppointment.staffList, Validators.required),
        });
    
        component.currentAppointment = { appointmentId: 'id1' } as Appointment;
    
        // Act
        component.updateAppointment();
    
        // Assert
        expect(component.showUpdate).toBeFalse();
        expect(service.updateAppointment).toHaveBeenCalledWith('id1', mockUpdatedAppointment);
        expect(service.getAppointmentListWithDetails).toHaveBeenCalled();
        expect(component.appointmentList).toEqual(mockAppointmentList);
        expect(component.message).toEqual([{
            severity: 'success',
            summary: 'Success!',
            detail: 'Operation Request updated successfully!',
        }]);
    });
});