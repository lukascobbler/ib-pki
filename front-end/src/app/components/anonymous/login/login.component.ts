import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgOptimizedImage, NgIf } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule, FormGroup } from '@angular/forms';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import {AuthService} from "../../../services/auth/auth.service";
import {ToastrService} from "../../common/toastr/toastr.service";


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [NgOptimizedImage, MatFormField, MatInput, MatLabel, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loading = false;
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toast: ToastrService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  submit() {
    console.log('hello')
    if (this.form.invalid) {
      this.toast.info('Check fields', 'Please enter a valid email and password.');
      return;
    }

    const { email, password } = this.form.value as { email: string; password: string };

    this.loading = true;
    this.auth.login({ email, password }).subscribe({
      next: (user) => {
        this.toast.success('Welcome', 'You are now logged in.');

        const returnUrl = new URLSearchParams(location.search).get('returnUrl');
        const target = returnUrl ?? this.defaultRouteForRole(user.role);

        this.router.navigateByUrl(target);
        this.loading = false
      },
      error: (err) => {
        const msg = this.extractError(err);
        this.toast.error('Login failed', msg);
        this.loading = false;
      },
    });
  }

  goRegister() {
    this.router.navigate(['/register']);
  }

  private defaultRouteForRole(role: string): string {
    switch (role) {
      case 'EeUser': return '/my-certificates';
      case 'CaUser': return '/signed-certificates';
      case 'Admin':  return '/all-certificates';
      default:       return '/';
    }
  }

  private extractError(err: any): string {
    const msg =
      err?.error?.error ||
      err?.error?.message ||
      err?.message ||
      'Unexpected error. Please try again.';
    return typeof msg === 'string' ? msg : 'Unexpected error. Please try again.';
  }
}