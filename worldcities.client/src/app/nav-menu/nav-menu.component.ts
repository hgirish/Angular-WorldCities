import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss'
})
export class NavMenuComponent implements OnInit, OnDestroy {
  private destroySubject = new Subject();
  isLoggedIn: boolean = false;

  constructor(private authService: AuthService,
    private router: Router) {
    this.authService.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
      })
  }
  onLogOut(): void {
    this.authService.logout();
    this.router.navigate(["/"]);
  }

    ngOnInit(): void {
      this.isLoggedIn = this.authService.isAuthenticated();
    }
    ngOnDestroy(): void {
      this.destroySubject.next(true);
      this.destroySubject.complete();
    }

}
