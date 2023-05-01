import { Component, OnInit } from '@angular/core';
import {FormBuilder} from "@angular/forms";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-chaos-dashboard',
  templateUrl: './chaos-dashboard.component.html',
  styleUrls: ['./chaos-dashboard.component.css']
})
export class ChaosDashboardComponent implements OnInit {
  chaosForm: any;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.chaosForm = this.fb.group({
      ChaosCodex: this.fb.group({
        ChaosConfigs: this.fb.group({
          Computation: this.createChaosConfigGroup(),
          Database: this.createChaosConfigGroup(),
          Connector: this.createChaosConfigGroup()
        }),
        AcceptableAverageLatencyInMs: [100000],
        EndTestAt: [''],
        StartTestAt: ['']
      }),
      Coordinates: this.fb.group({
        StartingCoordinates: [''],
        DestinationCoordinates: ['']
      })
    });
  }

  ngOnInit(): void {}

  createChaosConfigGroup() {
    return this.fb.group({
      Key: [''],
      Value: this.fb.group({
        NanoCpus: [''],
        Memory: [''],
        MaxNumberOfPods: [''],
        KillRate: ['']
      })
    });
  }

  onSubmit() {
    const chaosData = this.chaosForm.value;

  }

}
