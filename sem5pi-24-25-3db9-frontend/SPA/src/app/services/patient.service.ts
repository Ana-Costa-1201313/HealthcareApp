import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { map, Observable } from "rxjs";
import { Patient } from "../model/patient.model";
import { EditPatient } from "../model/editPatient.model";
import { DataDeletionPatientRequestDto } from "../model/dataDeletionPatientRequestDto.model";


@Injectable({ providedIn: 'root' })
export class PatientService {
    url = `${environment.apiUrl}${environment.endpoints.patient}`;

    constructor(private http: HttpClient) { }

    getPatientList(): Observable<Patient[]> {
        return this.http.get<Patient[]>(this.url);
    }
    createPatient(patient: Partial<Patient>): Observable<Patient> {
        return this.http.post<Patient>(this.url, patient);
    }

    updatePatient(id: string, patient: EditPatient): Observable<Patient> {
        return this.http.patch<Patient>(`${this.url}/${id}`, patient);
    }

    deletePatient(id: string): Observable<void> {
        return this.http.delete<void>(`${this.url}/${id}`);
    }

    getPatientByEmail(email: string): Observable<Patient> {
        return this.http.get<Patient[]>(`${this.url}/SearchByVariousAttributes/?email=${email}`)
            .pipe(map(patients => patients[0]));
    }

    getPatientMedicalHistoryJson(email: string): Observable<Blob> {
        return this.http.get(`${this.url}/download-medical-history-json/${email}`, {
            responseType: 'blob',
        });
    }

    getPatientMedicalHistoryXml(email: string): Observable<Blob> {
        return this.http.get(`${this.url}/download-medical-history-xml/${email}`, {
            responseType: 'blob',
        });
    }

    createPatientDeleteRequest(dto: DataDeletionPatientRequestDto): Observable<string> {
        return this.http.post(`${this.url}/request-data-deletion`, dto, { responseType: 'text' });
    }


}