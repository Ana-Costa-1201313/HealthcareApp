import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Message, SelectItem } from 'primeng/api';
import { PatientMedicalRecord } from '../../model/patientMedicalRecord/patientMedicalRecord.model';
import { AllergyService } from '../../services/allergy.service';
import { MedicalConditionService } from '../../services/medicalCondition.service';
import { PatientMedicalRecordService } from '../../services/patientMedicalRecord.service';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FloatLabelModule } from 'primeng/floatlabel';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MessagesModule } from 'primeng/messages';
import { MultiSelectModule } from 'primeng/multiselect';
import { MenubarComponent } from '../menubar/menubar.component';
import { PatientService } from '../../services/patient.service';
import { Patient } from '../../model/patient.model';

@Component({
  selector: 'app-patientmedicalrecord',
  standalone: true,
  imports: [
    CommonModule, TableModule, DialogModule, ReactiveFormsModule, ButtonModule,
    MessagesModule, DropdownModule, InputTextModule, InputTextareaModule, FloatLabelModule, MultiSelectModule, MenubarComponent
  ],
  templateUrl: './patientmedicalrecord.component.html',
  styleUrls: ['./patientmedicalrecord.component.css']
})
export class PatientmedicalrecordComponent implements OnInit {
  medicalRecordList: PatientMedicalRecord[] = [];
  showCreateModal = false;
  showEditAllergiesModal = false;
  showEditConditionsModal = false;
  showEditFamilyHistoryModal = false;
  showEditFreeTextModal = false;

  createMedicalRecordForm: FormGroup;
  editAllergiesForm: FormGroup;
  editConditionsForm: FormGroup;
  editFamilyHistoryForm: FormGroup;
  editFreeTextForm: FormGroup;

  allergyNames: SelectItem[] = [];
  medicalConditionNames: SelectItem[] = [];
  familyHistoryEntries: SelectItem[] = [];
  freeTextEntries: SelectItem[] = [];
  message: Message[] = [];

  currentMedicalRecord: PatientMedicalRecord | null = null;

  patients: Patient[] = [];
  patientsEmais: string[] = [];

  constructor(
    private fb: FormBuilder,
    private patientMedicalRecordService: PatientMedicalRecordService,
    private medicalConditionService: MedicalConditionService,
    private allergyService: AllergyService,
    private patientService: PatientService
  ) {
    // Formulário para criar registro médico
    this.createMedicalRecordForm = this.fb.group({
      patientId: ['', Validators.required],
      allergies: [[]],
      medicalConditions: [[]],
      familyHistory: this.fb.array([]),
      freeText: this.fb.array([])
    });

    this.editAllergiesForm = this.fb.group({
      allergies: [[]],
    });

    this.editConditionsForm = this.fb.group({
      medicalConditions: [[]],
     });

    this.editFamilyHistoryForm = this.fb.group({
      familyHistory: this.fb.array([]),
     });

     this.editFreeTextForm = this.fb.group({
      freeText: [[]],
     });
  }

  ngOnInit(): void {

    this.patientService.getPatientList().subscribe((p) => {
      this.patients = p;

      const emails: string[] = [];

      this.patients.forEach((p) => emails.push(p.email));
      this.patientsEmais = emails;
    });

    // Carregar registros médicos
    this.patientMedicalRecordService.getMedicalRecords().subscribe((records) => {
      this.medicalRecordList = records;
    });

    // Carregar nomes de alergias
    this.allergyService.getAllergies().subscribe((allergies) => {
      this.allergyNames = allergies.map((allergy) => ({
        label: allergy.name || '',
        value: allergy.name || ''
      }));
    });

    // Carregar nomes de condições médicas
    this.medicalConditionService.getAllMedicalCondition().subscribe((conditions) => {
      this.medicalConditionNames = conditions.map((condition) => ({
        label: condition.name || '',
        value: condition.name || ''
      }));
    });

    this.familyHistoryEntries = this.familyHistoryEntries.map((history) => ({
      label: history.label || '',
      value: history.label || ''
    }));

    this.freeTextEntries = this.freeTextEntries.map((entry) => ({
      label: entry.label || '',
      value: entry.label || ''
    }));
  }

  openCreateModal(): void {
    this.createMedicalRecordForm.reset();
    this.familyHistory.clear();
    this.freeText.clear();
    this.showCreateModal = true;
  }

  // Métodos para Family History
  get familyHistory(): FormArray {
    return this.createMedicalRecordForm.get('familyHistory') as FormArray;
  }

  get familyHistoryArray(): FormArray {
    return this.editFamilyHistoryForm.get('familyHistory') as FormArray;
  }

  addFamilyHistory(): void {
    this.familyHistory.push(this.fb.control('', Validators.required));
  }

  addFamilyHistoryEdit(): void {
    this.familyHistoryArray.push(this.fb.group({ text: ['', Validators.required] }));
  }

  removeFamilyHistory(index: number): void {
    this.familyHistoryArray.removeAt(index);
  }

  // Métodos para Free Text
  get freeText(): FormArray {
    return this.createMedicalRecordForm.get('freeText') as FormArray;
  }

  get freeTextArray(): FormArray {
    return this.editFreeTextForm.get('freeText') as FormArray;
  }

  addFreeText(): void {
    this.freeText.push(this.fb.control('', Validators.required));
  }

