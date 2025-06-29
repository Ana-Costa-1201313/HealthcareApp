import { Component } from '@angular/core';
import { ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { FormsModule } from '@angular/forms';
import { Role } from '../../model/role.model';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule,DialogModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  userStatus: string | null = null;
  showPrivacyPolicy: boolean = false;

  constructor(
                private router: Router,
                private loginService: LoginService,
                private cdr: ChangeDetectorRef
  ) {
    if (this.getSession() != null) this.router.navigate(['/home']);
  }

  private getSession() {
    const session = sessionStorage.getItem('SessionUtilizadorInfo');
    return session ? JSON.parse(session) : null;
  }

  onSubmit() {
    // Handle login logic here
    console.log("Login with Email:", this.email, "and Password:", this.password);
    // Example: Update the session after successful login

    this.loginService.doLogin(this.email, this.password).subscribe(
      (response) => {
        //console.log('Login successful:', response);
        //console.log('Login successful:', response?.value);

        // Update userStatus based on the response
        this.userStatus = this.email || 'Guest';

        const sessionData = {
          username: response?.value?.username,
          loggedIn: true,
          role: response?.value?.role,
        };


        // expira quando é fechada a pagina correspondente
        sessionStorage.setItem('SessionUtilizadorInfo', JSON.stringify(sessionData));

        console.log(sessionStorage.getItem('SessionUtilizadorInfo'));

        this.cdr.detectChanges();

        this.setJwtCookie(response?.value?.jwt);

        console.log('Cookies:', document.cookie);

         //Redirect user to the home page
        //switch (response?.value?.role?.toUpperCase()) {
        //  case Role.Admin:
        //    this.router.navigate(['/dashboard'])

        //}
        this.router.navigate(['/home']);
      },
      (error) => {
        console.error('Login failed:', error);

        // Reset userStatus or show error message
        this.userStatus = 'Login Failed';
        alert('Login failed. Please check your credentials.');
      }
    );
    // Redirect user to home page (or dashboard)
    //this.router.navigate(['/']);
  }

  onRegister() {
    // Navigate to the registration page
    this.loginService.doRegister(this.email, this.email).subscribe(
      response => {
        console.log('Registration successful', response);
        // Navigate to the registration page or another view
        this.router.navigate(['/']);
      },
      error => {
        console.error('Registration failed', error);
        alert('Registration failed. Please check your credentials.');
        // Handle error appropriately
      }
    );
    this.router.navigate(['/']);
  }

  private setJwtCookie(jwt: string | null): void {
    // Set the expiration time to 5 hours from now
    const expirationDate = new Date();
    expirationDate.setTime(expirationDate.getTime() + (5 * 60 * 60 * 1000));  // 5 hours in milliseconds

    // Set the JWT token in a cookie with HttpOnly, Secure, SameSite, and expiry
    //document.cookie = `jwt=${jwt}; expires=${expirationDate.toUTCString()}; path=/; Secure; HttpOnly; SameSite=Strict`;
    document.cookie = `jwt=${jwt}; expires=${expirationDate.toUTCString()}; path=/; Secure; SameSite=Strict`;
  }

  private getCookie(name: string): string | undefined {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop()!.split(';').shift();
    return undefined;
  }
}
