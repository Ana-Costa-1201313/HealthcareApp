import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { StaffComponent } from './components/staff/staff.component';
import { PatientComponent } from './components/patient/patient.component';
import { OperationtypeComponent } from './components/operationtype/operationtype.component';
import { SpecializationComponent } from './components/specialization/specialization.component';
import { PlanningComponent } from './components/planning/planning.component';
import { PatientProfileComponent } from './components/patient/patientprofile.component';
import { LoginComponent } from './components/login/login.component';
import { VisualizationComponent } from './components/visualization/visualization.component';
import { OperationrequestComponent } from './components/operationrequest/operationrequest.component';
import { ProfileComponent } from './components/profile/profile.component';
import { RoomsComponent } from './components/rooms/rooms.component';
import { RoomTypesComponent } from './components/room-types/room-types.component';
import { MedicalconditionComponent } from './components/medicalcondition/medicalcondition.component';
import { AllergyComponent } from './components/allergy/allergy.component';
import { PatientmedicalrecordComponent } from './components/patientmedicalrecord/patientmedicalrecord.component';
import { AppointmentComponent } from './components/appointment/appointment.component';

export const routes: Routes = [
  {
    path: '',
    component: LoginComponent,
    title: 'Login page',
  },
  {
    path: 'home',
    component: HomeComponent,
    title: 'Home page',
  },
  {
    path: 'staff',
    component: StaffComponent,
    title: 'Staff page',
  },
  {
    path: 'patient',
    component: PatientComponent,
    title: 'Patient page',
  },
  {
    path: 'operationtype',
    component: OperationtypeComponent,
    title: 'Operation Type page',
  },
  {
    path: 'specialization',
    component: SpecializationComponent,
    title: 'Specialization page',
  },
  {
    path: 'planning',
    component: PlanningComponent,
    title: 'Planning page',
  },
  {
    path: 'visualization',
    component: VisualizationComponent,
    title: 'Visualization page',
  },
  {
    path: 'operationrequest',
    component: OperationrequestComponent,
    title: 'Operation Request page',
  },
  {
    path: 'appointment',
    component: AppointmentComponent,
    title: 'Appointment page',
  },
  {
    path: 'patientprofile',
    component: PatientProfileComponent,
    title: 'Patient Profile page',
  },
  {
    path: 'profile',
    component: ProfileComponent,
    title: 'Profile',
  },
  {
    path: 'allergies',
    component: AllergyComponent,
    title: 'Allergy page',
  },
  {
    path: 'medicalcondition',
    component: MedicalconditionComponent,
    title: 'Medical Condition',
  },
  {
    path: 'patientMedicalRecord',
    component: PatientmedicalrecordComponent,
    title: 'Patient Medical Record'
  },
  {
    path: 'room',
    component: RoomsComponent,
    title: 'Room',
  },
  {
    path: 'roomtype',
    component: RoomTypesComponent,
    title: 'RoomTypes',
  },
  {
    path: '**',
    component: LoginComponent,
    title: 'Login page',
  }
];
