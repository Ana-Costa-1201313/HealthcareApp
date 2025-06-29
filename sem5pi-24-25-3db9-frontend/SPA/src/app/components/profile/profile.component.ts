import { Component, OnInit } from '@angular/core';
import { MenubarComponent } from '../menubar/menubar.component';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { TabViewModule } from 'primeng/tabview';
import { PatientService } from '../../services/patient.service';
import { StaffService } from '../../services/staff.service';
import { Patient } from '../../model/patient.model';
import { Staff } from '../../model/staff/staff.model';
import { CommonModule } from '@angular/common';
import { SelectButtonModule } from 'primeng/selectbutton';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { DataDeletionPatientRequestDto } from '../../model/dataDeletionPatientRequestDto.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [MenubarComponent, AvatarModule, AvatarGroupModule, TabViewModule, CommonModule, SelectButtonModule, FormsModule, ReactiveFormsModule, ButtonModule, DialogModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  accountPatient: Patient | null = null;
  accountStaff: Staff | null = null;
  email: string;
  role: string;
  deactivate: boolean = false;
  showPrivacyPolicy: boolean = false;
  formFormatDownload!: FormGroup;

  stateOptions: any[] = [
    { label: 'json', value: 'json' },
    { label: 'xml', value: 'xml' }
  ];



  constructor(
    private patientService: PatientService,
    private staffService: StaffService) {


    this.formFormatDownload = new FormGroup({
      value: new FormControl(null)
    });



  }

  ngOnInit(): void {

    const session = this.getSession();
    this.email = session?.username || null;
    this.role = session?.role || null;

    if (this.role == 'Patient') {
      this.patientService.getPatientByEmail(this.email).subscribe((p) => {
        this.accountPatient = p;
      });

    } else if (this.role == 'Doctor' || this.role == 'Nurse' || this.role == 'Technician') {
      this.staffService.getStaffByEmail(this.email).subscribe((s) => {
        this.accountStaff = s;
      });
    }
  }

  private getSession() {
    const session = sessionStorage.getItem('SessionUtilizadorInfo');
    return session ? JSON.parse(session) : null;
  }

  public downloadMedicalHistory() {

    const format = this.formFormatDownload.get('value')?.value;

    if (format === 'json') {
      this.patientService.getPatientMedicalHistoryJson(this.email).subscribe((response: Blob) => {
        this.downloadFile(response, 'medical-history.json');
      });
    } else if (format === 'xml') {
      this.patientService.getPatientMedicalHistoryXml(this.email).subscribe((response: Blob) => {
        this.downloadFile(response, 'medical-history.xml');
      });
    }

  }


  private downloadFile(blob: Blob, filename: string) {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
  }

  public deleteRequest() {
    this.deactivate = true;
  }

  public sendRequest() {

    const request: DataDeletionPatientRequestDto = {
      email: this.email
    };

    this.patientService.createPatientDeleteRequest(request).subscribe({
      next: (response: string) => {
        console.log('Response:', response);
        this.deactivate = false;
      }
    });

  }

}
