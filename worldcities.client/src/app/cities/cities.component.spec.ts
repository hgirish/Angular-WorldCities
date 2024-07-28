import { CitiesComponent } from "./cities.component";
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AngularMaterialModule } from '../angular-material.module';
import { RouterModule } from "@angular/router";
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";

describe('CitiesComponent', () => {
  let component: CitiesComponent;
  let fixture: ComponentFixture<CitiesComponent>;

  beforeEach(async () => {
    // TODO declare & initialize providers

    await TestBed.configureTestingModule({
      declarations: [CitiesComponent],
      imports: [
        
        AngularMaterialModule,
        
      ],
      providers: [
        RouterModule.forRoot([]),
        provideAnimationsAsync(),        
      ]
    })
      .compileComponents();
  });
  beforeEach(() => {
    fixture = TestBed.createComponent(CitiesComponent);
    component = fixture.componentInstance;

    // TODO: configure fixture/component/childeren/etc

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
