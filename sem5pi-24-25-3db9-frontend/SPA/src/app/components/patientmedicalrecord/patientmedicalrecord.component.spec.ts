import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientmedicalrecordComponent } from './patientmedicalrecord.component';
import { PatientMedicalRecordService } from '../../services/patientMedicalRecord.service';
import { AllergyService } from '../../services/allergy.service';
import { MedicalConditionService } from '../../services/medicalCondition.service';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { PatientMedicalRecord } from '../../model/patientMedicalRecord/patientMedicalRecord.model';
import { SelectItem } from 'primeng/api';
import { PatientService } from '../../services/patient.service';

describe('PatientmedicalrecordComponent', () => {
  let component: PatientmedicalrecordComponent;
  let fixture: ComponentFixture<PatientmedicalrecordComponent>;
  let medicalRecordService: jasmine.SpyObj<PatientMedicalRecordService>;
  let allergyService: jasmine.SpyObj<AllergyService>;
  let medicalConditionService: jasmine.SpyObj<MedicalConditionService>;
  let patientService: jasmine.SpyObj<PatientService>;

  beforeEach(async () => {
    const mockMedicalRecordService = jasmine.createSpyObj('PatientMedicalRecordService', [
      'getMedicalRecords',
      'createMedicalRecord',
      'updateMedicalRecordAllergies',
      'updateMedicalRecordMedicalConditions',
    ]);

    const mockAllergyService = jasmine.createSpyObj('AllergyService', ['getAllergies']);
    const mockMedicalConditionService = jasmine.createSpyObj('MedicalConditionService', ['getAllMedicalCondition']);
    
    const mockPatientService = jasmine.createSpyObj('PatientService', ['getPatientList']);

    await TestBed.configureTestingModule({
      imports: [PatientmedicalrecordComponent, ReactiveFormsModule],
      providers: [
        { provide: PatientMedicalRecordService, useValue: mockMedicalRecordService },
        { provide: AllergyService, useValue: mockAllergyService },
        { provide: MedicalConditionService, useValue: mockMedicalConditionService },
        { provide: PatientService, useValue: mockPatientService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientmedicalrecordComponent);
    component = fixture.componentInstance;

    medicalRecordService = TestBed.inject(PatientMedicalRecordService) as jasmine.SpyObj<PatientMedicalRecordService>;
    allergyService = TestBed.inject(AllergyService) as jasmine.SpyObj<AllergyService>;
    medicalConditionService = TestBed.inject(MedicalConditionService) as jasmine.SpyObj<MedicalConditionService>;
    patientService = TestBed.inject(PatientService) as jasmine.SpyObj<PatientService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });


  it('should open create modal and reset form', () => {
    spyOn(component.createMedicalRecordForm, 'reset');

    component.openCreateModal();

    expect(component.showCreateModal).toBeTrue();
    expect(component.createMedicalRecordForm.reset).toHaveBeenCalled();
  });

  it('should open edit allergies modal with correct data', () => {
    const record: PatientMedicalRecord = { id: '1', patientId: '1001', allergies: ['Peanuts'] };

    component.openEditAllergiesModal(record);

    expect(component.currentMedicalRecord).toEqual(record);
    
  });

  it('should open edit allergies modal with correct data 2', () => {
    const record: PatientMedicalRecord = { id: '1', patientId: '1001', allergies: ['Peanuts'] };

    component.openEditAllergiesModal(record);

    
    expect(component.editAllergiesForm.value.allergies).toEqual(['Peanuts']);
    
  });

  it('should update allergies successfully', () => {
    const record: PatientMedicalRecord = { id: '1', patientId: '1001', allergies: ['Peanuts'] };
    const updatedRecord: PatientMedicalRecord = { ...record, allergies: ['Peanuts', 'Shellfish'] };

    medicalRecordService.updateMedicalRecordAllergies.and.returnValue(of(updatedRecord));

    component.currentMedicalRecord = record;
    component.editAllergiesForm.patchValue({ allergies: ['Peanuts', 'Shellfish'] });

    component.updateAllergies();

    expect(medicalRecordService.updateMedicalRecordAllergies).toHaveBeenCalledWith(
      record.id!,
      ['Peanuts', 'Shellfish']
    );
    expect(component.currentMedicalRecord?.allergies).toEqual(['Peanuts', 'Shellfish']);
    
  });

  it('should update allergies successfully 2', () => {
    const record: PatientMedicalRecord = { id: '1', patientId: '1001', allergies: ['Peanuts'] };
    const updatedRecord: PatientMedicalRecord = { ...record, allergies: ['Peanuts', 'Shellfish'] };

    medicalRecordService.updateMedicalRecordAllergies.and.returnValue(of(updatedRecord));

    component.currentMedicalRecord = record;
    component.editAllergiesForm.patchValue({ allergies: ['Peanuts', 'Shellfish'] });

    component.updateAllergies();

    expect(medicalRecordService.updateMedicalRecordAllergies).toHaveBeenCalledWith(
      record.id!,
      ['Peanuts', 'Shellfish']
    );
    
    expect(component.showEditAllergiesModal).toBeFalse();
  });

  it('should open edit medical conditions modal with correct data', () => {
    const record: PatientMedicalRecord = {
      id: '1',
      patientId: '1001',
      medicalConditions: ['Rotavirus enteritis', 'Plasmodium falciparum malaria'],
    };
  
    component.openEditConditionsModal(record);
  
    expect(component.currentMedicalRecord).toEqual(record);
    expect(component.editConditionsForm.value.medicalConditions).toEqual([
      'Rotavirus enteritis',
      'Plasmodium falciparum malaria',
    ]);
    expect(component.showEditConditionsModal).toBeTrue();
  });
  
  it('should open edit medical conditions modal with correct data', () => {
    const record: PatientMedicalRecord = {
      id: '1',
      patientId: '1001',
      medicalConditions: ['Rotavirus enteritis', 'Plasmodium falciparum malaria'],
    };
  
    component.openEditConditionsModal(record);
  
    expect(component.currentMedicalRecord).toEqual(record);
    expect(component.editConditionsForm.value.medicalConditions).toEqual([
      'Rotavirus enteritis',
      'Plasmodium falciparum malaria',
    ]);
    expect(component.showEditConditionsModal).toBeTrue();
  });

  it('should update medical conditions successfully', () => {
    const record: PatientMedicalRecord = {
      id: '1',
      patientId: '1001',
      medicalConditions: ['Rotavirus enteritis'],
    };
    const updatedRecord: PatientMedicalRecord = {
      ...record,
      medicalConditions: ['Rotavirus enteritis', 'Plasmodium falciparum malaria'],
    };
  
    medicalRecordService.updateMedicalRecordMedicalConditions.and.returnValue(of(updatedRecord));
  
    component.currentMedicalRecord = record;
    component.editConditionsForm.patchValue({
      medicalConditions: ['Rotavirus enteritis', 'Plasmodium falciparum malaria'],
    });
  
    component.updateConditions();
  
    expect(medicalRecordService.updateMedicalRecordMedicalConditions).toHaveBeenCalledWith(
      record.id!,
      ['Rotavirus enteritis', 'Plasmodium falciparum malaria']
    );
    expect(component.currentMedicalRecord?.medicalConditions).toEqual([
      'Rotavirus enteritis',
      'Plasmodium falciparum malaria',
    ]);
    expect(component.showEditConditionsModal).toBeFalse();
  });


  
  
});
