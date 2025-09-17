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
import {
  EditCertificateRequestDialogComponent
} from './edit-certificate-request-dialog/edit-certificate-request-dialog.component';

@Component({
  selector: 'app-certificate-requests',
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
    MatHeaderCellDef
  ],
  templateUrl: './certificate-requests.component.html',
  styleUrl: './certificate-requests.component.scss'
})
export class CertificateRequestsComponent {
  constructor(private dialog: MatDialog) {
  }

  displayedColumns: string[] = [
    'subject',
    'organization',
    'organizationUnit',
    'submittedOn',
    'fingerprint',
    'actions'
  ];

  certificateRequestsDataSource: {subject: string, organization: string, organizationUnit: string, submittedOn: string, fingerprint: string}[] = [
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },{
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },{
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
    {
      subject: 'Michael Scott',
      organization: 'Dunder Mifflin',
      organizationUnit: 'Sales',
      submittedOn: '2026-01-01',
      fingerprint: '9A:BC:DE:F1:23:45:67:89:AB:CD:AB:AC:67:89:AB:CD:AB:AC'
    },
  ];

  openEditCertificate() {
    const dialogRef: MatDialogRef<EditCertificateRequestDialogComponent, null> = this.dialog.open(EditCertificateRequestDialogComponent, {
      width: '700px',
      maxWidth: '80vw',
      autoFocus: false
    });
  }
}
