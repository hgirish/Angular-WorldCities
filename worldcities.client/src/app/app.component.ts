import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit  {
  title = 'World Cities';

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.init();
  }
}
