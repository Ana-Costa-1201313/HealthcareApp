import { Component, OnInit } from '@angular/core';
import { MenubarComponent } from '../menubar/menubar.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { HttpErrorResponse } from '@angular/common/http';
import { SelectItem, Message } from 'primeng/api';
import { RoomTypeDto } from '../../model/room/type/roomTypeDto.model';
import { RoomService } from '../../services/room.service';


import { FilterMatchMode } from 'primeng/api';
import { Button, ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { MessagesModule } from 'primeng/messages';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-room-types',
  standalone: true,
  imports: [  
              CardModule, 
              RouterModule, 
              CommonModule, 
              MenubarComponent,
              DialogModule,
              FormsModule,
              ButtonModule,
              MessagesModule,
              TableModule,
              Button
            ],
  templateUrl: './room-types.component.html',
  styleUrl: './room-types.component.css'
})
export class RoomTypesComponent {
  role: string | null = null;
  showLogoffButton: boolean = false;

  roomTypeList: RoomTypeDto[] = [];
  filteredRoomTypeList: RoomTypeDto[] = [];
  currentRoomType: RoomTypeDto | null = null;
  showDetails: boolean = false;
  showCreate: boolean = false;
  showEdit: boolean = false;
  newRoomType: Partial<RoomTypeDto> = {};
  editingRoomType: RoomTypeDto = {} as RoomTypeDto;
  showDeleteConfirm: boolean = false;
  message: Message[] = [];

  constructor(private roomService: RoomService) {
  }

  ngOnInit(): void {

    const session = this.getSession();
    this.role = session?.role || null;

    if (session == null) {
      window.location.href = "";
    }

    this.fetchRoomTypes(); // Fetch the data on component initialization


  }

  private getSession() {
    const session = sessionStorage.getItem('SessionUtilizadorInfo');
    return session ? JSON.parse(session) : null;
  }


  openCreateModal(): void {
    this.newRoomType = {};
    this.showCreate = true;
  }
  openDetailsModal(roomTypeDto: RoomTypeDto): void {
    this.currentRoomType = roomTypeDto;
    this.showDetails = true;
  }

  openEditModal(roomTypeDto: RoomTypeDto) {
    this.currentRoomType = { ...roomTypeDto };
    this.showEdit = true;
  }
  openDeleteModal(roomTypeDto: RoomTypeDto): void {
    this.currentRoomType = roomTypeDto;
  }
  onFailure(error: HttpErrorResponse): void {
    this.message = [{
      severity: 'error',
      summary: 'Failure!',
      detail: error.status >= 500 ? 'Server error' : error.error.message
    }];
  }


  fetchRoomTypes(): void {
    this.roomService.getRoomTypes().subscribe(
      (roomTypes: RoomTypeDto[]) => {
        this.roomTypeList = roomTypes; // Populate the full list
        this.filteredRoomTypeList = [...roomTypes]; // Initialize filtered list
      },
      (error) => this.onFailure(error) // Handle errors
    );
  }

  submitNewRoomType(): void {
    this.roomService.createRoomType(this.newRoomType).subscribe(
      (newRoomType) => {
        this.roomTypeList.push(newRoomType);
        this.filteredRoomTypeList = [...this.roomTypeList];
        this.showCreate = false;
 
        this.message = [{
          severity: 'success',
          summary: 'Success',
          detail: `${this.newRoomType.surgeryRoomType} type has been created`
        }];
      },
      (error) => this.onFailure(error)
    );
   }

}
