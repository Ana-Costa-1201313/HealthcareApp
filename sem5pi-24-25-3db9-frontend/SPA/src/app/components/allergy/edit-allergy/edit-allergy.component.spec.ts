import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { SimpleChange } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AllergyService } from '../../../services/allergy.service';
import { AllergyComponent } from '../allergy.component';
import { EditAllergyComponent } from './edit-allergy.component';

describe('EditAllergyComponent', () => {
  let component: EditAllergyComponent;
  let fixture: ComponentFixture<EditAllergyComponent>;
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

    fixture = TestBed.createComponent(EditAllergyComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(AllergyService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should edit allergy', () => {
    spyOn(service, 'editAllergy').and.returnValue(of({} as any));

    component.editAllergy();

    expect(component.showEdit).toBeFalse();
  });

  it('should send edited allergy', () => {
    component.currentAllergy = {
      id: 'id',
      name: 'name',
      code: 'code',
      description: 'description',
    } as any;

    spyOn(service, 'editAllergy').and.returnValue(of({} as any));

    component.editAllergyForm.setValue({
      name: 'nameee',
      code: 'codeee',
      description: 'description',
    });

    const request = {
      ...component.editAllergyForm.value,
    };

    component.editAllergy();

    expect(service.editAllergy).toHaveBeenCalledOnceWith(
      component.currentAllergy.id,
      request
    );
  });

  it('should load on changes', () => {
    spyOn(component, 'ngOnChanges').and.callThrough();

    component.ngOnChanges({
      currentAllergy: new SimpleChange(null, component.currentAllergy, null),
    });

    expect(component.ngOnChanges).toHaveBeenCalled();
  });
});
