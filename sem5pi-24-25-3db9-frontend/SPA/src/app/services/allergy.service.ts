import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Allergy } from '../model/allergy/allergy.model';
import { CreateAllergyDto } from '../model/allergy/dto/createAllergyDto';
import { EditAllergyDto } from '../model/allergy/dto/editAllergyDto';
import { AllergyMapper } from '../model/allergy/allergyMapper';
import { AllergyDto } from '../model/allergy/dto/allergyDto';

@Injectable({ providedIn: 'root' })
export class AllergyService {
  url = `${environment.recordsBackofficeApiUrl}${environment.endpoints.allergies}`;

  constructor(private http: HttpClient) {}

  getAllergies(): Observable<Allergy[]> {
    return this.http
      .get<AllergyDto[]>(this.url)
      .pipe(map((dtos: AllergyDto[]) => dtos.map(AllergyMapper.toAllergy)));
  }

  addAllergy(allergy: CreateAllergyDto): Observable<Allergy> {
    return this.http
      .post<CreateAllergyDto>(this.url, allergy)
      .pipe(map(AllergyMapper.toAllergyCreate));
  }

  editAllergy(allergyId: string, allergy: EditAllergyDto): Observable<Allergy> {
    return this.http
      .put<EditAllergyDto>(`${this.url}/${allergyId}`, allergy)
      .pipe(map(AllergyMapper.toAllergyEdit));
  }
}
