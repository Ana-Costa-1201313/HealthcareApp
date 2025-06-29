import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PlanningDTO } from '../model/planningDto.model';
import { AppointmentDto } from '../model/appointmentDto.model';

@Injectable({ providedIn: 'root' })
export class PlanningService {
    url = `${environment.apiUrl}${environment.endpoints.planning}`;

    constructor(private http: HttpClient) { }

    postPlanning(plan: PlanningDTO): Observable<AppointmentDto[]> {
        return this.http.post<AppointmentDto[]>(this.url, plan);
    }
}