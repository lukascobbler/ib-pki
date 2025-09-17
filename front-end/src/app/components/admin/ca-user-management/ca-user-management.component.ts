import {Component} from '@angular/core';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow, MatRowDef, MatTable
} from "@angular/material/table";
import {MatIconButton} from "@angular/material/button";
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {CaRegistrationDialogComponent} from './ca-registration-dialog/ca-registration-dialog.component';
import {CaAddCertificateDialogComponent} from './ca-add-certificate-dialog/ca-add-certificate-dialog.component';

@Component({
  selector: 'app-ca-user-management',
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
  templateUrl: './ca-user-management.component.html',
  styleUrl: './ca-user-management.component.scss'
})
export class CaUserManagementComponent {
  constructor(private dialog: MatDialog) {
  }

  displayedColumns: string[] = [
    'name',
    'surname',
    'organization',
    'email',
    'actions'
  ];

  caUsersDataSource: { name: string, surname: string, organization: string, email: string }[] = [
    {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    },
    {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    },
    {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    },
    {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    },
    {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    }, {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    }, {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    },
    {
      name: 'Michael',
      surname: 'Scott',
      organization: 'Dunder Mifflin',
      email: 'michael.scott@dundermifflin.com',
    },
  ];

  openRegisterDialog() {
    const dialogRef: MatDialogRef<CaRegistrationDialogComponent, null> = this.dialog.open(CaRegistrationDialogComponent, {
      width: '30rem'
    });
  }

  openAssignNewCertificate() {
    const dialogRef: MatDialogRef<CaAddCertificateDialogComponent, null> = this.dialog.open(CaAddCertificateDialogComponent, {
      width: '30rem'
    });
  }
}
