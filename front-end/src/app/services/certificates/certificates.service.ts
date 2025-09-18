import {CreateCertificate} from '../../models/CreateCertificate';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Certificate} from '../../models/Certificate';

@Injectable({
  providedIn: 'root'
})
export class CertificatesService {
  constructor(private httpClient: HttpClient) {
  }

  urlCore = "https://localhost:8081/api/v1/certificates"

  issueCertificate(createCertificate: CreateCertificate): Observable<string> {
    return this.httpClient.post<string>(`${this.urlCore}/issue`, createCertificate);
  }

  getAllCertificates(): Observable<Certificate[]> {
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/get-all`);
  }

  getAllSigningCertificates(): Observable<Certificate[]> {
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/get-all-signing`);
  }
}
