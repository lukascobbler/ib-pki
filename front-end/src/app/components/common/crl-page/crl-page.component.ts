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
import {CertificateDetailsDialogComponent} from '../certificate-details-dialog/certificate-details-dialog.component';

@Component({
  selector: 'app-crl-page',
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
  templateUrl: './crl-page.component.html',
  styleUrl: './crl-page.component.scss'
})
export class CrlPageComponent {
  constructor(private dialog: MatDialog) {
  }

  displayedColumns: string[] = [
    'issuedBy',
    'issuedTo',
    'revocationReason',
    'fingerprint',
    'actions'
  ];

  certificatesDataSource: {issuedBy: string, issuedTo: string, revocationReason: string, fingerprint: string}[] = [

    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      fingerprint: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
  ];

  openCertificateDetails() {
    const dialogRef: MatDialogRef<CertificateDetailsDialogComponent, null> = this.dialog.open(CertificateDetailsDialogComponent, {
      width: '700px',
      maxWidth: '70vw'
    });
  }
}
