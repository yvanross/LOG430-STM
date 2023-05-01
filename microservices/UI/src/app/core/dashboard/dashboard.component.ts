import {Component, OnInit, ViewChild} from '@angular/core';
import {MatSort} from "@angular/material/sort";
import {MatTableDataSource} from "@angular/material/table";
import {HttpClient} from "@angular/common/http";
import {IngressService} from "../../../Infrastructure/ingress.service";
import {ExperimentReportDto} from "../../../Dtos/experimentReportDto";
import {interval, startWith, switchMap} from "rxjs";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  dataSource = new MatTableDataSource<Row>();
  displayedColumns: string[] = ['User', 'AverageLatency', 'ErrorCount', 'Message'];

  @ViewChild(MatSort, { static: true }) sort!: MatSort;

  constructor(private http: HttpClient, private ingressService : IngressService) { }

  ngOnInit(): void {
    this.dataSource.sort = this.sort;

    interval(100000000)
      .pipe(
        startWith(0),
        switchMap(() => this.ingressService.fetchData()))
      .subscribe(response => {
        let rows:Row[] = [];

        // Iterate over the key-value pairs
        for (const key in response) {
          const value: string | undefined = response[key].at(0);
          if(value)
          {
            console.log(`Key: ${key}, Value:`, JSON.parse(value));
            rows.push(new Row(key, JSON.parse(value)));
          }
        }

        this.dataSource.data = rows;
      });
  }
}

export class Row
{
  constructor(
    public User:string,
    public ExperimentReport : ExperimentReportDto | undefined) {}
}
