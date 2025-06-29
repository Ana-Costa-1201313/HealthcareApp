import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
import { Allergy } from '../../../model/allergy/allergy.model';

@Component({
  selector: 'app-detail-allergy',
  standalone: true,
  imports: [DialogModule, CommonModule],
  templateUrl: './detail-allergy.component.html',
})
export class DetailAllergyComponent {
  @Input() currentAllergy: Allergy = null;
  @Input() showDetails: boolean = false;
  @Output() showDetailsChange = new EventEmitter<boolean>();
}
