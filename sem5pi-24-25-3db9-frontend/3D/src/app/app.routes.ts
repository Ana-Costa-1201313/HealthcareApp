import { Routes } from '@angular/router';
import { HospitalComponent } from './components/hospital/hospital.component';

export const routes: Routes = [

    { path: '', redirectTo: '/hospital', pathMatch: 'full'},
    { path: 'hospital', component: HospitalComponent},

];
