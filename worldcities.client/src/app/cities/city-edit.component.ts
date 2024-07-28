import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormControl, FormGroup, Validators } from '@angular/forms';
import { City } from './city';
import { ActivatedRoute, Router } from '@angular/router';
import { Country } from '../countries/country';
import {  Observable, Subject, Subscription } from 'rxjs';
import { map, takeUntil} from 'rxjs/operators'
import { BaseFormComponent } from '../base-form.component';
import { CityService } from './city.service';
@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrl: './city-edit.component.scss'
})
export class CityEditComponent
extends BaseFormComponent
  implements OnInit, OnDestroy {
  // the view title
  title?: string;



  // the city object to edit
  city?: City;

  // the city object id, as fetched from the active route:
  // it's null when we're adding a new city,
  // and not null when we're editing an existing one.
  id?: number;

  // the countries array for the select
  countries?: Observable< Country[]>;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private cityService: CityService
  ) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      lon: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),
      countryId: new FormControl('', Validators.required)
    },null,this.idDupeCity());

    
    this.form.get("name")!.valueChanges
      .pipe(takeUntil(this.destorySubject))
      .subscribe(() => {
      if (!this.form.dirty) {
        this.log("Name has been loaded with initial values");
      } else {
        this.log("Name was updated by the user.")
      }
    });

    this.form.valueChanges
      .pipe(takeUntil(this.destorySubject))
      .subscribe(val => {
        if (!this.form.dirty) {
          this.log("Form has been loaded");
        } else {
          this.log("Form was updated by the user.")
        }
      });

    this.loadData();
  }
    log(str: string) {
      console.log("["
        + new Date().toLocaleString()
        + "]" + str );
    }

  loadData() {
    // Load countries
    this.loadCountries();

    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
   
    if (this.id) {
      // EDIT MODE

      // fetch the city from the server
      this.cityService.get(this.id).subscribe({
        next: (result) => {
          this.city = result;
          this.title = 'Edit - ' + this.city.name;

          // update the form with the city value
          this.form.patchValue(this.city);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE

      this.title = "Create a new City";

    }

    
  }



  onSubmit() {
    var city = (this.id) ? this.city : <City>{};
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      if (this.id) {
        // EDTI MODE

        this.cityService
          .put( city)
          .subscribe({
            next: (result) => {
              console.log("City " + city!.id + " has been updated.");

              // go back to the cities view
              this.router.navigate(['/cities']);
            },
            error: (err) => console.error(err)
          })

      } else {
        // ADD NEW MODE
        this.cityService
          .post( city)
          .subscribe({
            next: (result) => {
              console.log("City " + result.id + " has been created");

              // go back to cities view
              this.router.navigate(['/cities']);
            },
            error: (err) => console.error(err)
          });
      }

    


    }
  }

  loadCountries() {
    // fetch all the countries from the server   

    this.countries = this.cityService
      .getCountries(0, 9999, "name", "asc", null, null)
      .pipe(map(x => x.data));
  }

  idDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = this.form.controls['lat'].value;
      city.lon = this.form.controls['lon'].value;
      city.countryId = this.form.controls['countryId'].value;

      return this.cityService.isDupeCity(city)
        .pipe(map(result => {
        return (result ? { isDupeCity: true } : null);
      }))

    }
  }
  private subscriptions: Subscription = new Subscription();
  
  ngOnDestroy() {
    this.subscriptions.unsubscribe();
    this.destorySubject.next(true)
    this.destorySubject.complete()
  }

  private destorySubject = new Subject();

}
