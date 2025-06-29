import { Component, OnInit } from '@angular/core';
import { Specialization } from '../../model/specialization.model';
import { SpecializationService } from '../../services/specialization.service';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import {
  FormGroup,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CreateSpecializationDto } from '../../model/createSpecializationDto.model';
import { FilterMatchMode, Message, SelectItem } from 'primeng/api';
import { HttpErrorResponse } from '@angular/common/http';
import { DialogModule } from 'primeng/dialog';
import { MessagesModule } from 'primeng/messages';
import { MenubarComponent } from '../menubar/menubar.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FloatLabelModule } from 'primeng/floatlabel';
import { editSpecialization } from '../../model/EditSpecialization.model';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-specialization',
  standalone: true,
  imports: [
    InputTextModule,
    ButtonModule,
    TableModule,
    FormsModule,
    ReactiveFormsModule,
    DialogModule,
    CommonModule,
    MessagesModule,
    MenubarComponent,
    InputTextareaModule,
    FloatLabelModule,
  ],
  templateUrl: './specialization.component.html',
  styleUrl: './specialization.component.css',
})
export class SpecializationComponent implements OnInit {
  specializationList: Specialization[] = [];
  showCreate = false;
  showEdit = false;
  showDetails: boolean = false;
  message: Message[] = [];
  matchModeOptions: SelectItem[] = [];
  createSpecializationForm: FormGroup;
  selectedSpecialization: Specialization | null = null;

  constructor(private service: SpecializationService, private fb: FormBuilder) {
    this.createSpecializationForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });
  }

  ngOnInit(): void {
    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS },
    ];

    this.service.getSpecializationList().subscribe((spec) => {
      this.specializationList = spec;
    });
  }

  openDetailsModal(spec: Specialization): void {
    this.selectedSpecialization = spec;
    this.showDetails = true;
  }

  openCreateModal(): void {
    this.showCreate = true;
  }

  addSpecialization(): void {
    if (this.createSpecializationForm.invalid) return;

    this.showCreate = false;

    const request: CreateSpecializationDto = {
      ...this.createSpecializationForm.value,
    };

    this.service.addSpecialization(request).subscribe({
      next: () => {
        // Success message
        this.message = [
          {
            severity: 'success',
            summary: 'Success!',
            detail: 'Your Specialization was added with success',
          },
        ];

        this.createSpecializationForm.reset();

        this.service.getSpecializationList().subscribe((spec) => {
          this.specializationList = spec;
        });
      },
      error: (error) => this.onFailure(error),
    });
  }

  openEditModal(specialization: Specialization): void {
    this.selectedSpecialization = { ...specialization };

    this.createSpecializationForm.setValue({
      name: this.selectedSpecialization.name,
      description: this.selectedSpecialization.description,
    });

    this.showEdit = true;
  }

  editSpecialization(): void {
    if (this.createSpecializationForm.invalid) return;

    const editSpecializationData: editSpecialization = {
      name: this.createSpecializationForm.value.name,
      description: this.createSpecializationForm.value.description,
    };

    this.service
      .editSpecialization(
        this.selectedSpecialization?.id,
        editSpecializationData
      )
      .subscribe(
        (updatedSpec) => {
          const index = this.specializationList.findIndex(
            (p) => p.id === updatedSpec.id
          );
          if (index !== -1) {
            this.specializationList[index] = updatedSpec;
            this.message = [
              {
                severity: 'success',
                summary: 'Success!',
                detail: `${this.createSpecializationForm.value.name}'s has been edited successfully.`,
              },
            ];
          }
          this.showEdit = false;
        },
        (error) => this.onFailure(error)
      );
  }

  onFailure(error: HttpErrorResponse): void {
    this.message = [
      {
        severity: 'error',
        summary: 'Failure!',
        detail: error.status >= 500 ? 'Server error' : error.error.message,
      },
    ];
  }
}
