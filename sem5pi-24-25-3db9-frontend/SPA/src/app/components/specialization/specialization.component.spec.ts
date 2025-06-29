import {
  ComponentFixture,
  fakeAsync,
  TestBed,
  tick,
} from '@angular/core/testing';

import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { SpecializationService } from '../../services/specialization.service';
import { Specialization } from '../../model/specialization.model';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { SpecializationComponent } from './specialization.component';

describe('SpecializationComponent', () => {
  let component: SpecializationComponent;
  let fixture: ComponentFixture<SpecializationComponent>;
  let service: SpecializationService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SpecializationComponent],
      providers: [
        SpecializationService,
        provideHttpClientTesting(),
        provideHttpClient(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SpecializationComponent);
    component = fixture.componentInstance;
    service = fixture.debugElement.injector.get(SpecializationService);
  });

  it('should open create modal', () => {
    component.openCreateModal();

    expect(component.showCreate).toBeTrue();
  });

  it('should add specialization', () => {
    spyOn(service, 'addSpecialization').and.returnValue(of({} as any));

    const mockSpecializationList: Specialization[] = [
      {
        name: 'SpecName',
        code: 'A100',
        description: 'DescriptionDescriptionDescription',
        active: true,
      },
    ];

    spyOn(service, 'getSpecializationList').and.returnValue(
      of(mockSpecializationList)
    );

    component.addSpecialization();

    expect(component.showCreate).toBeFalse();
  });

  it('should initialize with a list of specializations', () => {
    const mockSpecializationList: Specialization[] = [
      {
        name: 'SpecName',
        code: 'A100',
        description: 'DescriptionDescriptionDescription',
        active: true,
      },
    ];

    spyOn(service, 'getSpecializationList').and.returnValue(
      of(mockSpecializationList)
    );

    component.ngOnInit();

    expect(component.specializationList).toEqual(mockSpecializationList);
  });

  it('invalid form when fields are missing', () => {
    component.createSpecializationForm.controls['name'].setValue('');
    expect(component.createSpecializationForm.invalid).toBeTrue();
  });

  it('should display a server error message for 500 status codes', () => {
    const mockError: HttpErrorResponse = {
      status: 500,
      error: {},
    } as HttpErrorResponse;

    component.onFailure(mockError);

    expect(component.message).toEqual([
      {
        severity: 'error',
        summary: 'Failure!',
        detail: 'Server error',
      },
    ]);
  });

  it('should open details: selectedSpecialization equal to', () => {
    const spec: Specialization = {
      name: 'SpecName',
      code: 'A100',
      description: 'DescriptionDescriptionDescription',
      active: true,
    };

    component.openDetailsModal(spec);

    expect(component.selectedSpecialization).toEqual(spec);
  });

  it('should open details: showDetails to be True', () => {
    const spec: Specialization = {
      name: 'SpecName',
      code: 'A100',
      description: 'DescriptionDescriptionDescription',
      active: true,
    };

    component.openDetailsModal(spec);

    expect(component.showDetails).toBeTrue();
  });

  it('should load specializations: spec list equal to', () => {
    const list: Specialization[] = [{ id: '1' }, { id: '2' }] as any;

    spyOn(service, 'getSpecializationList').and.returnValue(of(list));

    component.ngOnInit();

    expect(component.specializationList).toEqual(list);
  });
});
