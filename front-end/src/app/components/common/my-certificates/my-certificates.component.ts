import {Component, inject} from '@angular/core';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow, MatRowDef, MatTable
} from '@angular/material/table';
import {MatIconButton} from '@angular/material/button';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {RevokeCertificateDialogComponent} from '../revoke-certificate-dialog/revoke-certificate-dialog.component';
import {CertificateDetailsDialogComponent} from '../certificate-details-dialog/certificate-details-dialog.component';
import {DatePipe, NgIf} from '@angular/common';
import {Certificate} from '../../../models/Certificate';
import {CertificatesService} from '../../../services/certificates/certificates.service';
import {downloadFile} from '../blob/download-file';
import {AuthService} from '../../../services/auth/auth.service';
import {ToastrService} from '../toastr/toastr.service';
import {extractBlobError} from '../blob/extract-blob-error';

@Component({
  selector: 'app-my-certificates',
  standalone: true,
  imports: [
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderRow,
    MatHeaderRowDef,
    MatIconButton,
    MatRow,
    MatRowDef,
    MatTable,
    MatHeaderCellDef,
    DatePipe,
    NgIf
  ],
  templateUrl: './my-certificates.component.html',
  styleUrl: './my-certificates.component.scss'
})
export class MyCertificatesComponent {
  certificatesService = inject(CertificatesService);
  toast = inject(ToastrService);
  dialog = inject(MatDialog);
  auth = inject(AuthService);

  displayedColumns: string[] = [
    'issuedBy',
    'status',
    'validFrom',
    'validUntil',
    'serialNumber',
    'actions'
  ];

  certificatesDataSource: {issuedBy: string, status: string, validFrom: string, validUntil: string, serialNumber: string}[] = [
    {
      issuedBy: 'John',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
  ];

  openRevokeCertificate(certificate: Certificate) {
    const dialogRef: MatDialogRef<RevokeCertificateDialogComponent, null> = this.dialog.open(RevokeCertificateDialogComponent, {
      width: '30rem'
    });
  }

  openCertificateDetails(certificate: Certificate) {
    this.dialog.open(CertificateDetailsDialogComponent, {
      width: '850px',
      maxWidth: '70vw',
      data: { encodedCertificate: certificate.decryptedCertificate }
    });
  }

  downloadCertificate(certificate: Certificate) {
    this.certificatesService.downloadCertificate(certificate).subscribe({
      next: (blob: Blob) => {
        downloadFile(blob, `certificate_${certificate.prettySerialNumber}.pfx`)
      },
      error: async (err) => {
        const errorMessage = await extractBlobError(err);
        this.toast.error("Error", "Download failed: " + errorMessage);
      }
    });
  }

  downloadCertificateChain(certificate: Certificate) {
    this.certificatesService.downloadCertificateChain(certificate).subscribe({
      next: (blob: Blob) => {
        downloadFile(blob, `certificate_chain_${certificate.prettySerialNumber}.pfx`)
      },
      error: async (err) => {
        const errorMessage = await extractBlobError(err);
        this.toast.error("Error", "Download failed: " + errorMessage);
      }
    });
  }
}
