import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
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
import { AllergyService } from '../../../services/allergy.service';
import { CreateAllergyDto } from '../../../model/allergy/dto/createAllergyDto';

@Component({
  selector: 'app-create-allergy',
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
  templateUrl: './create-allergy.component.html',
})
export class CreateAllergyComponent {
  @Input() showCreate: boolean = false;
  @Output() showCreateChange = new EventEmitter<boolean>();
  @Output() onAdd = new EventEmitter<void>();
  @Output() onFailure = new EventEmitter<HttpErrorResponse>();

  createAllergyForm = new FormGroup({
    name: new FormControl(null, Validators.required),
    code: new FormControl(null, Validators.required),
    description: new FormControl<string | null>(null),
  });

  constructor(private service: AllergyService) {}

  addAllergy(): void {
    this.showCreateChange.emit(false);

    const request: CreateAllergyDto = {
      ...this.createAllergyForm.value,
    };

    this.service.addAllergy(request).subscribe({
      next: () => {
        this.onAdd.emit();
        this.createAllergyForm.reset();
      },
      error: (error) => {
        this.onFailure.emit(error);
      },
    });
  }
}
