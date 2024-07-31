import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Subscription, tap } from 'rxjs';
import { ConnectionService, ConnectionServiceOptions, ConnectionState } from 'ng-connection-service';
import { environment } from '../environments/environment';

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

  status!: string;
  currentState!: ConnectionState;
  subscription = new Subscription();

  constructor(private authService: AuthService,
    private connectionService: ConnectionService) { }

  ngOnInit(): void {
    this.authService.init();
    this.checkConnectionStatus();
  }
  
  checkConnectionStatus(): void {
    const options: ConnectionServiceOptions = {
      enableHeartbeat: true,
      heartbeatUrl: environment.baseUrl + 'api/heartbeat',
      heartbeatInterval: 30000
    };
    this.subscription.add(
      this.connectionService.monitor(options).pipe(
        tap((newState: ConnectionState) => {
          this.currentState = newState;

          if (this.currentState.hasNetworkConnection && this.currentState.hasInternetAccess) {
            this.status = 'ONLINE';

          } else {
            this.status = 'OFFLINE';

          }
          console.log('status', this.status);
        })
      ).subscribe()
    );
  }
}
