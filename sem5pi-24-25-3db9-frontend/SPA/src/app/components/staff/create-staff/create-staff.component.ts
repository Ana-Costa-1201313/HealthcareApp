import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  FormArray,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { Role } from '../../../model/role.model';
import { CreateStaffDto } from '../../../model/staff/dto/createStaffDto';
import { StaffService } from '../../../services/staff.service';

@Component({
  selector: 'app-create-staff',
  standalone: true,
  imports: [
    CommonModule,
    DialogModule,
    ButtonModule,
    MessageModule,
    FormsModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    CalendarModule,
  ],
  templateUrl: './create-staff.component.html',
})
export class CreateStaffComponent {
  @Input() showCreate: boolean = false;
  @Input() specializationsNames: string[] = [];
  @Output() showCreateChange = new EventEmitter<boolean>();
  @Output() onAdd = new EventEmitter<void>();
  @Output() onFailure = new EventEmitter<HttpErrorResponse>();
  roles: Role[] = [Role.Doctor, Role.Nurse, Role.Technician];

  createStaffForm = new FormGroup({
    name: new FormControl(null, Validators.required),
    licenseNumber: new FormControl(null, Validators.required),
    phone: new FormControl(null, Validators.required),
    specialization: new FormControl<string | null>(null),
    availabilitySlots: new FormArray([new FormControl<Date[]>(null)]),
    role: new FormControl<Role | null>(null, Validators.required),
    recruitmentYear: new FormControl(null, Validators.required),
  });

  constructor(private service: StaffService) {}

  addStaff(): void {
    this.showCreateChange.emit(false);

    const availabilitySlotsIso: string[] = [];

    this.createStaffForm.get('availabilitySlots').value.forEach((slot) => {
      if (slot != null) {
        availabilitySlotsIso.push(
          slot[0].toISOString() + '/' + slot[1].toISOString()
        );
      }
    });

    const request: CreateStaffDto = {
      ...this.createStaffForm.value,

      phone: this.createStaffForm.get('phone').value.toString(),

      availabilitySlots: availabilitySlotsIso,

      recruitmentYear: new Date(
        this.createStaffForm.get('recruitmentYear').value
      ).getFullYear(),
    };

    this.service.addStaff(request).subscribe({
      next: () => {
        this.onAdd.emit();
        this.createStaffForm.reset();
        this.createStaffForm.controls.availabilitySlots.clear();
        this.addSlot();
      },
      error: (error) => {
        this.onFailure.emit(error);
      },
    });
  }

  addSlot(): void {
    const availabilitySlot = new FormControl<Date[]>(null);

    this.createStaffForm.controls.availabilitySlots.push(availabilitySlot);
  }
}
