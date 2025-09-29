import {Component, inject} from '@angular/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconButton} from '@angular/material/button';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {CdkCopyToClipboard} from '@angular/cdk/clipboard';
import {MatInput} from '@angular/material/input';
import {KeyPair} from '../../../../models/KeyPair';

@Component({
  selector: 'app-keys-dialog',
  standalone: true,
  imports: [
    MatFormField,
    MatIconButton,
    CdkCopyToClipboard,
    MatLabel,
    MatInput
  ],
  templateUrl: './keys-dialog.component.html',
  styleUrl: './keys-dialog.component.scss'
})
export class KeysDialogComponent {
  dialogRef = inject(MatDialogRef<KeysDialogComponent, null>);
  keys = inject<KeyPair>(MAT_DIALOG_DATA);

  onNoClick() {
    this.dialogRef.close();
  }
}
