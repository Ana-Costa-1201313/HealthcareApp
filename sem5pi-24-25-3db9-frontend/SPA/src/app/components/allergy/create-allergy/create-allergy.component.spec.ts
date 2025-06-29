import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AllergyService } from '../../../services/allergy.service';
import { AllergyComponent } from '../allergy.component';
import { CreateAllergyComponent } from './create-allergy.component';
import { of, throwError } from 'rxjs';

describe('CreateAllergyComponent', () => {
  let component: CreateAllergyComponent;
  let fixture: ComponentFixture<CreateAllergyComponent>;
  let service: AllergyService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllergyComponent],
      providers: [
        AllergyService,
        provideHttpClientTesting(),
        provideHttpClient(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateAllergyComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(AllergyService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should add allergy', () => {
    spyOn(service, 'addAllergy').and.returnValue(of({} as any));

    component.addAllergy();

    expect(component.showCreate).toBeFalse();
  });

  it('should send allergy', () => {
    spyOn(service, 'addAllergy').and.returnValue(of({} as any));

    component.createAllergyForm.setValue({
      name: 'name',
      code: 'code',
      description: 'description',
    });

    const request = {
      ...component.createAllergyForm.value,
    };

    component.addAllergy();

    expect(service.addAllergy).toHaveBeenCalledOnceWith(request);
  });

  it('should send error adding allergy', () => {
    spyOn(service, 'addAllergy').and.returnValue(
      throwError(() => {
        return { status: 400, error: { message: 'abc' } } as any;
      })
    );

    spyOn(component.onFailure, 'emit');

    component.addAllergy();

    expect(component.onFailure.emit).toHaveBeenCalledOnceWith({
      status: 400,
      error: { message: 'abc' },
    } as any);
  });
});
