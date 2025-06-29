import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OperationTypeService } from '../../services/operationType.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { OperationType } from '../../model/operationType/operationType.model';
import { of, throwError } from 'rxjs';
import { SpecializationService } from '../../services/specialization.service';
import { Specialization } from '../../model/specialization.model';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { MedicalconditionComponent } from './medicalcondition.component';
import { MedicalConditionService } from '../../services/medicalCondition.service';
import { MedicalCondition } from '../../model/medicalCondition/medicalCondition.model';


describe('MedicalConditionComponent', () => {
    let component: MedicalconditionComponent;
    let fixture: ComponentFixture<MedicalconditionComponent>;
    let service: MedicalConditionService;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MedicalconditionComponent],
            providers: [
                OperationTypeService,
                provideHttpClientTesting(),
                provideHttpClient(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(MedicalconditionComponent);
        component = fixture.componentInstance;
        service = fixture.debugElement.injector.get(MedicalConditionService);
    });

    it('should open medical condition details', () => {

        const medicalCondition: MedicalCondition = {
            id: 'id',
            name: 'name',
            code: 'code',
            description: 'desc',
            symptoms: []
        };

        component.openDetailsModal(medicalCondition);

        expect(component.currentMedicalCondition.name).toEqual('name');
        expect(component.currentMedicalCondition.code).toEqual('code');
        expect(component.currentMedicalCondition.description).toEqual('desc');
        expect(component.currentMedicalCondition.symptoms.length).toEqual(0);
        expect(component.showDetails).toBe(true);
    });

    it('should add medical condition', () => {
        spyOn(service, 'addMedicalCondition').and.returnValue(of({} as any));
        spyOn(component, 'addSymptoms');

        const mockMedicalConditionList: MedicalCondition[] = [
            {
                id: '1',
                name: 'name',
                code: 'code',
                description: 'desc',
                symptoms: []
            },
            {
                id: '2',
                name: 'name2',
                code: 'code2',
                description: 'desc2',
                symptoms: []
            },
        ];

        spyOn(service, 'getAllMedicalCondition').and.returnValue(of(mockMedicalConditionList));

        component.createMedicalConditionForm
            .get('symptoms');

        component.addMedicalCondition();

        expect(component.showCreate).toBeFalse();
    });

    it('should send medical condition', () => {
        spyOn(service, 'addMedicalCondition').and.returnValue(of({} as any));
        spyOn(component, 'addSymptoms').and.callThrough();

        const mockMedicalConditionList: MedicalCondition[] = [
            {
                id: '1',
                name: 'name',
                code: 'code',
                description: 'desc',
                symptoms: ['fever']
            },
        ];

        spyOn(service, 'getAllMedicalCondition').and.returnValue(of(mockMedicalConditionList));

        component.createMedicalConditionForm = new FormGroup({
            name: new FormControl('', Validators.required),
            code: new FormControl('', Validators.required),
            description: new FormControl('', Validators.required),
            symptoms: new FormArray([])
        });

        component.createMedicalConditionForm.setValue({
            name: 'name',
            code: 'code',
            description: 'desc',
            symptoms: []
        });

        component.addSymptoms();
        component.symptoms.at(0).get('symptomText')?.setValue("fever");

        const expectedRequest = {
            name: 'name',
            code: 'code',
            description: 'desc',
            symptoms: ['fever']
        };

        expect(component.createMedicalConditionForm.get('name')?.value).toEqual('name');
        expect(component.createMedicalConditionForm.get('code')?.value).toEqual('code');
        expect(component.createMedicalConditionForm.get('description')?.value).toEqual('desc');
        expect(component.symptoms.at(0).get('symptomText')?.value).toEqual('fever');

        component.addMedicalCondition();

        expect(service.addMedicalCondition).toHaveBeenCalledOnceWith(expectedRequest);

        expect(component.createMedicalConditionForm.get('name')?.value).toEqual(null);
        expect(component.createMedicalConditionForm.get('code')?.value).toEqual(null);
        expect(component.createMedicalConditionForm.get('description')?.value).toEqual(null);
        expect(component.symptoms.length).toEqual(0);
        expect(component.showCreate).toBeFalse();
    });

    it('should add symptoms while creating new medical condition', () => {
        component.addSymptoms();
        component.symptoms.at(0).get('symptomText')?.setValue('Fever');

        component.addSymptoms();
        component.symptoms.at(1).get('symptomText')?.setValue('Cough');

        component.addSymptoms();
        component.symptoms.at(2).get('symptomText')?.setValue('Headache');

        expect(component.symptoms.length).toBe(3);

        expect(component.symptoms.at(0).get('symptomText')?.value).toBe('Fever');
        expect(component.symptoms.at(1).get('symptomText')?.value).toBe('Cough');
        expect(component.symptoms.at(2).get('symptomText')?.value).toBe('Headache');
    });


    it('should remove symptoms while creating new medical condition', () => {
        component.addSymptoms();
        component.symptoms.at(0).get('symptomText')?.setValue('Fever');

        component.addSymptoms();
        component.symptoms.at(1).get('symptomText')?.setValue('Cough');

        expect(component.symptoms.length).toBe(2);

        expect(component.symptoms.at(0).get('symptomText')?.value).toBe('Fever');
        expect(component.symptoms.at(1).get('symptomText')?.value).toBe('Cough');

        component.removeSymptoms(0);

        expect(component.symptoms.length).toBe(1);

        expect(component.symptoms.at(0).get('symptomText')?.value).toBe('Cough');

        component.removeSymptoms(0);

        expect(component.symptoms.length).toBe(0);
    });

    it('should add symptoms while updating new medical condition', () => {
        expect(component.updateSymptoms.length).toBe(0);

        component.addUpdateSymptom();
        component.updateSymptoms.at(0).get('symptomText')?.setValue('Fever');

        expect(component.updateSymptoms.at(0).get('symptomText')?.value).toBe('Fever');
        expect(component.updateSymptoms.length).toBe(1);

        component.addUpdateSymptom();
        component.updateSymptoms.at(1).get('symptomText')?.setValue('Cough');

        expect(component.updateSymptoms.at(1).get('symptomText')?.value).toBe('Cough');
        expect(component.updateSymptoms.length).toBe(2);
    });

    it('should remove symptoms while updating new medical condition', () => {
        expect(component.updateSymptoms.length).toBe(0);

        component.addUpdateSymptom();
        component.updateSymptoms.at(0).get('symptomText')?.setValue('Fever');

        expect(component.updateSymptoms.at(0).get('symptomText')?.value).toBe('Fever');
        expect(component.updateSymptoms.length).toBe(1);

        component.addUpdateSymptom();
        component.updateSymptoms.at(1).get('symptomText')?.setValue('Cough');

        expect(component.updateSymptoms.at(1).get('symptomText')?.value).toBe('Cough');
        expect(component.updateSymptoms.length).toBe(2);

        component.removeUpdateSymptom(0);

        expect(component.updateSymptoms.length).toBe(1);
        expect(component.updateSymptoms.at(0).get('symptomText')?.value).toBe('Cough');

        component.removeUpdateSymptom(0);

        expect(component.updateSymptoms.length).toBe(0);
    });

    it('should update medical condition successfully', () => {
        spyOn(service, 'updateMedicalCondition').and.returnValue(of({} as any));
        spyOn(service, 'getAllMedicalCondition').and.returnValue(of([]));

        component.updateMedicalConditionForm = new FormGroup({
            name: new FormControl('Updated Name', Validators.required),
            code: new FormControl('Updated Code', Validators.required),
            description: new FormControl('Updated Desc', Validators.required),
            symptoms: new FormArray([])
        });

        component.updateSymptoms.push(new FormGroup({
            symptomText: new FormControl('Updated Fever')
        }));

        component.currentMedicalCondition = {
            id: '1',
            name: 'Old Name',
            code: 'Old Code',
            description: 'Old Desc',
            symptoms: ['Cough']
        };

        component.updateMedicalCondition();

        const expectedRequest = {
            name: 'Updated Name',
            code: 'Updated Code',
            description: 'Updated Desc',
            symptoms: ['Updated Fever']
        };

        expect(service.updateMedicalCondition).toHaveBeenCalledOnceWith('1', expectedRequest);
        expect(component.showUpdate).toBeFalse();
    });

    it('should update medical condition successfully', () => {
        spyOn(service, 'updateMedicalCondition').and.returnValue(of({} as any));
        spyOn(service, 'getAllMedicalCondition').and.returnValue(of([]));

        // Initialize the form with values
        component.updateMedicalConditionForm = new FormGroup({
            name: new FormControl('Updated Name', Validators.required),
            code: new FormControl('Updated Code', Validators.required),
            description: new FormControl('Updated Desc', Validators.required),
            symptoms: new FormArray([])
        });

        component.updateSymptoms.push(new FormGroup({
            symptomText: new FormControl('Updated Fever')
        }));

        component.currentMedicalCondition = {
            id: '1',
            name: 'Old Name',
            code: 'Old Code',
            description: 'Old Desc',
            symptoms: ['Cough']
        };

        component.updateMedicalCondition();

        const expectedRequest = {
            name: 'Updated Name',
            code: 'Updated Code',
            description: 'Updated Desc',
            symptoms: ['Updated Fever']
        };

        expect(service.updateMedicalCondition).toHaveBeenCalledOnceWith('1', expectedRequest);

        expect(component.updateMedicalConditionForm.get('name')?.value).toEqual('Updated Name');
        expect(component.updateMedicalConditionForm.get('code')?.value).toEqual('Updated Code');
        expect(component.updateMedicalConditionForm.get('description')?.value).toEqual('Updated Desc');
        expect(component.updateSymptoms.at(0).get('symptomText')?.value).toEqual('Updated Fever');

        expect(component.showUpdate).toBeFalse();

        expect(component.message).toEqual([
            {
                severity: 'success',
                summary: 'Success!',
                detail: 'Medical Condition updated successfully!'
            }
        ]);

        expect(service.getAllMedicalCondition).toHaveBeenCalled();
        expect(component.medicalConditionList.length).toBe(0);
    });

    it('should open the update modal with correct values', () => {
        const mockMedicalCondition: MedicalCondition = {
            id: '1',
            name: 'Old Name',
            code: 'Old Code',
            description: 'Old Desc',
            symptoms: ['Fever', 'Cough']
        };

        spyOn(component.updateMedicalConditionForm, 'reset');
        spyOn(component.updateSymptoms, 'clear');
        spyOn(component.updateMedicalConditionForm, 'patchValue');

        component.openUpdateModal(mockMedicalCondition);

        expect(component.currentMedicalCondition).toEqual(mockMedicalCondition);
        expect(component.showUpdate).toBeTrue();

        expect(component.updateMedicalConditionForm.reset).toHaveBeenCalled();
        expect(component.updateSymptoms.clear).toHaveBeenCalled();

        expect(component.updateMedicalConditionForm.patchValue).toHaveBeenCalledWith({
            name: 'Old Name',
            code: 'Old Code',
            description: 'Old Desc',
        });

        expect(component.updateSymptoms.length).toBe(2);
        expect(component.updateSymptoms.at(0).get('symptomText')?.value).toBe('Fever');
        expect(component.updateSymptoms.at(1).get('symptomText')?.value).toBe('Cough');
    });


});