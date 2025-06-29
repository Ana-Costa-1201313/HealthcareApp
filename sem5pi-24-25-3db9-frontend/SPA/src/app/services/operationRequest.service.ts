import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OperationRequest } from '../model/operationRequest.model';
import { environment } from '../../environments/environment';
import { CreateOperationRequest } from '../model/createOperationRequest';
import { EditOperationRequest } from '../model/editOperationRequest.model';
import { CreatePickedOperationRequest } from '../model/createPickedOperationRequest';

@Injectable({ providedIn: 'root' })
export class OperationRequestService {
  url = `${environment.apiUrl}${environment.endpoints.operationRequests}`;

  constructor(private http: HttpClient) {}

  getOperationRequestList(): Observable<OperationRequest[]> {
    return this.http.get<OperationRequest[]>(this.url);
  }

  addOperationRequest(operationRequest: CreateOperationRequest): Observable<OperationRequest> {
    return this.http.post<OperationRequest>(this.url, operationRequest);
  }

  addPickedOperationRequest(operationRequest: CreatePickedOperationRequest): Observable<OperationRequest> {
    return this.http.post<OperationRequest>(this.url+"/picked", operationRequest);
  }

  getPickedOperationRequestList(): Observable<OperationRequest[]> {
    return this.http.get<OperationRequest[]>(this.url+"/list/?status=Picked");
  }

  deactivateOperationRequest(operationRequestId: string): Observable<OperationRequest>{
    return this.http.delete<OperationRequest>(`${this.url}/${operationRequestId}`);
  }

  updateOperationRequest(operationRequestId: string, editOperationRequest: EditOperationRequest): Observable<OperationRequest>{
    return this.http.patch<OperationRequest>(`${this.url}/${operationRequestId}`, editOperationRequest);
  }
}