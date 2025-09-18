import {CreateCertificate} from '../../models/CreateCertificate';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CertificatesService {
  constructor(private httpClient: HttpClient) {
  }

  urlCore = "http://localhost:8080/api/v1/certificates"

  issueCertificate(createCertificate: CreateCertificate): Observable<string> {
    return this.httpClient.post<string>(`${this.urlCore}/issue`, createCertificate);
  }
}
