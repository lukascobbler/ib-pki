import {Component, ElementRef, ViewChild, NgZone} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {NgForOf, NgIf} from '@angular/common';
import {MatDialogRef} from '@angular/material/dialog';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {DateAdapter, MAT_DATE_FORMATS, MatNativeDateModule} from '@angular/material/core';
import {MatSelectModule} from '@angular/material/select';
import {MatIconModule} from '@angular/material/icon';
import {CUSTOM_DATE_FORMATS} from '../../../common/custom-components/custom-date/custom-date-formats';
import {CustomDateAdapter} from '../../../common/custom-components/custom-date/custom-date-adapter';

@Component({
  selector: 'app-edit-certificate-request-dialog',
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
  templateUrl: './edit-certificate-request-dialog.component.html',
  styleUrl: './edit-certificate-request-dialog.component.scss'
})
export class EditCertificateRequestDialogComponent {
  @ViewChild('scrollableDialog') scrollContainer!: ElementRef<HTMLDivElement>;
  extensions: { key: string, value: string, type: number }[] = [];
  dateNotBefore: Date | null = null;
  dateNotAfter: Date | null = null;

  constructor(
    public dialogRef: MatDialogRef<EditCertificateRequestDialogComponent, null>, public ngZone: NgZone) {
  }

  onNoClick() {
    this.dialogRef.close(undefined);
  }

  addExtension() {
    this.extensions.push({key: '', value: '', type: Math.round(Math.random())});
    const sub = this.ngZone.onStable.subscribe(() => {
      this.scrollToBottom();
      sub.unsubscribe();
    });
  }

  scrollToBottom() {
    const container = this.scrollContainer?.nativeElement;
    if (container) container.scrollTop = container.scrollHeight;
  }

  removeExtension(index: number) {
    this.extensions.splice(index, 1);
  }
}
