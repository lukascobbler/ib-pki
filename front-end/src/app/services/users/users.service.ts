import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {CreateCaUser} from '../../models/CreateCaUser';
import {Observable} from 'rxjs';
import {CaUser} from '../../models/CaUser';
import {Certificate} from '../../models/Certificate';
import {AddCertificateToCaUser} from '../../models/AddCertificateToCaUser';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  httpClient = inject(HttpClient)
  urlCore = "https://localhost:8081/api/v1/users"

  getAllCaUsers(): Observable<CaUser[]> {
    return this.httpClient.get<CaUser[]>(`${this.urlCore}/get-all-ca-users`);
  }
}
