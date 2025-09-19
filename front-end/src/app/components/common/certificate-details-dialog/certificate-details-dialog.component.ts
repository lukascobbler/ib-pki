import {Component, inject} from '@angular/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconButton} from '@angular/material/button';
import {MatInput} from '@angular/material/input';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {ToastrService} from "../toastr/toastr.service";

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
  dialogRef = inject(MatDialogRef<CertificateDetailsDialogComponent, null>);
  data = inject(MAT_DIALOG_DATA) as { decryptedCertificate: string };
  decryptedCertificate = this.data.decryptedCertificate;
  toastr = inject(ToastrService);

  onNoClick() {
    this.dialogRef.close(undefined);
  }

  copyCertificate() {
    if (!this.decryptedCertificate) return;
    navigator.clipboard.writeText(this.decryptedCertificate)
      .then(() => this.toastr.success('Success', 'Certificate copied to clipboard'))
      .catch(() => this.toastr.error('Error', `Unable to copy certificate: {err}`));
  }
}
