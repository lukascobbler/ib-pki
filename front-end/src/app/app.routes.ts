import { Routes } from '@angular/router';
import {IssueCertificateComponent} from './components/common/issue-certificate/issue-certificate.component';
import {AllCertificatesComponent} from './components/admin/all-certificates/all-certificates.component';
import {CaUserManagementComponent} from './components/admin/ca-user-management/ca-user-management.component';
import {LoginComponent} from './components/anonymous/login/login.component';

export const routes: Routes = [
  {path: "login", component: LoginComponent},
  {path: "issue-certificate", component: IssueCertificateComponent},
  {path: "all-certificates", component: AllCertificatesComponent},
  {path: "manage-ca-users", component: CaUserManagementComponent},
];
