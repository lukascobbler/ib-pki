import { Routes } from '@angular/router';
import {IssueCertificateComponent} from './components/common/issue-certificate/issue-certificate.component';
import {AllCertificatesComponent} from './components/admin/all-certificates/all-certificates.component';

export const routes: Routes = [
  {path: "issue-certificate", component: IssueCertificateComponent},
  {path: "all-certificates", component: AllCertificatesComponent},
];
