import { Component } from '@angular/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {
  MatDatepicker,
  MatDatepickerInput,
  MatDatepickerModule,
  MatDatepickerToggle
} from '@angular/material/datepicker';
import {MatInput} from '@angular/material/input';
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MatNativeDateModule,
  MatOption,
} from '@angular/material/core';
import {MatSelect} from '@angular/material/select';
import {MatIconButton} from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {NgForOf, NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {CustomDateAdapter} from '../../shared/custom-calendar/custom-date-adapter';
import {CUSTOM_DATE_FORMATS} from '../../shared/custom-calendar/custom-date-formats';

@Component({
  selector: 'app-issue-certificate',
  standalone: true,
  imports: [
    MatFormField,
    MatLabel,
    MatDatepickerInput,
    MatDatepickerToggle,
    MatDatepicker,
    MatInput,
    MatOption,
    MatSelect,
    MatIconButton,
    NgForOf,
    MatIconModule,
    FormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    NgIf,
  ],
  providers: [
    {provide: DateAdapter, useClass: CustomDateAdapter},
    {provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS}
  ],
  templateUrl: './issue-certificate.component.html',
  styleUrl: './issue-certificate.component.scss'
})
export class IssueCertificateComponent {
  extensions: {key: string, value: string, type: number}[] = [];

  addExtension() {
    this.extensions.push({ key: '', value: '', type: Math.round(Math.random()) });
  }

  removeExtension(index: number) {
    this.extensions.splice(index, 1);
  }
}
