import { Component } from '@angular/core';
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatIconButton} from "@angular/material/button";
import {MatInput} from "@angular/material/input";
import {MatOption} from "@angular/material/core";
import {MatSelect} from "@angular/material/select";
import {MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-ca-add-certificate-dialog',
  standalone: true,
    imports: [
        MatFormField,
        MatIconButton,
        MatInput,
        MatLabel,
        MatOption,
        MatSelect
    ],
  templateUrl: './ca-add-certificate-dialog.component.html',
  styleUrl: './ca-add-certificate-dialog.component.scss'
})
export class CaAddCertificateDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<CaAddCertificateDialogComponent, null>) {
  }

  closeDialog() {
    this.dialogRef.close(null);
  }

  onNoClick() {
    this.dialogRef.close(undefined);
  }
}
