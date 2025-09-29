import {Component, inject} from '@angular/core';
import {MatFormField} from '@angular/material/form-field';
import {MatIconButton} from '@angular/material/button';
import {MatOption} from '@angular/material/core';
import {MatSelect} from '@angular/material/select';
import {MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-revoke-certificate-dialog',
  standalone: true,
  imports: [
    MatFormField,
    MatIconButton,
    MatOption,
    MatSelect
  ],
  templateUrl: './revoke-certificate-dialog.component.html',
  styleUrl: './revoke-certificate-dialog.component.scss'
})
export class RevokeCertificateDialogComponent {
  dialogRef = inject(MatDialogRef<RevokeCertificateDialogComponent, null>);

  closeDialog() {
    this.dialogRef.close(null);
  }

  onNoClick() {
    this.dialogRef.close();
  }
}
