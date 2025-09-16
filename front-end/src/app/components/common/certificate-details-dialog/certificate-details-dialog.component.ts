import {Component, Input} from '@angular/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconButton} from '@angular/material/button';
import {MatInput} from '@angular/material/input';
import {MatDialogRef} from '@angular/material/dialog';

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
  @Input() certificateDetails: string = "Subject DN: CN=John Doe, O=Example Corp, C=US\n" +
    "Issuer DN: CN=Example CA, O=Example Org, C=US\n" +
    "Serial Number: 1234567890\n" +
    "Version: 3\n" +
    "Signature Algorithm: SHA256withRSA\n" +
    "Valid From: 2025-01-01\n" +
    "Valid To: 2026-01-01\n" +
    "Public Key: RSA 2048\n" +
    "Fingerprint (SHA-256): AB:CD:EF:12:34:56:78:9A:BC:DE:F0:12:34:56:78:9A:BC:DE:F1:23:45:67:89:AB:CD:EF:01:23:45:67:89\n" +
    "Key Usage: Digital Signature, Key Encipherment\n" +
    "Extended Key Usage: Client Auth, Server Auth\n" +
    "Subject Alternative Names: DNS:www.example.com, DNS:example.com, Email:user@example.com\n" +
    "Basic Constraints: CA: FALSE\n" +
    "CRL Distribution Points: http://example.com/crl\n" +
    "Authority Key Identifier: 01:23:45:67:89:AB:CD:EF:01:23:45:67:89:AB:CD:EF\n" +
    "Authority Information Access:\n" +
    "    OCSP - URI:http://ocsp.example.com\n" +
    "    CA Issuers - URI:http://example.com/cacert.pem\n"

  constructor(
    public dialogRef: MatDialogRef<CertificateDetailsDialogComponent, null>) {
  }

  onNoClick() {
    this.dialogRef.close(undefined);
  }
}
