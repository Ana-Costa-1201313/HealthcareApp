import { Component, OnInit } from '@angular/core';
import { AllergyService } from '../../services/allergy.service';
import { Allergy } from '../../model/allergy/allergy.model';
import { MenubarComponent } from '../menubar/menubar.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { TableModule } from 'primeng/table';
import { FilterMatchMode, Message, SelectItem } from 'primeng/api';
import { DetailAllergyComponent } from './detail-allergy/detail-allergy.component';
import { HttpErrorResponse } from '@angular/common/http';
import { CreateAllergyComponent } from './create-allergy/create-allergy.component';
import { EditAllergyComponent } from './edit-allergy/edit-allergy.component';

@Component({
  selector: 'app-allergy',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    DialogModule,
    ButtonModule,
    MessagesModule,
    MessageModule,
    FormsModule,
    ReactiveFormsModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    CalendarModule,
    MenubarComponent,
    DetailAllergyComponent,
    CreateAllergyComponent,
    EditAllergyComponent,
  ],
  templateUrl: './allergy.component.html',
  styleUrl: './allergy.component.css',
})
export class AllergyComponent implements OnInit {
  allergies: Allergy[] = [];
  message: Message[] = [];
  matchModeOptions: SelectItem[] = [];
  currentAllergy: Allergy = null;
  showDetails: boolean = false;
  showCreate: boolean = false;
  showEdit: boolean = false;
  role: string | null = null;

  constructor(private service: AllergyService) { }

  ngOnInit(): void {
    const session = this.getSession();
    this.role = session?.role || null;

    this.matchModeOptions = [
      { label: 'Contains', value: FilterMatchMode.CONTAINS },
    ];

    this.service
      .getAllergies()
      .subscribe((a: Allergy[]) => (this.allergies = a));
  }

  openDetailsModal(allergy: Allergy): void {
    this.currentAllergy = allergy;
    this.showDetails = true;
  }

  openCreateModal(): void {
    this.showCreate = true;
  }

  openEditModal(allergy: Allergy): void {
    this.currentAllergy = null;
    this.currentAllergy = allergy;
    this.showEdit = true;
  }

  onAdd() {
    this.service
      .getAllergies()
      .subscribe((a: Allergy[]) => (this.allergies = a));

    this.message = [
      {
        severity: 'success',
        summary: 'Success!',
        detail: 'Your Allergy was added with success',
      },
    ];
  }

  onEdit() {
    this.service
      .getAllergies()
      .subscribe((a: Allergy[]) => (this.allergies = a));

    this.message = [
      {
        severity: 'success',
        summary: 'Success!',
        detail: 'Your Allergy was edited with success',
      },
    ];
  }

  onFailure(error: HttpErrorResponse): void {
    if (error.status >= 500) {
      this.message = [
        { severity: 'error', summary: 'Failure!', detail: 'Server error' },
      ];
    } else {
      this.message = [
        { severity: 'error', summary: 'Failure!', detail: error.error.message },
      ];
    }
  }

  private getSession() {
    const session = sessionStorage.getItem('SessionUtilizadorInfo');
    return session ? JSON.parse(session) : null;
  }
}
