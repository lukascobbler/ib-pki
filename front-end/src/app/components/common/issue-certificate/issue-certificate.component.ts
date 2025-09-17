import {Component} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {DateAdapter, MAT_DATE_FORMATS, MatNativeDateModule} from '@angular/material/core';
import {MatSelectModule} from '@angular/material/select';
import {NgForOf, NgIf} from '@angular/common';
import {MatIconModule} from '@angular/material/icon';
import {CustomDateAdapter} from '../custom-components/custom-date-adapter';
import {CUSTOM_DATE_FORMATS} from '../custom-components/custom-date-formats';

@Component({
  selector: 'app-issue-certificate',
  standalone: true,
  imports: [
    FormsModule,
    MatDatepickerModule,
    MatInputModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    NgForOf,
    NgIf
  ],
  providers: [
    {provide: DateAdapter, useClass: CustomDateAdapter},
    {provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS}
  ],
  templateUrl: './issue-certificate.component.html',
  styleUrl: './issue-certificate.component.scss'
})
export class IssueCertificateComponent {
  extensions: { key: string, value: string, type: number }[] = [];
  dateNotBefore: Date | null = null;
  dateNotAfter: Date | null = null;

  addExtension() {
    this.extensions.push({key: '', value: '', type: Math.round(Math.random())});
    const content = document.querySelector('.content');
    if (content) content.scrollTop = content.scrollHeight;
  }

  removeExtension(index: number) {
    this.extensions.splice(index, 1);
  }
}
