import {  HTTP_INTERCEPTORS, provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { CitiesComponent } from './cities/cities.component';
import { AngularMaterialModule } from './angular-material.module';
import { CountriesComponent } from './countries/countries.component';
import { ReactiveFormsModule } from '@angular/forms';
import { CityEditComponent } from './cities/city-edit.component';
import { CountryEditComponent } from './countries/country-edit.component';
import { AboutComponent } from './about/about.component';
import { LoginComponent } from './auth/login.component';
import { AuthInterceptor,  } from './auth/auth.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavMenuComponent,
    CitiesComponent,
    CountriesComponent,
    CityEditComponent,
    CountryEditComponent,
    AboutComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    AngularMaterialModule,
    ReactiveFormsModule,
  ],
  providers: [
    provideHttpClient(
      // DI-based interceptors must be explicitly enable
      withInterceptorsFromDi(),
    ),
    provideAnimationsAsync(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi:true
    }
    
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
