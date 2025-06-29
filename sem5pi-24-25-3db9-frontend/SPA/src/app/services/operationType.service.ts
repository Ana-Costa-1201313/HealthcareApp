import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OperationType } from '../model/operationType/operationType.model';
import { environment } from '../../environments/environment';
import { OperationTypeDto } from '../model/operationType/operationTypeDto.model';

@Injectable({ providedIn: 'root' })
export class OperationTypeService {
  url = `${environment.apiUrl}${environment.endpoints.operationTypes}`;

  constructor(private http: HttpClient) {}

  getOperationTypeList(): Observable<OperationType[]> {
    return this.http.get<OperationType[]>(this.url);
  }

  deactivateOperationType(opTypeId: string): Observable<OperationType>{
    return this.http.delete<OperationType>(`${this.url}/${opTypeId}`);
  }

  addOperationType(opType: OperationTypeDto): Observable<OperationType>{
    return this.http.post<OperationType>(this.url, opType);
  }

  updateOperationType(opType: OperationTypeDto): Observable<OperationType>{
    return this.http.put<OperationType>(`${this.url}/${opType.id}`, opType);
  } 
}