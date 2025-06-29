import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { FilterMatchMode, Message, SelectItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { TableModule } from 'primeng/table';
import { ListboxModule } from 'primeng/listbox';
import { OperationRequest } from '../../model/operationRequest.model';
import { OperationRequestService } from '../../services/operationRequest.service';
import { CreateAppointment } from '../../model/createAppointment.model';
import { AppointmentService } from '../../services/appointment.service';
import { Appointment } from '../../model/appointment.model';
import { EditAppointment } from '../../model/editAppointmentDto.model';
import { MenubarComponent } from '../menubar/menubar.component';

@Component({
  selector: 'app-appointment',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    DialogModule,
    ButtonModule,
    MessagesModule,
    MessageModule,
    FormsModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    ListboxModule,
    InputNumberModule,
    CalendarModule,
    MenubarComponent
  ],
  templateUrl: './appointment.component.html',
  styleUrl: './appointment.component.css'
})
export class AppointmentComponent implements OnInit {

  appointmentList: Appointment[] = [];
  filteredAppointmentList: Appointment[] = [];
  currentAppointment: Appointment | null = null;
  operationRequests: OperationRequest[] = [];
  selectedOpRequest: OperationRequest | null = null;
  matchModeOptions: SelectItem[] = [];
  totalRecords: number = 0;
  lazyEvent: any;
  message: Message[] = [];
  showCreate: boolean = false;
  showUpdate: boolean = false;
  showDetails: boolean = false;
  error: string | null = null;

  updateAppointmentForm: FormGroup;

  createAppointmentForm = new FormGroup({
    selectedOpRequest: new FormControl(null, Validators.required),
    opRequestId: new FormControl(null, Validators.required),
    dateTime: new FormControl(null, Validators.required),
    surgeryRoomNumber: new FormControl(null, Validators.required),
  });

  constructor(
    private service: AppointmentService,
    private operationRequestService: OperationRequestService,
    private fb : FormBuilder) {
    this.updateAppointmentForm = this.fb.group({
      dateTime: ['', Validators.required],
      surgeryRoomNumber: ['', Validators.required],
      staffList: this.fb.array([]),
    })
   }

  ngOnInit(): void {
    this.service.getAppointmentListWithDetails().subscribe((a) => {
      this.appointmentList = a.map(appointment => ({
        ...appointment,
      }));
      this.filteredAppointmentList = [...this.appointmentList];
      });
    
    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS }
    ]
  };

  fetchOperationRequests(): void {
    this.operationRequestService.getPickedOperationRequestList().subscribe(
      (data: OperationRequest[]) => {
        this.operationRequests = data;
      },
      (error) => {
        this.message = [{ severity: 'error', summary: 'Error', detail: 'Failed to load operation requests' }];
      }
    );
  }

  onOperationRequestChange(event: any): void {
    if (event.value) {
      this.createAppointmentForm.patchValue({
        opRequestId: event.value.id
      });
    }
  }

  get staffList(): FormArray {
    return this.updateAppointmentForm.get('staffList') as FormArray;
  }

  addStaff(): void {
    const staffArray = this.updateAppointmentForm.get('staffList') as FormArray;
    staffArray.push(new FormControl('', Validators.required));
  }

  removeStaff(index: number): void {
    const staffArray = this.updateAppointmentForm.get('staffList') as FormArray;
    staffArray.removeAt(index);
  }

  openCreateModal(): void {
    this.showCreate = true;
  }

  addAppointment(): void {
    this.showCreate = false;

    const request: CreateAppointment = {
      opRequestId: this.createAppointmentForm.controls.opRequestId.value,
      dateTime: this.createAppointmentForm.controls.dateTime.value,
      surgeryRoomNumber: this.createAppointmentForm.controls.surgeryRoomNumber.value,
    };

    this.service.addAppointment(request).subscribe(
      (appointment) => {
        this.appointmentList.push(appointment);
        this.filteredAppointmentList = [...this.appointmentList];
      },
      (error: HttpErrorResponse) => {
        this.message = [
          { severity: 'error', summary: 'Error', detail: error.message },
        ];
      }
    );
  }

  openDetailsModal(appointment: Appointment): void {
    this.currentAppointment = appointment;
    this.showDetails = true;
  }
  
  openUpdateModal(appointment: Appointment): void {
    this.currentAppointment = appointment;
  
    // Clear existing controls in staffList
    const staffArray = this.updateAppointmentForm.get('staffList') as FormArray;
    staffArray.clear();
  
    // Populate the staffList FormArray with the existing staff
    appointment.operationRequest.selectedStaff.forEach(staff => {
      staffArray.push(new FormControl(staff, Validators.required));
    });
  
    // Populate other form controls
    this.updateAppointmentForm.patchValue({
      dateTime: appointment.dateTime,
      surgeryRoomNumber: appointment.surgeryRoomNumber,
    });
  
    this.showUpdate = true;
  }

  updateAppointment(): void {
    if(this.updateAppointmentForm.invalid) return;

    this.showUpdate = false;

    const updatedData: EditAppointment = {
      dateTime: this.updateAppointmentForm.value.dateTime,
      surgeryRoomNumber: this.updateAppointmentForm.value.surgeryRoomNumber.toString(),
      staffList: this.updateAppointmentForm.value.staffList,
    };

    this.service.updateAppointment(this.currentAppointment.appointmentId, updatedData).subscribe({
      next: () => {
        this.message = [
          {
            severity: 'success',
            summary: 'Success!',
            detail: 'Operation Request updated successfully!',
          },
        ];
  
        this.service.getAppointmentListWithDetails().subscribe((a) => {
          this.appointmentList = a.map(appointment => ({
            ...appointment,
          }));
        });
      },
      error: (error) => this.onFailure(error),
    });
  }

  onFailure(error: HttpErrorResponse): void {
    if (error.status >= 500) {
      this.message = [
        { severity: 'error', summary: 'Failure!', detail: 'Server error' },
      ];
    } else {
      this.message = [
        { severity: 'error', summary: 'Failure!', detail: error.error.message },
      ];
    }
  }
}