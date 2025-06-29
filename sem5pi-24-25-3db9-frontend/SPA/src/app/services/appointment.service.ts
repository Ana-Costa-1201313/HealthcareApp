import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AppointmentDto } from '../model/appointmentDto.model';
import { CreateAppointment } from '../model/createAppointment.model';
import { EditAppointment } from '../model/editAppointmentDto.model';
import { Appointment } from '../model/appointment.model';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  url = `${environment.apiUrl}${environment.endpoints.appointment}`;

  constructor(private http: HttpClient) {}

  getAppointmentList(): Observable<AppointmentDto[]> {
    return this.http.get<AppointmentDto[]>(this.url);
  }

  getAppointmentListWithDetails(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.url}/details`);
  }

  addAppointment(appointment: CreateAppointment): Observable<AppointmentDto> {
    return this.http.post<AppointmentDto>(this.url, appointment);
  }

  updateAppointment(appointmentId: string, editAppointment: EditAppointment): Observable<Appointment> {
    return this.http.patch<Appointment>(`${this.url}/${appointmentId}`, editAppointment);
  }
}