import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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
import { Specialization } from '../../model/specialization.model';
import { Staff } from '../../model/staff/staff.model';
import { SpecializationService } from '../../services/specialization.service';
import { StaffService } from '../../services/staff.service';
import { CreateStaffComponent } from './create-staff/create-staff.component';
import { DeleteStaffComponent } from './delete-staff/delete-staff.component';
import { DetailStaffComponent } from './detail-staff/detail-staff.component';
import { EditStaffComponent } from './edit-staff/edit-staff.component';
import { MenubarComponent } from '../menubar/menubar.component';

@Component({
  selector: 'app-staff',
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
    InputNumberModule,
    CalendarModule,
    DetailStaffComponent,
    DeleteStaffComponent,
    CreateStaffComponent,
    EditStaffComponent,
    MenubarComponent
  ],
  templateUrl: './staff.component.html',
  styleUrl: './staff.component.css',
})
export class StaffComponent implements OnInit {
  matchModeOptions: SelectItem[] = [];
  totalRecords: number = 0;
  lazyEvent: any;
  message: Message[] = [];
  staffList: Staff[] = [];
  currentStaff: Staff = null;
  specializations: Specialization[] = [];
  specializationsNames: string[] = [];
  showCreate: boolean = false;
  showDetails: boolean = false;
  deactivate: boolean = false;
  showEdit: boolean = false;

  constructor(
    private service: StaffService,
    private specService: SpecializationService
  ) {}

  ngOnInit(): void {
    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS },
    ];

    this.service.getTotalRecords().subscribe((t) => (this.totalRecords = t));

    this.specService.getSpecializationList().subscribe((s) => {
      this.specializations = s;

      const names: string[] = [];

      this.specializations.forEach((spec) => names.push(spec.name));

      this.specializationsNames = names;
    });
  }

  loadStaffLazy(event: any) {
    this.lazyEvent = event;

    this.service
      .getStaffList(
        event.filters?.name?.value,
        event.filters?.email?.value,
        event.filters?.specialization?.value,
        event.first / event.rows + 1,
        event.rows
      )
      .subscribe((s: Staff[]) => (this.staffList = s));
  }

  openCreateModal(): void {
    this.showCreate = true;
  }

  openDetailsModal(staff: Staff): void {
    this.currentStaff = staff;
    this.showDetails = true;
  }

  openEditModal(staff: Staff): void {
    this.currentStaff = null;
    this.currentStaff = staff;
    this.showEdit = true;
  }

  openDeactivateModal(staff: Staff) {
    this.currentStaff = staff;
    this.deactivate = true;
  }

  onAdd() {
    this.loadStaffLazy(this.lazyEvent);

    this.message = [
      {
        severity: 'success',
        summary: 'Success!',
        detail: 'Your Staff Profile was added with success',
      },
    ];
  }

  onEdit() {
    this.loadStaffLazy(this.lazyEvent);

    this.message = [
      {
        severity: 'success',
        summary: 'Success!',
        detail: 'Your Staff Profile was edited with success',
      },
    ];
  }

  onDeactivate() {
    this.loadStaffLazy(this.lazyEvent);

    this.message = [
      {
        severity: 'success',
        summary: 'Success!',
        detail: 'Your Staff Profile was deactivated with success',
      },
    ];
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
