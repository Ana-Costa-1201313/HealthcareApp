import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailStaffComponent } from './detail-staff.component';

describe('DetailStaffComponent', () => {
  let component: DetailStaffComponent;
  let fixture: ComponentFixture<DetailStaffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailStaffComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DetailStaffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create availability slots', () => {
    spyOn(component, 'createAvailabilitySlot').and.callThrough();

    component.showAvailabilitySlots(
      '2024-10-10T12:00:00.000Z/2024-10-11T15:00:00.000Z'
    );

    expect(component.createAvailabilitySlot).toHaveBeenCalled();
  });
});
