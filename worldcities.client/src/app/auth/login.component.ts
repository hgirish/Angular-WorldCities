import { Component, OnInit } from '@angular/core';
import { BaseFormComponent } from '../base-form.component';
import { LoginResult } from './login-result';
import { AuthService } from './auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginRequest } from './login-request';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent
  extends BaseFormComponent
 implements OnInit {

  title?: string;
  loginResult?: LoginResult;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }

  onSubmit() {
    var loginRequest = <LoginRequest>{};
    loginRequest.email = this.form.controls['email'].value;
    loginRequest.password = this.form.controls['password'].value;

    this.authService
      .login(loginRequest)
      .subscribe({
        next: (result) => {
          console.log(result);
          this.loginResult = result;
          if (result.success) {
            this.router.navigate(["/"]);
          }
        },
        error: (err) => {
          console.log(err);
          if (err.status == 401) {
            this.loginResult = err.error;
          }
        }
      });

  }

}