  addFreeTextEdit(): void {
    this.freeTextArray.push(this.fb.group({ text: ['', Validators.required] }));
  }

  removeFreeText(index: number): void {
    this.freeTextArray.removeAt(index);
  }

  createMedicalRecord(): void {
    if (this.createMedicalRecordForm.invalid) return;

    const record: PatientMedicalRecord = this.createMedicalRecordForm.value;

    this.patientMedicalRecordService.createMedicalRecord(record).subscribe({
      next: () => {
        this.showCreateModal = false;
        this.message = [
          { severity: 'success', summary: 'Success', detail: 'Record created successfully!' },
        ];
        this.ngOnInit(); // Recarregar a lista
      },
      error: (error) => {
        this.message = [
          { severity: 'error', summary: 'Error', detail: error.error.message },
        ];
      },
    });
  }

  openEditAllergiesModal(record: PatientMedicalRecord): void {
    this.currentMedicalRecord = record;
    this.editAllergiesForm.patchValue({
      allergies: record.allergies || [],
    });
    this.showEditAllergiesModal = true;
  }

  updateAllergies(): void {
    if (!this.currentMedicalRecord) return;

    console.log('Updating allergies for record ID:', this.currentMedicalRecord.id);
    const allergies = this.editAllergiesForm.value.allergies;

    this.patientMedicalRecordService
      .updateMedicalRecordAllergies(this.currentMedicalRecord.id!, allergies)
      .subscribe({
        next: (updatedRecord) => {
          this.currentMedicalRecord!.allergies = updatedRecord.allergies;
          this.message = [
            { severity: 'success', summary: 'Success', detail: 'Allergies updated successfully!' },
          ];
          this.showEditAllergiesModal = false;
        },
        error: () => {
          this.message = [
            { severity: 'error', summary: 'Error', detail: 'Failed to update allergies.' },
          ];
        },
      });
  }

  openEditConditionsModal(record: PatientMedicalRecord): void {
    this.currentMedicalRecord = record;
    this.editConditionsForm.patchValue({
      medicalConditions: record.medicalConditions || [],
    });
    this.showEditConditionsModal = true;
  }

  updateConditions(): void {
    if (!this.currentMedicalRecord) return;

    const conditions = this.editConditionsForm.value.medicalConditions;

    this.patientMedicalRecordService
      .updateMedicalRecordMedicalConditions(this.currentMedicalRecord.id!, conditions)
      .subscribe({
        next: (updatedRecord) => {
          this.currentMedicalRecord!.medicalConditions = updatedRecord.medicalConditions;
          this.message = [
            { severity: 'success', summary: 'Success', detail: 'Medical conditions updated successfully!' },
          ];
          this.showEditConditionsModal = false;
        },
        error: () => {
          this.message = [
            { severity: 'error', summary: 'Error', detail: 'Failed to update medical conditions.' },
          ];
        },
      });
  }

  openEditFamilyHistoryModal(record: PatientMedicalRecord): void {
    this.currentMedicalRecord = record;
  
    const familyHistoryArray = this.fb.array(
      record.familyHistory.map((history) => this.fb.group({ text: [history, Validators.required] }))
    );
  
    this.editFamilyHistoryForm.setControl('familyHistory', familyHistoryArray);
    this.showEditFamilyHistoryModal = true;
  }

  updateFamilyHistory(): void {
    if (!this.currentMedicalRecord || this.editFamilyHistoryForm.invalid) return;
  
    // Transform the familyHistoryArray values to a simple array of strings
    const familyHistoryValues = this.familyHistoryArray.value.map((item: any) => item.text);
  
    this.patientMedicalRecordService
      .updateMedicalRecordFamilyHistory(this.currentMedicalRecord.id!, familyHistoryValues)
      .subscribe({
        next: (updatedRecord) => {
          this.currentMedicalRecord!.familyHistory = updatedRecord.familyHistory;
          this.message = [
            { severity: 'success', summary: 'Success', detail: 'Family history updated successfully!' },
          ];
          this.showEditFamilyHistoryModal = false;
        },
        error: () => {
          this.message = [
            { severity: 'error', summary: 'Error', detail: 'Failed to update family history.' },
          ];
        },
      });
  }

  openEditFreeTextModal(record: PatientMedicalRecord): void {
    this.currentMedicalRecord = record;

    const freeTextArray = this.fb.array(
      record.freeText.map((text) => this.fb.group({ text: [text, Validators.required] }))
    ); 

    this.editFreeTextForm.setControl('freeText', freeTextArray);
    this.showEditFreeTextModal = true;
  }

  updateFreeText(): void {
    if (!this.currentMedicalRecord || this.editFreeTextForm.invalid) return;

    const freeTextValues = this.freeTextArray.value.map((entry: { text: string }) => entry.text);

    this.patientMedicalRecordService
        .updateMedicalRecordFreeText(this.currentMedicalRecord.id!, freeTextValues)
        .subscribe({
            next: (updatedRecord) => {
                this.currentMedicalRecord!.freeText = updatedRecord.freeText;
                this.message = [
                    { severity: 'success', summary: 'Success', detail: 'Free Text updated successfully!' },
                ];
                this.showEditFreeTextModal = false;
            },
            error: () => {
                this.message = [
                    { severity: 'error', summary: 'Error', detail: 'Failed to update free text.' },
                ];
            },
        });
  }
}
