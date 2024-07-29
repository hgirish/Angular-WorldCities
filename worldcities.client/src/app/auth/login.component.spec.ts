import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
import { AuthService } from './auth.service';
import { AngularMaterialModule } from '../angular-material.module';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from '../app-routing.module';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async () => {
    let authService = jasmine.createSpyObj<AuthService>('AuthService',['login']);

    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [
        AngularMaterialModule,
        ReactiveFormsModule,
        AppRoutingModule,
      ],
      providers: [
        {
          provide: AuthService,
          useValue: authService
        },
        provideAnimationsAsync(),
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
