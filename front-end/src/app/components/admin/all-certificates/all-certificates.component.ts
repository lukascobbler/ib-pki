import {Component, inject, OnInit} from '@angular/core';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable, MatTableDataSource
} from '@angular/material/table';
import {MatIconButton} from '@angular/material/button';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {
  RevokeCertificateDialogComponent
} from '../../common/revoke-certificate-dialog/revoke-certificate-dialog.component';
import {
  CertificateDetailsDialogComponent
} from '../../common/certificate-details-dialog/certificate-details-dialog.component';
import {CertificatesService} from '../../../services/certificates/certificates.service';
import {Certificate} from '../../../models/Certificate';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {DatePipe, NgIf} from '@angular/common';
import {ToastrService} from '../../common/toastr/toastr.service';
import {downloadFile} from '../../common/blob/download-file';
import {extractBlobError} from '../../common/blob/extract-blob-error';

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
    MatHeaderRowDef,
    MatProgressSpinner,
    NgIf,
    DatePipe
  ],
  templateUrl: './all-certificates.component.html',
  styleUrl: './all-certificates.component.scss'
})
export class AllCertificatesComponent implements OnInit {
  certificatesService = inject(CertificatesService);
  toast = inject(ToastrService)
  dialog = inject(MatDialog);
  displayedColumns: string[] = ['issuedTo', 'issuedBy', 'status', 'validFrom', 'validUntil', 'serialNumber', 'actions'];
  certificatesDataSource = new MatTableDataSource<Certificate>();
  certificates: Certificate[] = [];
  loadingCertificates = true;

  ngOnInit() {
    this.certificatesService.getAllCertificates().subscribe({
      next: value => {
        this.certificates = value;
        this.certificatesDataSource.data = this.certificates;
        this.loadingCertificates = false;
      },
      error: err => {
        this.toast.error("Error", "Unable to load certificates: " + err);
      }
    })
  }

  openRevokeCertificate(certificate: Certificate) {
    const dialogRef: MatDialogRef<RevokeCertificateDialogComponent, null> = this.dialog.open(RevokeCertificateDialogComponent, {
      width: '30rem'
    });
  }

  openCertificateDetails(certificate: Certificate) {
    this.dialog.open(CertificateDetailsDialogComponent, {
      width: '850px',
      maxWidth: '70vw',
      data: {encodedCertificate: certificate.decryptedCertificate}
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
        downloadFile(blob, `certificate_chain_${certificate.prettySerialNumber}.pem`)
      },
      error: async (err) => {
        const errorMessage = await extractBlobError(err);
        this.toast.error("Error", "Download failed: " + errorMessage);
      }
    });
  }
}
