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
import {
  RevokeCertificateDialogComponent
} from '../../common/revoke-certificate-dialog/revoke-certificate-dialog.component';
import {
  CertificateDetailsDialogComponent
} from '../../common/certificate-details-dialog/certificate-details-dialog.component';
import {DatePipe} from '@angular/common';
import {Certificate} from '../../../models/Certificate';
import {downloadFile} from '../../common/download-file/download-file';
import {CertificatesService} from '../../../services/certificates/certificates.service';
import {ToastrService} from '../../common/toastr/toastr.service';

@Component({
  selector: 'app-signed-certificates',
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
    DatePipe
  ],
  templateUrl: './signed-certificates.component.html',
  styleUrl: './signed-certificates.component.scss'
})
export class SignedCertificatesComponent {
  certificatesService = inject(CertificatesService);
  toast = inject(ToastrService)
  dialog = inject(MatDialog);

  displayedColumns: string[] = [
    'issuedTo',
    'status',
    'validFrom',
    'validUntil',
    'serialNumber',
    'actions'
  ];

  signedCertificatesDataSource: {issuedTo: string, status: string, validFrom: string, validUntil: string, serialNumber: string}[] = [
    {
      issuedTo: 'John',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      serialNumber: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    }
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
      error: (err) => {
        this.toast.error("Error", "Download failed: ", err);
      }
    });
  }

  downloadCertificateChain(certificate: Certificate) {
    this.certificatesService.downloadCertificateChain(certificate).subscribe({
      next: (blob: Blob) => {
        downloadFile(blob, `certificate_chain_${certificate.prettySerialNumber}.pfx`)
      },
      error: (err) => {
        this.toast.error("Error", "Download failed: ", err);
      }
    });
  }
}
