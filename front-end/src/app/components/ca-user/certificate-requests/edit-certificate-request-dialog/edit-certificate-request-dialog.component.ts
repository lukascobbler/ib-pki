import { Component } from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconButton} from '@angular/material/button';
import {MatInput} from '@angular/material/input';
import {MatOption} from '@angular/material/core';
import {MatSelect} from '@angular/material/select';
import {NgForOf, NgIf} from '@angular/common';
import {MatDialogContent, MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-edit-certificate-request-dialog',
  standalone: true,
  imports: [
    FormsModule,
    MatDatepicker,
    MatDatepickerInput,
    MatDatepickerToggle,
    MatFormField,
    MatIconButton,
    MatInput,
    MatLabel,
    MatOption,
    MatSelect,
    NgForOf,
    NgIf,
    ReactiveFormsModule,
    MatDialogContent
  ],
  templateUrl: './edit-certificate-request-dialog.component.html',
  styleUrl: './edit-certificate-request-dialog.component.scss'
})
export class EditCertificateRequestDialogComponent {
  extensions: {key: string, value: string, type: number}[] = [];

  constructor(
    public dialogRef: MatDialogRef<EditCertificateRequestDialogComponent, null>) {
  }

  closeDialog() {
    this.dialogRef.close(null);
  }

  onNoClick() {
    this.dialogRef.close(undefined);
  }

  addExtension() {
    this.extensions.push({ key: '', value: '', type: Math.round(Math.random()) });
  }

  removeExtension(index: number) {
    this.extensions.splice(index, 1);
  }
}
