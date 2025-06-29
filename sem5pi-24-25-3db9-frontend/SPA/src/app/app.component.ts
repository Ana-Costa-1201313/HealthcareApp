import { Component , OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'SPA';

  ngOnInit(): void {
    // Check if 'session' already exists in localStorage
    //const session = localStorage.getItem('session');

    // If no session is found, set a default value
    //if (!session) {
    //  const defaultSession = {
    //    email: 'guest@example.com',
    //    loggedIn: false,
    //    role: 'guest'
    //  };
    //  localStorage.setItem('session', JSON.stringify(defaultSession));

    //} else {
      // Optionally: you can parse the session and perform additional checks if needed
      //const parsedSession = JSON.parse(session);
      //console.log('Session info:', parsedSession);
    //}
  }
}
