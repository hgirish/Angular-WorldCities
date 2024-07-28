import { CitiesComponent } from "./cities.component";
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AngularMaterialModule } from '../angular-material.module';
import { Router, RouterModule } from "@angular/router";
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";
import { CityService } from "./city.service";
import { of } from "rxjs";
import { ApiResult, BaseService } from "../base.service";
import { City } from "./city";
import { provideHttpClient } from "@angular/common/http";


describe('CitiesComponent', () => {
  let component: CitiesComponent;
  let fixture: ComponentFixture<CitiesComponent>;
 // let cityService: CityService;
  beforeEach(async () => {

     let cityService = jasmine.createSpyObj<CityService>('CityService', ['getData']);

    // Configure the 'getData' spy method
    cityService.getData.and.returnValue(
      // return an Observable with some test data
      of<ApiResult<City>>(<ApiResult<City>>{
        data: [
          <City>{
            name: 'TestCity1',
            id: 1, lat: 1, lon: 1,
            countryId: 1, countryName: 'TestCountry1'
          },
          <City>{
            name: 'TestCity2',
            id: 2, lat: 1, lon: 1,
            countryId: 1, countryName: 'TestCountry1'
          },
          <City>{
            name: 'TestCity3',
            id: 3, lat: 1, lon: 1,
            countryId: 1, countryName: 'TestCountry1'
          }
        ],
        totalCount: 3,
        pageIndex: 0,
        pageSize: 10
      }));


    await TestBed.configureTestingModule({
      declarations: [
        CitiesComponent,
      ],
      imports: [
        AngularMaterialModule,
        RouterModule.forRoot([]),
      ],
      providers: [       
        {
          provide: CityService,
          useValue: cityService
        },
        provideHttpClient(),
        provideAnimationsAsync(),
        
      ]
    })
      .compileComponents();

    
   
  });
  beforeEach(() => {
    fixture = TestBed.createComponent(CitiesComponent);
    component = fixture.componentInstance;

    component.paginator = jasmine.createSpyObj(
      "MatPaginator",["length","pageIndex","pageSize"]
    )

    fixture.detectChanges();
  })

  it('should create', () => {
    expect(component).toBeTruthy();
    
  });

  it('should display a "Cities" title', () => {
    let title = fixture.nativeElement.querySelector('h1');
    expect(title.textContent).toEqual('Cities');
  })

  it('should contain a table with a list of one or more cities', () => {
    let table = fixture.nativeElement.querySelector('table.mat-mdc-table');

    let tableRows = table.querySelectorAll('tr.mat-mdc-row');
   
    expect(tableRows.length).toEqual(3);
  })
  // TODO: implement some other tests

});
