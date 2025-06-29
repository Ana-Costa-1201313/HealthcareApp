import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientComponent } from './patient.component';
import { PatientService } from '../../services/patient.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { Patient } from '../../model/patient.model';
import { of } from 'rxjs';
import { ExclamationTriangleIcon } from 'primeng/icons/exclamationtriangle';

describe('PatientComponent', () => {
  let component: PatientComponent;
  let fixture: ComponentFixture<PatientComponent>;
  let service: PatientService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientComponent],
      providers:[
        PatientService,
        provideHttpClientTesting(),
        provideHttpClient(),
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(PatientService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load patient profile list', () => {
    const mockPatients: Patient[] = [
      {
        id: '1',
        firstName: 'Bernardo',
        lastName: 'Silva',
        fullName: 'Bernardo Silva',
        gender: 'Masculine',
        dateOfBirth: new Date('2000-01-01'),
        email: 'bernardoSilva@gmail.com',
        phone: '919100055',
        emergencyContact: '987987986',
      },
      {
        id: '2',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
      },
    ];

    spyOn(service,'getPatientList').and.returnValue(of(mockPatients));

    component.ngOnInit();
    
    expect(component.patientList).toEqual(mockPatients);
    
  });

  it('should load patient profile list 2', () => {
    const mockPatients: Patient[] = [
      {
        id: '1',
        firstName: 'Bernardo',
        lastName: 'Silva',
        fullName: 'Bernardo Silva',
        gender: 'Masculine',
        dateOfBirth: new Date('2000-01-01'),
        email: 'bernardoSilva@gmail.com',
        phone: '919100055',
        emergencyContact: '987987986',
      },
      {
        id: '2',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
      },
    ];

    spyOn(service,'getPatientList').and.returnValue(of(mockPatients));

    component.ngOnInit();
    
    
    expect(component.filteredPatientList).toEqual(mockPatients);
   
  });

  it('should open the create modal', ()=>{
    component.openCreateModal();

    expect(component.newPatient).toEqual({});
    expect(component.showCreate).toBeTrue();
  });

  it('should open the details modal', () => {

    const mockPatient: Patient = {
      id: '1',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    component.openDetailsModal(mockPatient);

    expect(component.currentPatient).toEqual(mockPatient);
    expect(component.showDetails).toBeTrue;
  });

  it('should open the edit modal', ()=> {
    const mockPatient: Patient = {
      id: '1',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    component.openEditModal(mockPatient);

    expect(component.editingPatient).toEqual(mockPatient);
    expect(component.showEdit).toBeTrue();
  });

  it('should create a new patient profile', () => {
    const newPatient: Partial<Patient> = {
      firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    spyOn(service, 'createPatient').and.returnValue(of(newPatient as Patient));

    component.newPatient = newPatient;
    component.submitNewPatient();

    expect(component.patientList).toContain(jasmine.objectContaining(newPatient));
   
  });

  it('should create a new patient profile 2', () => {
    const newPatient: Partial<Patient> = {
      firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    spyOn(service, 'createPatient').and.returnValue(of(newPatient as Patient));

    component.newPatient = newPatient;
    component.submitNewPatient();

   
    expect(component.filteredPatientList).toContain(jasmine.objectContaining(newPatient));
   
  });

  it('should create a new patient profile 3', () => {
    const newPatient: Partial<Patient> = {
      firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    spyOn(service, 'createPatient').and.returnValue(of(newPatient as Patient));

    component.newPatient = newPatient;
    component.submitNewPatient();

   
    expect(component.message[0].severity).toBe('success');
  });

  it('should edit a patient profile', () => {
    const mockPatient: Patient = {
      id: '1',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    const updatedPatient: Patient = {
      ...mockPatient, 
      firstName: 'Bernardo',
      lastName: 'Silva',
      fullName: 'Bernardo Silva'
    };

    component.patientList = [mockPatient];
    component.editingPatient = {... mockPatient};
    spyOn(service, 'updatePatient').and.returnValue(of(updatedPatient));

    component.submitEditPatient();

    expect(component.patientList[0]).toEqual(updatedPatient);
    expect(component.filteredPatientList[0]).toEqual(updatedPatient);
    expect(component.showEdit).toBeFalse();
    expect(component.message[0].severity).toBe('success');
    expect(component.patientList[0].firstName).toEqual('Bernardo');
    expect(component.patientList[0].email).toEqual('rubenDias@gmail.com');
  });

  it('should edit a patient profile 2', () => {
    const mockPatient: Patient = {
      id: '1',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    const updatedPatient: Patient = {
      ...mockPatient, 
      firstName: 'Bernardo',
      lastName: 'Silva',
      fullName: 'Bernardo Silva'
    };

    component.patientList = [mockPatient];
    component.editingPatient = {... mockPatient};
    spyOn(service, 'updatePatient').and.returnValue(of(updatedPatient));

    component.submitEditPatient();

    
    expect(component.patientList[0].firstName).toEqual('Bernardo');
    
  });

  it('should edit a patient profile  3', () => {
    const mockPatient: Patient = {
      id: '1',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };
    const updatedPatient: Patient = {
      ...mockPatient, 
      firstName: 'Bernardo',
      lastName: 'Silva',
      fullName: 'Bernardo Silva'
    };

    component.patientList = [mockPatient];
    component.editingPatient = {... mockPatient};
    spyOn(service, 'updatePatient').and.returnValue(of(updatedPatient));

    component.submitEditPatient();

    
    expect(component.patientList[0].email).toEqual('rubenDias@gmail.com');
  });

  it('should delete a patient profile', () => {
    const mockPatient: Patient = {
      id: '1',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };

    component.patientList = [mockPatient];
    component.filteredPatientList = [mockPatient];
    spyOn(service,'deletePatient').and.returnValue(of(null));

    component.currentPatient= mockPatient;
    component.confirmDeletePatient();
    
    expect(component.patientList).not.toContain(mockPatient);
    
  })

  it('should delete a patient profile 2', () => {
    const mockPatient: Patient = {
      id: '1',
        firstName: 'Ruben',
        lastName: 'Dias',
        fullName: 'Ruben Dias',
        gender: 'Masculine',
        dateOfBirth: new Date('1990-01-01'),
        email: 'rubenDias@gmail.com',
        phone: '919100056',
        emergencyContact: '987987982',
    };

    component.patientList = [mockPatient];
    component.filteredPatientList = [mockPatient];
    spyOn(service,'deletePatient').and.returnValue(of(null));

    component.currentPatient= mockPatient;
    component.confirmDeletePatient();
    
    
   
    expect(component.message[0].severity).toBe('success');
  })
});
