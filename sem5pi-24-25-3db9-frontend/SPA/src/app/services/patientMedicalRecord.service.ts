import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { PatientMedicalRecord } from "../model/patientMedicalRecord/patientMedicalRecord.model";
import { catchError, map, Observable, of, throwError } from "rxjs";
import { PatientMedicalRecordDto } from "../model/patientMedicalRecord/patientMedicalRecordDto.model";
import { PatientMedicalRecordMapper } from "../model/patientMedicalRecord/patientMedicalRecordMapper.model";

@Injectable({providedIn: 'root'})
export class PatientMedicalRecordService{
    url = `${environment.recordsBackofficeApiUrl}${environment.endpoints.patientMedicalRecord}`;
    constructor(private http: HttpClient) {}

    getMedicalRecords(): Observable<PatientMedicalRecord[]>{
        return this.http.get<PatientMedicalRecordDto[]>(this.url).pipe(
            map((dtos:PatientMedicalRecordDto[]) => dtos.map(PatientMedicalRecordMapper.toMedicalRecord))
        );
    }

    createMedicalRecord(record: PatientMedicalRecord): Observable<PatientMedicalRecord> {
        const dto = PatientMedicalRecordMapper.toDto(record);
        return this.http.post<PatientMedicalRecordDto>(this.url, dto).pipe(
          map(PatientMedicalRecordMapper.toMedicalRecord)
        );
      }

      updateMedicalRecordAllergies(patientId: string, allergies: string[]): Observable<PatientMedicalRecord> {
        const url = `${this.url}/${patientId}/allergies`;
        return this.http.put<PatientMedicalRecordDto>(url, {allergies }).pipe(
          map(PatientMedicalRecordMapper.toMedicalRecord)
        );
      }

      updateMedicalRecordMedicalConditions(patientId: string, medicalConditions: string[]): Observable<PatientMedicalRecord> {
        const url = `${this.url}/${patientId}/medical-conditions`;
        return this.http.put<PatientMedicalRecordDto>(url, {medicalConditions }).pipe(
          map(PatientMedicalRecordMapper.toMedicalRecord)
        );
      }

      updateMedicalRecordFamilyHistory(patientId: string, familyHistory: string[]): Observable<PatientMedicalRecord> {
        const url = `${this.url}/${patientId}/family-history`;
        return this.http.put<PatientMedicalRecordDto>(url, {familyHistory }).pipe(
          map(PatientMedicalRecordMapper.toMedicalRecord)
        );
      }

      updateMedicalRecordFreeText(patientId: string, freeText: string[]): Observable<PatientMedicalRecord> {
        const url = `${this.url}/${patientId}/free-text`;
        return this.http.put<PatientMedicalRecordDto>(url, {freeText }).pipe(
          map(PatientMedicalRecordMapper.toMedicalRecord)
        );
      }

      getMedicalRecordMedicalConditions(recordId: string): Observable<string[]> {
        const url = `${this.url}/${recordId}/medical-conditions`; 
        return this.http.get<string[]>(url);
    }
    
    getMedicalRecordAllergies(recordId: string): Observable<string[]> {
        const url = `${this.url}/${recordId}/allergies`;
        return this.http.get<string[]>(url);
    }

    getMedicalRecordFamilyHistory(recordId: string): Observable<string[]> {
      const url = `${this.url}/${recordId}/family-history`;
      return this.http.get<string[]>(url);
    }

    getMedicalRecordFreeText(recordId: string): Observable<string[]> {
      const url = `${this.url}/${recordId}/free-text`;
      return this.http.get<string[]>(url);
    }

    getMedicalRecordByEmail(email: string): Observable<PatientMedicalRecord>{
      return this.http.get<PatientMedicalRecordDto>(this.url+"/"+email).pipe(
          map(PatientMedicalRecordMapper.toMedicalRecord));
    }

}