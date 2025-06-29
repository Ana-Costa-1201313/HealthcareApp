import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { Staff } from '../../../model/staff/staff.model';
import { StaffService } from '../../../services/staff.service';

@Component({
  selector: 'app-delete-staff',
  standalone: true,
  imports: [DialogModule, CommonModule, ButtonModule],
  templateUrl: './delete-staff.component.html',
})
export class DeleteStaffComponent {
  @Input() currentStaff: Staff = null;
  @Input() deactivate: boolean = false;
  @Output() deactivateChange = new EventEmitter<boolean>();
  @Output() onDeactivate = new EventEmitter<void>();

  constructor(private service: StaffService) {}

  deactivateStaff() {
    if (this.currentStaff?.id == null) {
      return;
    }

    this.service.deactivateStaff(this.currentStaff.id).subscribe(() => {
      this.onDeactivate.emit();
    });

    this.deactivateChange.emit(false);
  }
}
