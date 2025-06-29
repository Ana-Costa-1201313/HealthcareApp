import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Message } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MessagesModule } from 'primeng/messages';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TableModule } from 'primeng/table';
import { RoomService } from '../../services/room.service';
import { Room } from '../../model/room.model';
import { OperationRequestService } from '../../services/operationRequest.service';
import { OperationRequest } from '../../model/operationRequest.model';
import { PlanningDTO } from '../../model/planningDto.model';
import { PlanningService } from '../../services/planning.service';
import { AppointmentDto } from '../../model/appointmentDto.model';
import { AppointmentService } from '../../services/appointment.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MenubarComponent } from '../menubar/menubar.component';

@Component({
  selector: 'app-planning',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    DialogModule,
    FormsModule,
    ButtonModule,
    MessagesModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    CalendarModule,
    SelectButtonModule,
    ListboxModule,
    ProgressSpinnerModule,
    MenubarComponent
  ],
  templateUrl: './planning.component.html',
  styleUrls: ['./planning.component.css'],
})
export class PlanningComponent implements OnInit {


  constructor(
    private roomService: RoomService,
    private opReqService: OperationRequestService,
    private planningService: PlanningService,
    private appointmentService: AppointmentService
  ) { }


  rooms: Room[] = [];
  roomNumbers: string[] = [];
  message: Message[] = [];
  opRequest: OperationRequest[] = [];
  filteredOpRequest: OperationRequest[] = [];
  selectedOpRequests: OperationRequest[] = [];


  selectedRoomNumber: string = '';
  date: Date | undefined;

  //dialog box
  isDialogVisible = false;
  loading = true;
  responseData: any;
  errorMessage: string | null = null;
  responseDataArray: AppointmentDto[] = [];



  stateOptions: any[] = [
    { label: 'Best Planning', planType: 'best' },
    { label: 'Last Surgery Time Planning', planType: 'heuristic1' },
    { label: 'Occupied Time Planning', planType: 'heuristic2' },
  ];

  planType: string = 'off';

  ngOnInit() {

    this.roomService.getRoomList().subscribe((room) => {
      this.rooms = room.map(r => ({
        ...r
      }));

      console.log(this.rooms);

      const numbers: string[] = []

      this.rooms.forEach((roomN) => numbers.push(roomN.roomNumber));
      this.roomNumbers = numbers;

      this.rooms.forEach((room, index) => {

        if (room.maintenanceSlots) {
          const duration = this.calculateMaintenanceDifference(room.maintenanceSlots);
          console.log(`Maintenance duration for Room ${index + 1}: ${duration} minutes`);

          const a = this.calculateRoomAvailability(room.maintenanceSlots);
          console.log(a);

        }
      });

    });

    this.message = [
      {
        severity: 'info',
        summary: 'Selection Updated',
        detail: `Current total time is 0% of the room's available time.`,
      },
    ];
  }

  onRoomNumberChange() {
    if (this.selectedRoomNumber && this.date) {
      this.getOperationTypeList();
    }
  }

  onDateChange() {
    if (this.selectedRoomNumber && this.date) {
      this.getOperationTypeList();
    }
  }

  getOperationTypeList() {
    if (!this.selectedRoomNumber || !this.date) {
      return;
    }

    this.opReqService.getPickedOperationRequestList().subscribe((opReq) => {
      this.opRequest = opReq;

      const filteredRequests = this.opRequest.filter((req) => {
        const deadline = new Date(req.deadlineDate);

        return this.isSameDay(deadline, this.date);
      });

      console.log(filteredRequests);
      this.filteredOpRequest = filteredRequests;
    });
  }

  isSameDay(date1: Date, date2: Date): boolean {
    const d1 = new Date(date1.getFullYear(), date1.getMonth(), date1.getDate());
    const d2 = new Date(date2.getFullYear(), date2.getMonth(), date2.getDate());

    return d1.getTime() === d2.getTime();
  }

