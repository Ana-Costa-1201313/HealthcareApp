import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { format } from 'date-fns';
import { DialogModule } from 'primeng/dialog';
import { Staff } from '../../../model/staff/staff.model';

@Component({
  selector: 'app-detail-staff',
  standalone: true,
  imports: [DialogModule, CommonModule],
  templateUrl: './detail-staff.component.html',
})
export class DetailStaffComponent {
  @Input() currentStaff: Staff = null;
  @Input() showDetails: boolean = false;
  @Output() showDetailsChange = new EventEmitter<boolean>();

  createAvailabilitySlot(avSlot: string): Date[] {
    const slotString: string[] = avSlot.split('/');

    const slotDate: Date[] = [];
    slotDate[0] = new Date(slotString[0]);
    slotDate[1] = new Date(slotString[1]);

    return slotDate;
  }

  showAvailabilitySlots(slots: string): string {
    const slotDate: Date[] = this.createAvailabilitySlot(slots);

    const slot1: string = format(slotDate[0], 'MM/dd/yyyy HH:mm');
    const slot2: string = format(slotDate[1], 'MM/dd/yyyy HH:mm');

    return slot1 + ' - ' + slot2;
  }
}
