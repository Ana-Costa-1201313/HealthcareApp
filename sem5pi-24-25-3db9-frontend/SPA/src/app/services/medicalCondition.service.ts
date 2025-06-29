import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { MedicalConditionDto } from '../model/medicalCondition/medicalConditionDto.model';
import { MedicalCondition } from '../model/medicalCondition/medicalCondition.model';
import { MedicalConditionMapper } from '../model/medicalCondition/medicalConditionMapper.model';

@Injectable({ providedIn: 'root' })
export class MedicalConditionService {
  url = `${environment.recordsBackofficeApiUrl}${environment.endpoints.medicalCondition}`;

  constructor(private http: HttpClient) { }

  addMedicalCondition(medicalCondition: MedicalConditionDto): Observable<MedicalCondition> {
    return this.http.post<MedicalConditionDto>(this.url, medicalCondition).pipe(
      map(MedicalConditionMapper.toMedicalCondition)
    );
  }

  getAllMedicalCondition(): Observable<MedicalCondition[]> {
    return this.http.get<MedicalConditionDto[]>(this.url).pipe(
      map((dtos: MedicalConditionDto[]) => dtos.map(MedicalConditionMapper.toMedicalCondition))
    );
  }

    updateMedicalCondition(medicalConditionId: string, medicalCondition: MedicalConditionDto): Observable<MedicalCondition>{
      return this.http.put<MedicalConditionDto>(`${this.url}/${medicalConditionId}`, medicalCondition).pipe(
        map(MedicalConditionMapper.toMedicalCondition)
      );
    } 
}