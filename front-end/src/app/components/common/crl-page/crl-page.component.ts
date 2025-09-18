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
import {MatDialog} from '@angular/material/dialog';
import {CertificateDetailsDialogComponent} from '../certificate-details-dialog/certificate-details-dialog.component';
import {Certificate} from '../../../models/Certificate';

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
    'serialNumber',
    'actions'
  ];

  certificatesDataSource: {issuedBy: string, issuedTo: string, revocationReason: string, serialNumber: string}[] = [

    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'keyCompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'affiliationChanged',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'privilegeWithdrawn',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
    {
      issuedBy: 'John Doe, IT, Example Corp, US',
      issuedTo: 'John Doe, IT, Example Corp, US',
      revocationReason: 'cACompromise',
      serialNumber: '9A:BC:1B:7C:43:64:G6:51:G2:BC:1B:7C:43:64:G6:51:G2'
    },
  ];

  openCertificateDetails(certificate: Certificate) {
    this.dialog.open(CertificateDetailsDialogComponent, {
      width: '700px',
      maxWidth: '70vw',
      data: { decryptedCertificate: certificate.decryptedCertificate }
    });
  }
}
