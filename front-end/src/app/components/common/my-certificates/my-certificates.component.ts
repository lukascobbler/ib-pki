import { Component } from '@angular/core';
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
import {DatePipe} from '@angular/common';

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
    DatePipe
  ],
  templateUrl: './my-certificates.component.html',
  styleUrl: './my-certificates.component.scss'
})
export class MyCertificatesComponent {
  constructor(private dialog: MatDialog) {
  }

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
