import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import {
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
import { Allergy } from '../../../model/allergy/allergy.model';
import { HttpErrorResponse } from '@angular/common/http';
import { AllergyService } from '../../../services/allergy.service';
import { EditAllergyDto } from '../../../model/allergy/dto/editAllergyDto';

@Component({
  selector: 'app-edit-allergy',
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
  templateUrl: './edit-allergy.component.html',
})
export class EditAllergyComponent implements OnChanges {
  @Input() showEdit: boolean = false;
  @Input() currentAllergy: Allergy = null;
  @Output() showEditChange = new EventEmitter<boolean>();
  @Output() onEdit = new EventEmitter<void>();
  @Output() onFailure = new EventEmitter<HttpErrorResponse>();

  editAllergyForm = new FormGroup({
    name: new FormControl(null, Validators.required),
    code: new FormControl(null, Validators.required),
    description: new FormControl<string | null>(null),
  });

  constructor(private service: AllergyService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['currentAllergy']) {
      this.editAllergyForm.get('name').setValue(this.currentAllergy?.name);
      this.editAllergyForm.get('code').setValue(this.currentAllergy?.code);
      this.editAllergyForm
        .get('description')
        .setValue(this.currentAllergy?.description);
    }
  }

  editAllergy(): void {
    this.showEditChange.emit(false);

    if (this.currentAllergy?.id == null) {
      return;
    }

    const request: EditAllergyDto = {
      ...this.editAllergyForm.value,
    };

    this.service.editAllergy(this.currentAllergy.id, request).subscribe({
      next: () => {
        this.onEdit.emit();
        this.editAllergyForm.reset();
      },
      error: (error) => {
        this.onFailure.emit(error);
      },
    });
  }
}
