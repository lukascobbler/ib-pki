import {Component, Inject} from '@angular/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatOption, MatSelect} from '@angular/material/select';
import {MAT_DIALOG_DATA, MatDialogClose, MatDialogRef} from '@angular/material/dialog';
import {FormsModule} from '@angular/forms';
import {MatInput} from '@angular/material/input';
import {NgIf} from '@angular/common';
import {MatIconButton} from '@angular/material/button';

@Component({
  selector: 'app-ca-registration-dialog',
  standalone: true,
  imports: [
    MatFormField,
    MatSelect,
    MatOption,
    MatDialogClose,
    FormsModule,
    MatInput,
    MatLabel,
    NgIf,
    MatIconButton
  ],
  templateUrl: './ca-registration-dialog.component.html',
  styleUrl: './ca-registration-dialog.component.scss'
})
export class CaRegistrationDialogComponent {

  constructor(
    public dialogRef: MatDialogRef<CaRegistrationDialogComponent, null>) {
  }

  closeDialog() {
    this.dialogRef.close(null);
  }

  onNoClick() {
    this.dialogRef.close(undefined);
  }
}
