import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditStaffComponent } from './edit-staff.component';
import { SpecializationService } from '../../../services/specialization.service';
import { StaffService } from '../../../services/staff.service';
import { StaffComponent } from '../staff.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { SimpleChange } from '@angular/core';

describe('EditStaffComponent', () => {
  let component: EditStaffComponent;
  let fixture: ComponentFixture<EditStaffComponent>;
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

    fixture = TestBed.createComponent(EditStaffComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(StaffService);
    specService = fixture.debugElement.injector.get(SpecializationService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should edit staff', () => {
    spyOn(service, 'editStaff').and.returnValue(of({} as any));
    spyOn(component, 'addAvailabilitySlotToEdit');

    component.editStaffForm.get('phone').setValue('');

    component.editStaffForm
      .get('availabilitySlots')
      .setValue([[new Date(), new Date()]]);

    component.editStaff();

    expect(component.showEdit).toBeFalse();
  });

  it('should send edited staff', () => {
    component.currentStaff = {
      id: 'id',
      name: 'name',
      phone: '999999999',
      specialization: 'spec',
    } as any;

    spyOn(service, 'editStaff').and.returnValue(of({} as any));
    spyOn(component, 'addAvailabilitySlotToEdit');

    const date1 = new Date();
    const date2 = new Date();

    component.editStaffForm.setValue({
      phone: 999999993,
      specialization: 'spec2',
      availabilitySlots: [[date1, date2]],
    });

    const request = {
      id: 'id',
      ...component.editStaffForm.value,
      phone: component.editStaffForm.get('phone').value.toString(),

      availabilitySlots: [date1.toISOString() + '/' + date2.toISOString()],
    };

    component.editStaff();

    expect(service.editStaff).toHaveBeenCalledOnceWith(
      component.currentStaff.id,
      request
    );
  });

  it('should load on changes', () => {
    spyOn(component, 'ngOnChanges').and.callThrough();

    component.currentStaff = {
      availabilitySlots: ['2024-10-10T12:00:00.000Z/2024-10-11T15:00:00.000Z'],
    };

    component.ngOnChanges({
      currentStaff: new SimpleChange(null, component.currentStaff, null),
    });

    expect(component.ngOnChanges).toHaveBeenCalled();
  });
});
