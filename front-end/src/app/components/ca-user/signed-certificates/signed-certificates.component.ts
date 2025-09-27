import {Component, inject, OnInit} from '@angular/core';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow, MatRowDef, MatTable, MatTableDataSource
} from '@angular/material/table';
import {MatIconButton} from '@angular/material/button';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {
  RevokeCertificateDialogComponent
} from '../../common/revoke-certificate-dialog/revoke-certificate-dialog.component';
import {
  CertificateDetailsDialogComponent
} from '../../common/certificate-details-dialog/certificate-details-dialog.component';
import {DatePipe, NgIf} from '@angular/common';
import {Certificate} from '../../../models/Certificate';
import {downloadFile} from '../../common/custom-components/blob/download-file';
import {CertificatesService} from '../../../services/certificates/certificates.service';
import {ToastrService} from '../../common/toastr/toastr.service';
import {extractBlobError} from '../../common/custom-components/blob/extract-blob-error';
import {AuthService} from '../../../services/auth/auth.service';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {RevokeCertificate} from '../../../models/RevokeCertificate';
import {CrlService} from '../../../services/crl/crl.service';

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
    DatePipe,
    MatProgressSpinner,
    NgIf
  ],
  templateUrl: './signed-certificates.component.html',
  styleUrl: './signed-certificates.component.scss'
})
export class SignedCertificatesComponent implements OnInit {
  certificatesService = inject(CertificatesService);
  toast = inject(ToastrService);
  dialog = inject(MatDialog);
  auth = inject(AuthService);
  crlService = inject(CrlService);

  signedByMeCertificates: Certificate[] = [];
  loading = true;
  signedCertificatesDataSource = new MatTableDataSource();

  displayedColumns: string[] = [
    'issuedTo',
    'status',
    'validFrom',
    'validUntil',
    'serialNumber',
    'actions'
  ];

  ngOnInit() {
    this.certificatesService.getCertificatesSignedByMe().subscribe({
      next: value => {
        this.signedByMeCertificates = value;
        this.signedCertificatesDataSource.data = this.signedByMeCertificates;
        this.loading = false;
      },
      error: err => {
        this.toast.error("Error", "Error loading certificates signed by me: ", err);
      }
    })
  }

  openRevokeCertificate(certificate: Certificate) {
    const dialogRef: MatDialogRef<RevokeCertificateDialogComponent, null> = this.dialog.open(RevokeCertificateDialogComponent, {
      width: '30rem'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      const revokeCertificate: RevokeCertificate = {
        revocationReason: result,
        serialNumber: certificate.serialNumber
      };

      this.loading = true;

      this.crlService.revokeCertificate(revokeCertificate).subscribe({
        next: () => {
          this.loading = false;
          this.toast.success("Success", "Successfully revoked the certificate");
        },
        error: err => {
          this.loading = false;
          this.toast.error("Error", "Error revoking the certificate: ", err)
        }
      })
    })
  }

  openCertificateDetails(certificate: Certificate) {
    this.dialog.open(CertificateDetailsDialogComponent, {
      width: '850px',
      maxWidth: '70vw',
      data: { encodedCertificate: certificate.decryptedCertificate }
    });
  }

  downloadCertificate(certificate: Certificate) {
    this.certificatesService.downloadCertificate(certificate.serialNumber).subscribe({
      next: (blob: Blob) => {
        downloadFile(blob, `certificate_${certificate.prettySerialNumber}.pfx`)
      },
      error: async (err) => {
        const errorMessage = await extractBlobError(err);
        this.toast.error("Error", "Download failed: " + errorMessage);
      }
    });
  }
}
