import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AllergyService } from '../../services/allergy.service';
import { AllergyComponent } from './allergy.component';
import { Allergy } from '../../model/allergy/allergy.model';
import { of } from 'rxjs';

describe('AllergyComponent', () => {
  let component: AllergyComponent;
  let fixture: ComponentFixture<AllergyComponent>;
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

    fixture = TestBed.createComponent(AllergyComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(AllergyService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should open details: currentAllergy equal to', () => {
    const allergy: Allergy = {
      id: 'id',
      name: 'name',
      code: 'code',
      description: 'description',
    };

    component.openDetailsModal(allergy);

    expect(component.currentAllergy).toEqual(allergy);
  });

  it('should open details: showDetails to be True', () => {
    const allergy: Allergy = {
      id: 'id',
      name: 'name',
      code: 'code',
      description: 'description',
    };

    component.openDetailsModal(allergy);

    expect(component.showDetails).toBeTrue();
  });

  it('should load staffs: staff list equal to', () => {
    const list: Allergy[] = [{ id: '1' }, { id: '2' }] as any;

    spyOn(service, 'getAllergies').and.returnValue(of(list));

    component.ngOnInit();

    expect(component.allergies).toEqual(list);
  });

  it('should open create modal', () => {
    component.openCreateModal();

    expect(component.showCreate).toBeTrue();
  });

  it('should send error 500', () => {
    const error: HttpErrorResponse = { status: 500 } as any;

    component.onFailure(error);

    expect(component.message).toEqual([
      { severity: 'error', summary: 'Failure!', detail: 'Server error' },
    ]);
  });

  it('should send error 400', () => {
    const error: HttpErrorResponse = {
      status: 400,
      error: { message: 'abc' },
    } as any;

    component.onFailure(error);

    expect(component.message).toEqual([
      { severity: 'error', summary: 'Failure!', detail: 'abc' },
    ]);
  });

  it('should open edit modal', () => {
    const allergy: Allergy = {
      id: 'id',
      name: 'name',
      code: 'code',
      description: 'description',
    };

    component.openEditModal(allergy);

    expect(component.showEdit).toBeTrue();
  });
});
