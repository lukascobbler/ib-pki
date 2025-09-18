import {CreateCertificateDTO} from '../../models/CreateCertificateDTO';
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

  urlCore = "http://localhost:8080/api/v1/certificates"

  issueCertificate(createCertificateDTO: CreateCertificateDTO): Observable<string> {
    return this.httpClient.post<string>(`${this.urlCore}/issue`, createCertificateDTO);
  }

  getAllCertificates(): Observable<Certificate[]> {
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/get-all`);
  }

  getAllSigningCertificates(): Observable<Certificate[]> {
    return this.httpClient.get<Certificate[]>(`${this.urlCore}/get-all-signing`);
  }
}
