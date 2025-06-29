import { ComponentFixture, TestBed } from '@angular/core/testing';

import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { SpecializationService } from '../../../services/specialization.service';
import { StaffService } from '../../../services/staff.service';
import { StaffComponent } from '../staff.component';
import { DeleteStaffComponent } from './delete-staff.component';

describe('DeleteStaffComponent', () => {
  let component: DeleteStaffComponent;
  let fixture: ComponentFixture<DeleteStaffComponent>;
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

    fixture = TestBed.createComponent(DeleteStaffComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(StaffService);
    specService = fixture.debugElement.injector.get(SpecializationService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should deactivate staff: deactive should be false', () => {
    const staff = { id: '1' } as any;
    component.currentStaff = staff;

    spyOn(service, 'deactivateStaff').and.returnValue(of({} as any));

    component.deactivateStaff();

    expect(component.deactivate).toBeFalse();
  });

  it('should deactivate staff: deactivateStaff should been called 1 time', () => {
    const staff = { id: '1' } as any;
    component.currentStaff = staff;

    spyOn(service, 'deactivateStaff').and.returnValue(of({} as any));

    component.deactivateStaff();

    expect(service.deactivateStaff).toHaveBeenCalledTimes(1);
  });

  it('should not deactivate staff', () => {
    spyOn(service, 'deactivateStaff').and.returnValue(of({} as any));

    component.deactivateStaff();

    expect(service.deactivateStaff).toHaveBeenCalledTimes(0);
  });
});
