import {Component, Inject} from '@angular/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconButton} from '@angular/material/button';
import {MatInput} from '@angular/material/input';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-certificate-details-dialog',
  standalone: true,
  imports: [
    MatFormField,
    MatIconButton,
    MatInput,
    MatLabel
  ],
  templateUrl: './certificate-details-dialog.component.html',
  styleUrl: './certificate-details-dialog.component.scss'
})
export class CertificateDetailsDialogComponent {
  decryptedCertificate = "";

  constructor(
    public dialogRef: MatDialogRef<CertificateDetailsDialogComponent, null>,
    @Inject(MAT_DIALOG_DATA) public data: { decryptedCertificate: string }
  ) {
    this.decryptedCertificate = data.decryptedCertificate;
  }

  onNoClick() {
    this.dialogRef.close(undefined);
  }
}
