import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DetailAllergyComponent } from './detail-allergy.component';

describe('DetailAllergyComponent', () => {
  let component: DetailAllergyComponent;
  let fixture: ComponentFixture<DetailAllergyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailAllergyComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DetailAllergyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
