import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FilterMatchMode, Message, SelectItem } from 'primeng/api';
import { Button, ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { MessagesModule } from 'primeng/messages';
import { TableModule } from 'primeng/table';
import { Patient } from '../../model/patient.model';
import { PatientService } from '../../services/patient.service';
import { FormsModule } from '@angular/forms';
import { EditPatient } from '../../model/editPatient.model';
import { HttpErrorResponse } from '@angular/common/http';
import { MenubarComponent } from '../menubar/menubar.component';

@Component({
  selector: 'app-patient',
  standalone: true,
  imports: [CommonModule, TableModule, DialogModule, FormsModule, ButtonModule,MessagesModule, MenubarComponent],
  templateUrl: './patientprofile.component.html',
  styleUrl: './patientprofile.component.css'
})
export class PatientProfileComponent implements OnInit {
  patientList: Patient[] = [];
  filteredPatientList: Patient[] = [];
  currentPatient: Patient | null = null;
  showDetails: boolean = false;
  showEdit: boolean = false;
  newPatient: Partial<Patient> = {};
  editingPatient: Patient = {} as Patient;
  matchModeOptions: SelectItem[] = [];
  message: Message[] = [];

  constructor(private patientService: PatientService) { }

  ngOnInit(): void {
    // Carregar lista de pacientes e configurar filtros
    this.patientService.getPatientList().subscribe((patients) => {
      this.patientList = patients.map(patient => ({
        ...patient,
        dateOfBirth: new Date(patient.dateOfBirth), 
      }));
      this.filteredPatientList = [...this.patientList];
    });
    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS }
    ];
  }

  openDetailsModal(patient: Patient): void {
    this.currentPatient = patient;
    this.showDetails = true;
  }

  openEditModal(patient: Patient){
    this.editingPatient = {...patient};
    this.showEdit = true;
  }

  onFailure(error: HttpErrorResponse): void {
    this.message = [{
      severity: 'error',
      summary: 'Failure!',
      detail: error.status >= 500 ? 'Server error' : error.error.message
    }];
  }
 
  submitEditPatient(): void {
 
  const editPatientData: EditPatient = {
    firstName: this.editingPatient.firstName,
    lastName: this.editingPatient.lastName,
    fullName: this.editingPatient.fullName,
    email: this.editingPatient.email,
    phone: this.editingPatient.phone,
    emergencyContact: this.editingPatient.emergencyContact,
    allergies: this.editingPatient.allergies
  };

  
  this.patientService.updatePatient(this.editingPatient.id, editPatientData).subscribe(
    (updatedPatient) => {
      
      const index = this.patientList.findIndex(p => p.id === updatedPatient.id);
      if (index !== -1) {
        this.patientList[index] = updatedPatient;
        this.filteredPatientList = [...this.patientList];

        this.message = [{
          severity: 'success',
          summary: 'Success',
          detail: `${this.editingPatient.fullName}'s profile has been edited successfully.`
        }];
      }
      this.showEdit = false; 
    },
    (error) => this.onFailure(error)
  );
}
  
}