  calculateMaintenanceDifference(maintenanceSlots: string[]): number {
    if (!maintenanceSlots || maintenanceSlots.length === 0) {
      return 0;
    }

    const slotString = maintenanceSlots[0];

    if (!slotString || typeof slotString !== 'string' || slotString.trim() === '') {
      return 0;
    }

    const [start, end] = slotString.split(' - ');

    const startDate = new Date(
      start.replace(/(\d{2})\/(\d{2})\/(\d{4})/, '$2/$1/$3')
    );
    const endDate = new Date(
      end.replace(/(\d{2})\/(\d{2})\/(\d{4})/, '$2/$1/$3')
    );

    if (isNaN(startDate.getTime()) || isNaN(endDate.getTime())) {
      console.error(`Invalid maintenance slot format: ${slotString}`);
      return 0;
    }

    const duration = (endDate.getTime() - startDate.getTime()) / (1000 * 60);
    return duration;
  }

  calculateRoomAvailability(maintenanceSlots: string[]): number {
    const maintenanceDuration = this.calculateMaintenanceDifference(maintenanceSlots);

    const availableTime = 1440 - maintenanceDuration;
    return availableTime;
  }

  calculateTotalSelectedTime(selectedRequests: OperationRequest[]): number {
    return selectedRequests.reduce((total, request) => {
      return total + ((request.opTypeName.anesthesiaPatientPreparationInMinutes + request.opTypeName.surgeryInMinutes + request.opTypeName.cleaningInMinutes) || 0);
    }, 0);
  }

  canSelectMore(operationDuration: number): boolean {
    const totalSelectedTime = this.calculateTotalSelectedTime(this.selectedOpRequests);
    const availableTime = this.calculateRoomAvailability(this.rooms.find(room => room.roomNumber === this.selectedRoomNumber)?.maintenanceSlots || []);
    const allowedTime = availableTime * 0.8;
    return (totalSelectedTime + operationDuration) <= allowedTime;
  }

  handleSelectionChange(event: any) {
    const selectedRequests: OperationRequest[] = event.value;

    const availableTime = this.calculateRoomAvailability(
      this.rooms.find(room => room.roomNumber === this.selectedRoomNumber)?.maintenanceSlots || []
    );

    const allowedTime = availableTime * 0.8;

    const totalSelectedTime = this.calculateTotalSelectedTime(selectedRequests);

    const percentageUsed = Math.floor((totalSelectedTime / availableTime) * 100);

    if (totalSelectedTime > allowedTime) {
      this.message = [
        {
          severity: 'warn',
          summary: '80% Limit Exceeded',
          detail: `Current total time is ${percentageUsed}% of the room's available time.`,
        },
      ];

      event.value = this.selectedOpRequests;
    } else {
      this.selectedOpRequests = [...selectedRequests];
      this.message = [
        {
          severity: 'info',
          summary: 'Selection Updated',
          detail: `Current total time is ${percentageUsed}% of the room's available time.`,
        },
      ];
    }
  }



  get selectedRoom(): Room | undefined {
    return this.rooms.find(room => room.roomNumber === this.selectedRoomNumber);
  }

  isSubmitDisabled(): boolean {
    const selectedRoom = this.selectedRoom;
    const isRoomNumberSelected = !!this.selectedRoomNumber;
    const isDateSelected = !!this.date;
    const isOpRequestSelected = this.selectedOpRequests.length > 0;
    const isValueSelected = !!this.planType;
    const isValueSelected2 = this.planType !== 'off';


    if (!isRoomNumberSelected || !isDateSelected || !isOpRequestSelected || !isValueSelected || !isValueSelected2) {
      return true;
    }

    if (!selectedRoom || !selectedRoom.maintenanceSlots) {
      return true;
    }

    const availableTime = this.calculateRoomAvailability(selectedRoom.maintenanceSlots);
    const allowedTime = availableTime * 0.8;
    const totalSelectedTime = this.calculateTotalSelectedTime(this.selectedOpRequests || []);

    return totalSelectedTime > allowedTime;
  }

  submitForm() {

       const planningData = new PlanningDTO(
         this.planType,
         this.selectedRoomNumber.toString(),
         this.date,
         this.selectedOpRequests
       );

    this.isDialogVisible = true;

    //REQUEST TEM DE SER MUDADO PARA 
    this.planningService.postPlanning(planningData).subscribe(
    //this.appointmentService.getAppointmentList().subscribe(
      (response: AppointmentDto[]) => {
        this.loading = false;
        this.responseDataArray = response;
      },
      (error) => {
        this.loading = false;
        this.errorMessage = error.message;
      }
    );
  }

  onClose() {
    this.isDialogVisible = false;

    window.location.reload();
  }

}
