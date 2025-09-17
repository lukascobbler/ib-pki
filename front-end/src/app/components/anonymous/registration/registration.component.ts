import {Component} from '@angular/core';
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {NgForOf, NgIf, NgOptimizedImage} from "@angular/common";
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [
    MatFormField,
    MatInput,
    MatLabel,
    NgOptimizedImage,
    NgForOf,
    NgIf,
    FormsModule
  ],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.scss'
})
export class RegistrationComponent {
  password: string = '';

  get password_strength(): number {
    return Math.min(Math.max(0, this.password.length - 1), 4);
  }

  protected readonly Array = Array;
}
