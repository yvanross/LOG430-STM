import {ChangeDetectorRef, Component, OnInit, ViewChild} from '@angular/core';
import {MatSort} from "@angular/material/sort";
import {MatTableDataSource} from "@angular/material/table";
import {HttpClient} from "@angular/common/http";
import {IngressService} from "../../../Infrastructure/ingress.service";
import {ExperimentReportDto} from "../../../Dtos/experimentReportDto";
import {interval, map, mergeMap, startWith, switchMap, tap} from "rxjs";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  dataSource = new MatTableDataSource<Row>();
  displayedColumns: string[] = ['User', 'AverageLatency', 'ErrorCount', 'Message'];

  @ViewChild(MatSort, { static: true }) sort!: MatSort;

  private pipeStarted: Boolean = false;

  constructor(private http: HttpClient, private ingressService : IngressService, private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.dataSource.sort = this.sort;

    if(!this.pipeStarted)
    {
      interval(1000)
        .pipe(
          startWith(0),
          mergeMap(() => this.ingressService.fetchData()))
        .subscribe(response => {
          let rows:Row[] = [];

          // Iterate over the key-value pairs
          for (const key in response) {
            let value: string | undefined = response[key].pop();
            if(value)
            {
              console.log(`Key: ${key}, Value:`, JSON.parse(value));
              rows.push(new Row(key, JSON.parse(value)));
            }
          }

          this.dataSource.data = rows;

          this.changeDetectorRef.detectChanges();
        });
      
      this.pipeStarted = true
    }

  }
}

export class Row
{
  constructor(
    public User:string,
    public ExperimentReport : ExperimentReportDto | undefined) {}
}
