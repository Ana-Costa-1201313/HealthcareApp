import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginDTO } from '../model/loginDTO.model';


@Injectable({ providedIn: 'root' })
export class LoginService {
  url = `${environment.authApiUrl}${environment.endpoints.login}`;
  apiUrl = `${environment.apiUrl}${environment.endpoints.registerNewPatientUser}`;

  constructor(private http: HttpClient) { }

  doLogin(
    email: string,
    password: string
  ): Observable<LoginDTO> {

    const body = {
      username: email,
      password: password
    };

    const boas = this.http.post<LoginDTO>(this.url, body, {
      headers: { 'Content-Type': 'application/json' }
    })
    return boas;
  }

  doRegister(email: string, confirmationEmail: string): Observable<any> {
    const payload = {
      Email: email,
      Role: 'Patient',
      ConfirmationEmail: email
    };
    return this.http.post(this.apiUrl, payload);
  }
}
