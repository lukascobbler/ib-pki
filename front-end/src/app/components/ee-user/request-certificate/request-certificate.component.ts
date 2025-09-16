import { Component } from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconButton} from '@angular/material/button';
import {MatInput} from '@angular/material/input';
import {MatOption} from '@angular/material/core';
import {MatSelect} from '@angular/material/select';
import {NgForOf, NgIf} from '@angular/common';
import {MatTab, MatTabContent, MatTabGroup} from '@angular/material/tabs';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {KeysDialogComponent} from './keys-dialog/keys-dialog.component';

@Component({
  selector: 'app-request-certificate',
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
    MatTabGroup,
    MatTab,
    MatTabContent
  ],
  templateUrl: './request-certificate.component.html',
  styleUrl: './request-certificate.component.scss'
})
export class RequestCertificateComponent {
  extensions: {key: string, value: string, type: number}[] = [];
  isDragging = false;
  fileName: string | null = null;

  constructor(private dialog: MatDialog) {
  }

  addExtension() {
    this.extensions.push({ key: '', value: '', type: Math.round(Math.random()) });
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
      width: '30rem'
    });
  }
}
