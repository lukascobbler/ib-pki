import {CreateCertificate} from '../../models/CreateCertificate';
import {HttpClient} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Certificate} from '../../models/Certificate';

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
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/get-valid-signing`);
  }

  downloadCertificate(certificate: Certificate): Observable<Blob> {
    return this.httpClient.get(`${this.urlCore}/download/${certificate.serialNumber}`, { responseType: 'blob' });
  }
}
