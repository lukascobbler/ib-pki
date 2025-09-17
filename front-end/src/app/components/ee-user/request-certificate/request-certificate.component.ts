import {Component} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {DateAdapter, MAT_DATE_FORMATS, MatNativeDateModule} from '@angular/material/core';
import {MatSelectModule} from '@angular/material/select';
import {NgForOf, NgIf} from '@angular/common';
import {MatTab, MatTabContent, MatTabGroup} from '@angular/material/tabs';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {KeysDialogComponent} from './keys-dialog/keys-dialog.component';
import {MatIconModule} from '@angular/material/icon';
import {CustomDateAdapter} from '../../common/custom-components/custom-date-adapter';
import {CUSTOM_DATE_FORMATS} from '../../common/custom-components/custom-date-formats';

@Component({
  selector: 'app-request-certificate',
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
    NgIf,
    MatTabGroup,
    MatTab,
    MatTabContent
  ],
  providers: [
    {provide: DateAdapter, useClass: CustomDateAdapter},
    {provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS}
  ],
  templateUrl: './request-certificate.component.html',
  styleUrl: './request-certificate.component.scss'
})
export class RequestCertificateComponent {
  extensions: { key: string, value: string, type: number }[] = [];
  isDragging = false;
  fileName: string | null = null;
  dateNotBeforeCSR: Date | null = null;
  dateNotBefore: Date | null = null;
  dateNotAfter: Date | null = null;

  constructor(private dialog: MatDialog) {
  }

  addExtension() {
    this.extensions.push({key: '', value: '', type: Math.round(Math.random())});
    const content = document.querySelector('.content');
    if (content) content.scrollTop = content.scrollHeight;
  }

  removeExtension(index: number) {
    this.extensions.splice(index, 1);
  }

  onFileChosen(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) this.fileName = file.name;
    // todo uraditi nesto sa fajlom
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
    const file = event.dataTransfer?.files[0];
    if (file) this.fileName = file.name;
    // todo uraditi nesto sa fajlom
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
  }

  removeFile(event: Event) {
    event.stopPropagation();
    this.fileName = null;
    // todo uraditi nesto sa fajlom
  }

  openCopyKeysDialog() {
    const dialogRef: MatDialogRef<KeysDialogComponent, null> = this.dialog.open(KeysDialogComponent, {
      width: '500px'
    });
  }
}
