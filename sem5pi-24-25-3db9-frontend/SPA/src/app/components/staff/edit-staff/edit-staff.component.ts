import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
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
import { EditStaffDto } from '../../../model/staff/editStaffDto';
import { Staff } from '../../../model/staff/staff.model';
import { StaffService } from '../../../services/staff.service';

@Component({
  selector: 'app-edit-staff',
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
  templateUrl: './edit-staff.component.html',
})
export class EditStaffComponent implements OnChanges {
  @Input() showEdit: boolean = false;
  @Input() currentStaff: Staff = null;
  @Input() specializationsNames: string[] = [];
  @Output() showEditChange = new EventEmitter<boolean>();
  @Output() onEdit = new EventEmitter<void>();
  @Output() onFailure = new EventEmitter<HttpErrorResponse>();

  editStaffForm = new FormGroup({
    phone: new FormControl(null, Validators.required),
    specialization: new FormControl<string | null>(null),
    availabilitySlots: new FormArray([new FormControl<Date[]>(null)]),
  });

  constructor(private service: StaffService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['currentStaff']) {
      this.editStaffForm.get('phone').setValue(this.currentStaff?.phone);

      this.editStaffForm
        .get('specialization')
        .setValue(this.currentStaff?.specialization);

      this.editStaffForm.controls.availabilitySlots.clear();

      if (this.currentStaff?.availabilitySlots.length != 0) {
        this.currentStaff?.availabilitySlots.forEach((slot, index) => {
          this.addAvailabilitySlotToEdit();

          this.editStaffForm.controls.availabilitySlots
            .at(index)
            .setValue(this.createAvailabilitySlot(slot));
        });
      } else {
        this.addAvailabilitySlotToEdit();
      }
    }
  }

  editStaff(): void {
    this.showEditChange.emit(false);

    if (this.currentStaff?.id == null) {
      return;
    }

    const availabilitySlotsIso: string[] = [];

    this.editStaffForm.get('availabilitySlots').value.forEach((slot) => {
      if (slot != null) {
        availabilitySlotsIso.push(
          slot[0].toISOString() + '/' + slot[1].toISOString()
        );
      }
    });

    const request: EditStaffDto = {
      ...this.editStaffForm.value,

      id: this.currentStaff.id,

      phone: this.editStaffForm.get('phone').value.toString(),

      availabilitySlots: availabilitySlotsIso,
    };

    this.service.editStaff(this.currentStaff.id, request).subscribe({
      next: () => {
        this.onEdit.emit();
        this.editStaffForm.reset();
        this.editStaffForm.controls.availabilitySlots.clear();
        this.addAvailabilitySlotToEdit();
      },
      error: (error) => {
        this.onFailure.emit(error);
      },
    });
  }

  addAvailabilitySlotToEdit(): void {
    const availabilitySlot = new FormControl<Date[]>(null);

    this.editStaffForm.controls.availabilitySlots.push(availabilitySlot);
  }

  createAvailabilitySlot(avSlot: string): Date[] {
    const slotString: string[] = avSlot.split('/');

    const slotDate: Date[] = [];
    slotDate[0] = new Date(slotString[0]);
    slotDate[1] = new Date(slotString[1]);

    return slotDate;
  }
}
