import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { MenubarComponent } from '../menubar/menubar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CardModule, RouterModule, CommonModule, MenubarComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  role: string | null = null;
  showLogoffButton: boolean = false;

  constructor() {
  }

  ngOnInit(): void {

    const session = this.getSession();
    this.role = session?.role || null;
    
    if(session == null){
      window.location.href="";
    }

  }

  private getSession() {
    const session = sessionStorage.getItem('SessionUtilizadorInfo');
    return session ? JSON.parse(session) : null;
  }
}
