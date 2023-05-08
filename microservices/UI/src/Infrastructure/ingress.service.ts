import { Injectable } from '@angular/core';
import {environment} from "../environments/environment";
import {HttpClient, HttpParams} from "@angular/common/http";
import {BehaviorSubject} from "rxjs";
import {StateRequestDto} from "../Dtos/experimentReportDto";

@Injectable({
  providedIn: 'root'
})
export class IngressService {

  private apiUrl = "http://localhost:"+environment.INGRESS_PORT;

  private token = new BehaviorSubject("");

  private username : string | undefined;

  constructor(private http: HttpClient) { }

  login(username: string, password: string) {
    const body = {
      name: username,
      secret: password
    };

    this.username = username

    this.http.post(this.apiUrl + '/Ingress/Authorize', body,
      {
        responseType: 'text'
      })
      .subscribe((response : string) => {
      this.setToken(response.toString())
    });
  }

  beginExperiment(body : any, wholeTeam : boolean)
  {
    const queryParams = new HttpParams()
      .set('includeAllVisibleAccounts', wholeTeam)

    this.http.post(this.apiUrl + '/Ingress/BeginExperiment', body, {
      params: queryParams,
      headers:
        {
          'Authorization' : 'Bearer ' + this.token.value
        },
    }).subscribe(response => {
      console.log('Chaos data submitted successfully', response);
    });;
  }

  getAuthenticationStatus()
  {
    return this.token;
  }

  fetchData() {
    return this.http.get<ResponseMap>(this.apiUrl + '/Ingress/GetStates',
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
  [key: string]: StateRequestDto;
}
