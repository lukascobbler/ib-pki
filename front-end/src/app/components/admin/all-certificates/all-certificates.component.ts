import { Component } from '@angular/core';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable
} from '@angular/material/table';
import {MatIconButton} from '@angular/material/button';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {
  RevokeCertificateDialogComponent
} from '../../common/revoke-certificate-dialog/revoke-certificate-dialog.component';
import {
  CertificateDetailsDialogComponent
} from '../../common/certificate-details-dialog/certificate-details-dialog.component';

@Component({
  selector: 'app-all-certificates',
  standalone: true,
  imports: [
    MatTable,
    MatHeaderCell,
    MatColumnDef,
    MatHeaderCellDef,
    MatCell,
    MatCellDef,
    MatIconButton,
    MatHeaderRow,
    MatRow,
    MatRowDef,
    MatHeaderRowDef
  ],
  templateUrl: './all-certificates.component.html',
  styleUrl: './all-certificates.component.scss'
})
export class AllCertificatesComponent {
  constructor(private dialog: MatDialog) {
  }

  displayedColumns: string[] = [
    'issuedBy',
    'issuedTo',
    'status',
    'validFrom',
    'validUntil',
    'fingerprint',
    'actions'
  ];

  certificatesDataSource: {issuedBy: string, issuedTo: string, status: string, validFrom: string, validUntil: string, fingerprint: string}[] = [

    {
      issuedBy: 'John',
      issuedTo: 'John',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },{
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      status: 'Active',
      validFrom: '2024-01-01',
      validUntil: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:9A:BC:DE:F1:23:45:67:89'
    },
  ];

  openRevokeCertificate() {
    const dialogRef: MatDialogRef<RevokeCertificateDialogComponent, null> = this.dialog.open(RevokeCertificateDialogComponent, {
      width: '30rem'
    });
  }

  openCertificateDetails() {
    const dialogRef: MatDialogRef<CertificateDetailsDialogComponent, null> = this.dialog.open(CertificateDetailsDialogComponent, {
      width: '700px',
      maxWidth: '70vw'
    });
  }
}
