import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { MessagesModule } from 'primeng/messages';
import { TableModule } from 'primeng/table';
import { MenubarComponent } from '../menubar/menubar.component';
import { FilterMatchMode, Message, SelectItem } from 'primeng/api';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FloatLabelModule } from 'primeng/floatlabel';
import { MedicalConditionDto } from '../../model/medicalCondition/medicalConditionDto.model';
import { MedicalConditionService } from '../../services/medicalCondition.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MedicalCondition } from '../../model/medicalCondition/medicalCondition.model';

@Component({
  selector: 'app-medicalcondition',
  standalone: true,
  imports: [
    CommonModule, TableModule, DialogModule, FormsModule, ButtonModule,
    MessagesModule, ReactiveFormsModule, DropdownModule, InputTextModule,
    InputNumberModule, MenubarComponent, InputTextareaModule, FloatLabelModule
  ],
  templateUrl: './medicalcondition.component.html',
  styleUrl: './medicalcondition.component.css'
})
export class MedicalconditionComponent implements OnInit {
  medicalConditionList: MedicalCondition[] = [];
  filteredMedicalConditionList: MedicalCondition[] = [];
  currentMedicalCondition: MedicalCondition | null = null;
  showDetails = false;
  showUpdate = false;
  showCreate = false;
  matchModeOptions: SelectItem[] = [];
  message: Message[] = [];
  role: string | null = null;



  createMedicalConditionForm: FormGroup;
  updateMedicalConditionForm: FormGroup;

  constructor(
    private service: MedicalConditionService,
    private fb: FormBuilder
  ) {
    this.createMedicalConditionForm = this.fb.group({
      name: ['', Validators.required],
      code: [null, Validators.required],
      description: [null, Validators.required],
      symptoms: this.fb.array([])
    });

    this.updateMedicalConditionForm = this.fb.group({
      name: ['', Validators.required],
      code: [null, Validators.required],
      description: [null, Validators.required],
      symptoms: this.fb.array([])
    });
  }

  ngOnInit(): void {
    const session = this.getSession();
    this.role = session?.role || null;

    this.updateMedicalConditionForm.get('code')?.disable();

    this.service.getAllMedicalCondition().subscribe((mc) => {
      this.medicalConditionList = mc.map(medicalCondition => ({
        ...medicalCondition
      }));
      this.filteredMedicalConditionList = [...this.medicalConditionList];
    });

    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS }
    ];
  }

  openCreateModal(): void {
    this.showCreate = true;
  }

  addMedicalCondition(): void {

    if (this.createMedicalConditionForm.invalid) return;

    this.showCreate = false;

    const request: MedicalConditionDto = {
      ...this.createMedicalConditionForm.value,
      symptoms: this.symptoms.value.map((fh: any) => fh.symptomText)
    };

    this.service.addMedicalCondition(request).subscribe({
      next: () => {
        // Success message
        this.message = [
          {
            severity: 'success',
            summary: 'Success!',
            detail: 'Your Medical Condition was added with success',
          },
        ];

        this.createMedicalConditionForm.reset();
        this.symptoms.clear();

        this.service.getAllMedicalCondition().subscribe((mc) => {
          this.medicalConditionList = mc.map(medicalCondition => ({
            ...medicalCondition
          }));
          this.filteredMedicalConditionList = [...this.medicalConditionList];
        });

      },
      error: (error) => this.onFailure(error),
    });
  }

  onFailure(error: HttpErrorResponse): void {
    this.message = [
      {
        severity: 'error',
        summary: 'Failure!',
        detail: error.status >= 500 ? 'Server error' : error.error.message
      },
    ];
  }

  get symptoms(): FormArray {
    return this.createMedicalConditionForm.get('symptoms') as FormArray;
  }

  addSymptoms(): void {
    const symptomsGroup = this.fb.group({
      symptomText: ['', Validators.required]
    });
    this.symptoms.push(symptomsGroup);
  }

  removeSymptoms(index: number): void {
    this.symptoms.removeAt(index);
  }

  openDetailsModal(medicalCondition: MedicalCondition): void {
    this.currentMedicalCondition = medicalCondition;
    this.showDetails = true;
  }

  openUpdateModal(medicalCondition: MedicalCondition): void {
    this.currentMedicalCondition = medicalCondition;
    this.showUpdate = true;

    this.updateMedicalConditionForm.reset();
    this.updateSymptoms.clear();

    this.updateMedicalConditionForm.patchValue({
      name: medicalCondition.name,
      code: medicalCondition.code,
      description: medicalCondition.description,
    });

    medicalCondition.symptoms.forEach(symptom => {
      this.updateSymptoms.push(this.fb.group({
        symptomText: [symptom, Validators.required],
      }));
    });
  }

  get updateSymptoms(): FormArray {
    return this.updateMedicalConditionForm.get('symptoms') as FormArray;
  }

  addUpdateSymptom(): void {
    const symptomGroup = this.fb.group({
      symptomText: ['', Validators.required],
    });
    this.updateSymptoms.push(symptomGroup);
  }

  removeUpdateSymptom(index: number): void {
    this.updateSymptoms.removeAt(index);
  }

  updateMedicalCondition(): void {
    if (this.updateMedicalConditionForm.invalid) return;

    this.showUpdate = false;

    const { id, ...currentMedicalConditionWithoutId } = this.currentMedicalCondition || {};

    const updatedData: MedicalConditionDto = {
      ...currentMedicalConditionWithoutId,
      ...this.updateMedicalConditionForm.value,
      symptoms: this.updateSymptoms.value.map((symptom: any) => symptom.symptomText)
    };

    this.service.updateMedicalCondition(id, updatedData).subscribe({
      next: () => {
        this.message = [
          {
            severity: 'success',
            summary: 'Success!',
            detail: 'Medical Condition updated successfully!',
          },
        ];

        // Refresh the operation type list
        this.service.getAllMedicalCondition().subscribe((mc) => {
          this.medicalConditionList = mc.map(medicalCondition => ({
            ...medicalCondition
          }));
          this.filteredMedicalConditionList = [...this.medicalConditionList];
        });
      },
      error: (error) => this.onFailure(error),
    });
  }

  private getSession() {
    const session = sessionStorage.getItem('SessionUtilizadorInfo');
    return session ? JSON.parse(session) : null;
  }


}
