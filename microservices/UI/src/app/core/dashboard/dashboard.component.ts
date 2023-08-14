import {ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {MatSort} from "@angular/material/sort";
import {MatTableDataSource} from "@angular/material/table";
import {HttpClient} from "@angular/common/http";
import {IngressService} from "../../../Infrastructure/ingress.service";
import {ExperimentReportDto, StateRequestDto} from "../../../Dtos/experimentReportDto";
import {exhaustMap, interval, map, mergeMap, startWith, Subject, Subscription, switchMap, takeUntil, tap} from "rxjs";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {

  dataSource = new MatTableDataSource<Row>();
  displayedColumns: string[] = ['User', 'Connected', 'Secure', 'Dirty', 'Version', 'AverageLatency', 'ErrorCount', 'Message', 'Computation', 'Connector', 'Database'];

  @ViewChild(MatSort, { static: true }) sort!: MatSort;

  constructor(private http: HttpClient, private ingressService : IngressService, private changeDetectorRef: ChangeDetectorRef) { }

  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.dataSource.sort = this.sort;

    interval(1000)
      .pipe(
        startWith(0),
        mergeMap(() => this.ingressService.fetchData()),
        takeUntil(this.destroy$))
      .subscribe(response => {
        let rows: Row[] = [];

        // Iterate over the key-value pairs
        for (const key in response) {
          let value = response[key];
          if (value) {

            if(!value.experimentReportDto || typeof value.experimentReportDto === 'string' )
              value.experimentReportDto = !value.experimentReportDto ?
                {
                  Timestamp: "No Data",
                  AverageLatency: -1,
                  ErrorCount: -1,
                  Message: "No Data",
                  RunningInstances: []
                } : JSON.parse(value.experimentReportDto!.toString())

            let experimentDto =  (<ExperimentReportDto>value.experimentReportDto!)

            let computation = 0;
            let connector = 0;
            let data = 0;

            for (let runningInstance of experimentDto.RunningInstances!) {
              switch (runningInstance.ArtifactType) {
                case "Computation": computation++;
                break;
                case "Connector": connector++;
                break;
                case "Database": data++;
                break;
              }
            }

            console.log(`Key: ${key}, Value:`, value);
            rows.push(new Row(key, true, value, computation, connector, data));
          }
        }

        this.dataSource.data = rows;

        this.changeDetectorRef.detectChanges();
      });
  }

  getConnectionQualityClass(connectionQuality: any) {
    return connectionQuality;
  }

  getSecurityClass(secure: any) {
    return secure ? 'Ready' : 'Unknown';
  }

  getIsDirtyClass(dirty: any) {
    return dirty ? 'Unknown' : 'Ready';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

export class Row
{
  constructor(
    public User:string,
    public Included:boolean = true,
    public StateRequest : StateRequestDto | undefined,
    public Computation:number,
    public Connector:number,
    public Database:number) {}
}
