import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Room } from '../model/room.model';
import { RoomTypeDto } from '../model/room/type/roomTypeDto.model';

@Injectable({ providedIn: 'root' })
export class RoomService {
    url = `${environment.apiUrl}${environment.endpoints.rooms}`;

    constructor(private http: HttpClient) { }

  getRoomList(): Observable<Room[]> {
    return this.http.get<Room[]>(this.url);
  }

  //mudar url's
  getRoomTypes(): Observable<RoomTypeDto[]> {
    return this.http.get<RoomTypeDto[]>(`${this.url}/getAllRoomTypeDto`);
  }

  createRoomType(roomType: Partial<RoomTypeDto>): Observable<RoomTypeDto> {
    const roomTypeDto: RoomTypeDto = {
      surgeryRoomType: roomType.surgeryRoomType,
      // fitForSurgery: roomType.fitForSurgery,
      //fitForSurgery: roomType.fitForSurgery === 'true' ? true : roomType.fitForSurgery, 
      fitForSurgery: roomType.fitForSurgery  ? true : false, 
      description: roomType.description
    };
    console.log(roomTypeDto);
    return this.http.post<RoomTypeDto>(`${this.url}/createRoomType`, roomTypeDto);
  }

  //mudar url's
  updateRoomType(id: number, roomType: RoomTypeDto): Observable<RoomTypeDto> {
    return this.http.put<RoomTypeDto>(`${this.url}/${id}`, roomType);
  }

  //mudar url's
  deleteRoomType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
