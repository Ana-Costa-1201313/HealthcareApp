import { Component , OnInit} from '@angular/core';
import { MenubarComponent } from '../menubar/menubar.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';

@Component({
    selector: 'app-rooms',
    standalone: true,
    imports: [CardModule, RouterModule, CommonModule, MenubarComponent],
    templateUrl: './rooms.component.html',
    styleUrl: './rooms.component.css'
})
export class RoomsComponent implements OnInit{
  role: string | null = null;
  showLogoffButton: boolean = false;

  constructor() {
  }

  ngOnInit(): void {

    const session = this.getSession();
    this.role = session?.role || null;

    if (session == null) {
      window.location.href = "";
    }

  }

  private getSession() {
    const session = sessionStorage.getItem('SessionUtilizadorInfo');
    return session ? JSON.parse(session) : null;
  }
}
