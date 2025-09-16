import { Routes } from '@angular/router';
import {IssueCertificateComponent} from './components/common/issue-certificate/issue-certificate.component';
import {AllCertificatesComponent} from './components/admin/all-certificates/all-certificates.component';
import {CaUserManagementComponent} from './components/admin/ca-user-management/ca-user-management.component';
import {LoginComponent} from './components/anonymous/login/login.component';
import {RegistrationComponent} from './components/anonymous/registration/registration.component';
import {MyCertificatesComponent} from './components/common/my-certificates/my-certificates.component';
import {RequestCertificateComponent} from './components/ee-user/request-certificate/request-certificate.component';
import {CertificateRequestsComponent} from './components/ca-user/certificate-requests/certificate-requests.component';
import {SignedCertificatesComponent} from './components/ca-user/signed-certificates/signed-certificates.component';
import {CrlPageComponent} from './components/common/crl-page/crl-page.component';

export const routes: Routes = [
  {path: "login", component: LoginComponent},
  {path: "register", component: RegistrationComponent},
  {path: "issue-certificate", component: IssueCertificateComponent},
  {path: "all-certificates", component: AllCertificatesComponent},
  {path: "manage-ca-users", component: CaUserManagementComponent},
  {path: "my-certificates", component: MyCertificatesComponent},
  {path: "request-certificate", component: RequestCertificateComponent},
  {path: "certificate-requests", component: CertificateRequestsComponent},
  {path: "signed-certificates", component: SignedCertificatesComponent},
  {path: "crl", component: CrlPageComponent}
];
