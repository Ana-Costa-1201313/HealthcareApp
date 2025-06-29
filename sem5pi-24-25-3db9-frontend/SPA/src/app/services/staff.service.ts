import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { EditStaffDto } from '../model/staff/editStaffDto';
import { Staff } from '../model/staff/staff.model';
import { CreateStaffDto } from '../model/staff/dto/createStaffDto';

@Injectable({ providedIn: 'root' })
export class StaffService {
  url = `${environment.apiUrl}${environment.endpoints.staff}`;

  constructor(private http: HttpClient) { }

  addStaff(staff: CreateStaffDto): Observable<Staff> {
    return this.http.post<Staff>(this.url, staff);
  }

  getStaffList(
    name: string,
    email: string,
    specialization: string,
    pageNum: number,
    pageSize: number
  ): Observable<Staff[]> {
    let params = new HttpParams();

    if (name) {
      params = params.append('name', name);
    }

    if (email) {
      params = params.append('email', email);
    }

    if (specialization) {
      params = params.append('specialization', specialization);
    }

    params = params.append('pageNum', pageNum);

    params = params.append('pageSize', pageSize);

    return this.http.get<Staff[]>(this.url, { params: params });
  }

  getTotalRecords(): Observable<number> {
    return this.http.get<number>(
      `${this.url}${environment.endpoints.totalRecordsStaff}`
    );
  }

  editStaff(staffId: string, staff: EditStaffDto): Observable<Staff> {
    return this.http.put<Staff>(`${this.url}/${staffId}`, staff);
  }

  deactivateStaff(staffId: string): Observable<Staff> {
    return this.http.delete<Staff>(`${this.url}/${staffId}`);
  }

  getStaffByEmail(email: string): Observable<Staff> {
    return this.http.get<Staff[]>(`${this.url}?email=${email}`)
      .pipe(map(staffs => staffs[0]));
  }
}
