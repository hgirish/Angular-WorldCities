import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { CitiesComponent } from './cities/cities.component';
import { CountriesComponent } from './countries/countries.component';
import { CityEditComponent } from './cities/city-edit.component';
import { CountryEditComponent } from './countries/country-edit.component';
import { LoginComponent } from './auth/login.component';
import { authGuard } from './auth/auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'cities', component: CitiesComponent },
  { path: 'city/:id', component: CityEditComponent, canActivate: [authGuard] },
  { path: 'city', component: CityEditComponent, canActivate: [authGuard] },
  { path: 'countries', component: CountriesComponent },
  { path: 'country/:id', component: CountryEditComponent, canActivate: [authGuard] },
  { path: 'country', component: CountryEditComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
