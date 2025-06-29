import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateStaffComponent } from './create-staff.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { Role } from '../../../model/role.model';
import { SpecializationService } from '../../../services/specialization.service';
import { StaffService } from '../../../services/staff.service';
import { StaffComponent } from '../staff.component';

describe('CreateStaffComponent', () => {
  let component: CreateStaffComponent;
  let fixture: ComponentFixture<CreateStaffComponent>;
  let service: StaffService;
  let specService: SpecializationService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaffComponent],
      providers: [
        StaffService,
        SpecializationService,
        provideHttpClientTesting(),
        provideHttpClient(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateStaffComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(StaffService);
    specService = fixture.debugElement.injector.get(SpecializationService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should add staff', () => {
    spyOn(service, 'addStaff').and.returnValue(of({} as any));
    spyOn(component, 'addSlot');

    component.createStaffForm.get('phone').setValue('');

    component.createStaffForm
      .get('availabilitySlots')
      .setValue([[new Date(), new Date()]]);

    component.addStaff();

    expect(component.showCreate).toBeFalse();
  });

  it('should send staff', () => {
    spyOn(service, 'addStaff').and.returnValue(of({} as any));
    spyOn(component, 'addSlot');

    const date1 = new Date();
    const date2 = new Date();

    component.createStaffForm.setValue({
      name: 'name',
      licenseNumber: 123,
      phone: 999999999,
      specialization: 'spec',
      availabilitySlots: [[date1, date2]],
      role: Role.Nurse,
      recruitmentYear: 2024,
    });

    const request = {
      ...component.createStaffForm.value,
      phone: component.createStaffForm.get('phone').value.toString(),

      availabilitySlots: [date1.toISOString() + '/' + date2.toISOString()],

      recruitmentYear: new Date(
        component.createStaffForm.get('recruitmentYear').value
      ).getFullYear(),
    };

    component.addStaff();

    expect(service.addStaff).toHaveBeenCalledOnceWith(request);
  });

  it('should send error adding staff', () => {
    spyOn(service, 'addStaff').and.returnValue(
      throwError(() => {
        return { status: 400, error: { message: 'abc' } } as any;
      })
    );

    component.createStaffForm.get('phone').setValue('');

    component.createStaffForm
      .get('availabilitySlots')
      .setValue([[new Date(), new Date()]]);

    spyOn(component.onFailure, 'emit');

    component.addStaff();

    expect(component.onFailure.emit).toHaveBeenCalledOnceWith({
      status: 400,
      error: { message: 'abc' },
    } as any);
  });

  it('should add slot', () => {
    component.addSlot();

    expect(
      component.createStaffForm.controls.availabilitySlots.controls.length
    ).toBe(2);
  });
});
