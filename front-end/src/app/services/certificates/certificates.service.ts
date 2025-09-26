import {CreateCertificate} from '../../models/CreateCertificate';
import {HttpClient} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Certificate} from '../../models/Certificate';
import {AddCertificateToCaUser} from '../../models/AddCertificateToCaUser';

@Injectable({
  providedIn: 'root'
})
export class CertificatesService {
  httpClient = inject(HttpClient)
  urlCore = "https://localhost:8081/api/v1/certificates"

  issueCertificate(createCertificate: CreateCertificate): Observable<string> {
    return this.httpClient.post<string>(`${this.urlCore}/issue`, createCertificate);
  }

  getAllCertificates(): Observable<Certificate[]> {
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/get-all`);
  }

  getValidSigningCertificates(): Observable<Certificate[]> {
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/get-all-valid-signing`);
  }

  getValidSigningCertificatesForCa(caUserId: string): Observable<Certificate[]> {
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/${caUserId}/get-valid-signing`);
  }

  addNewCertificateToCaUser(addCertificateToCaUser: AddCertificateToCaUser): Observable<void> {
    return this.httpClient.put<void>(`${this.urlCore}/add-certificate-to-ca-user`, addCertificateToCaUser);
  }

  downloadCertificate(certificate: Certificate): Observable<Blob> {
    return this.httpClient.get(`${this.urlCore}/download/${certificate.serialNumber}`, { responseType: 'blob' });
  }
}
