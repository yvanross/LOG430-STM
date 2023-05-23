import {Injectable, OnInit} from '@angular/core';
import {environment} from "../environments/environment";
import {HttpClient, HttpParams} from "@angular/common/http";
import {BehaviorSubject, delay, exhaustMap, Observable, retry, share, Subject, switchMap} from "rxjs";
import {StateRequestDto} from "../Dtos/experimentReportDto";
import {IChaosExperimentDto} from "../Dtos/IChaosExperimentDto";
import {IAuthDto} from "../Dtos/IAuthDto";

@Injectable({
  providedIn: 'root'
})
export class IngressService {

  private apiUrl = "http://localhost:"+environment.INGRESS_PORT;

  private token = new BehaviorSubject("");

  private static requestLogin$ = new Subject<IAuthDto>();
  private static requestBeginExperiment$ = new Subject<IChaosExperimentDto>();
  private static requestFetchData$ = new Subject();

  private static responseFetchData$ : Observable<ResponseMap>;

  constructor(private http: HttpClient) {
    this.ngOnInit()
  }

  login(authDto : IAuthDto) {
    IngressService.requestLogin$.next(authDto);
  }

  beginExperiment(body : IChaosExperimentDto, wholeTeam : boolean)
  {
    IngressService.requestBeginExperiment$.next(body);
  }

  fetchData() {
    IngressService.requestFetchData$.next({});

    return IngressService.responseFetchData$;
  }

  private setToken(response: string) {
    this.token.next(response);
  }

  getAuthenticationStatus()
  {
    return this.token;
  }

  ngOnInit(): void
  {
    IngressService.requestLogin$
      .pipe(
        switchMap( (authBody) =>
          this.http.post(this.apiUrl + '/Ingress/Authorize', authBody,
            {
              responseType: 'text'
            })
        ))
      .subscribe((response : string) => this.setToken(response.toString()));

    IngressService.requestBeginExperiment$
      .pipe(
        switchMap( (experimentBody) =>
          this.http.post(this.apiUrl + '/Ingress/BeginExperiment', experimentBody, {
            headers: { 'Authorization' : 'Bearer ' + this.token.value}
          })
            .pipe(retry({delay : 1000}))
        ))
       .subscribe(response => console.log('Chaos data submitted successfully', response))

    IngressService.responseFetchData$ = IngressService.requestFetchData$
      .pipe(
        exhaustMap( () =>
          this.http.get<ResponseMap>(this.apiUrl + '/Ingress/GetStates', {
              headers:
                {
                  'Authorization' : 'Bearer ' + this.token.value
                },
            })
            .pipe(retry({delay : 1000}))
        ), share())
  }
}

export interface ResponseMap {
  [key: string]: StateRequestDto;
}
