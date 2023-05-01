import { Injectable } from '@angular/core';
import {environment} from "../environments/environment";
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject} from "rxjs";
import {ExperimentReportDto} from "../Dtos/experimentReportDto";

@Injectable({
  providedIn: 'root'
})
export class IngressService {

  private apiUrl = "http://localhost:"+environment.INGRESS_PORT;

  private token = new BehaviorSubject("");

  constructor(private http: HttpClient) { }

  login(username: string, password: string) {
    const body = {
      name: username,
      secret: password
    };

    this.http.post(this.apiUrl + '/Ingress/Authorize', body,
      {
        responseType: 'text'
      })
      .subscribe((response : string) => {
      this.setToken(response.toString())
    });
  }

  getAuthenticationStatus()
  {
    return this.token;
  }

  fetchData() {
    return this.http.get<ResponseMap>(this.apiUrl + '/Ingress/GetLogs',
      {
        headers:
          {
            'Authorization' : 'Bearer ' + this.token.value
          },
      })
  }

  private setToken(response: string) {
    this.token.next(response);
  }
}

export interface ResponseMap {
  [key: string]: string[];
}
