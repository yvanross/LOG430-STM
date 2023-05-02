import { Component, OnInit } from '@angular/core';
import {FormBuilder} from "@angular/forms";
import {HttpClient} from "@angular/common/http";
import {IngressService} from "../../../Infrastructure/ingress.service";

@Component({
  selector: 'app-chaos-dashboard',
  templateUrl: './chaos-dashboard.component.html',
  styleUrls: ['./chaos-dashboard.component.css']
})
export class ChaosDashboardComponent implements OnInit {
  chaosForm: any;

  constructor(private fb: FormBuilder, private ingressService: IngressService) {
    this.chaosForm = this.fb.group({
      ChaosCodex: this.fb.group({
        ChaosConfigs: this.fb.group({
          Computation: this.createChaosConfigGroup(this.defaultChaosConfig.ChaosConfigs.Computation),
          Database: this.createChaosConfigGroup(this.defaultChaosConfig.ChaosConfigs.Database),
          Connector: this.createChaosConfigGroup(this.defaultChaosConfig.ChaosConfigs.Connector)
        }),
        AcceptableAverageLatencyInMs: [this.defaultChaosConfig.AcceptableAverageLatencyInMs],
        EndTestAt: [this.defaultChaosConfig.EndTestAt],
        StartTestAt: [this.defaultChaosConfig.StartTestAt]
      }),
      Coordinates: this.fb.group({
        StartingCoordinates: [this.defaultCoordinates.StartingCoordinates],
        DestinationCoordinates: [this.defaultCoordinates.DestinationCoordinates]
      })
    });
  }

  ngOnInit(): void {}

  createChaosConfigGroup(config : any) {
    return this.fb.group({
      Key: [config.Key],
      Value: this.fb.group({
        NanoCpus: [config.Value.NanoCpus],
        Memory: [config.Value.Memory],
        MaxNumberOfPods: [config.Value.MaxNumberOfPods],
        KillRate: [config.Value.KillRate]
      })
    });
  }

  onSubmit() {
    const chaosData = this.chaosForm.value;

    const startTestAtLocal = new Date(chaosData.ChaosCodex.StartTestAt);
    const endTestAtLocal = new Date(chaosData.ChaosCodex.EndTestAt);

    const payload = {
      ChaosCodex:{
        ChaosConfigs: {
          Computation: {
            NanoCpus: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.NanoCpus,
            Memory: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.Memory,
            MaxNumberOfPods: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.MaxNumberOfPods,
            KillRate: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.KillRate
          },
          Database: {
            NanoCpus: chaosData.ChaosCodex.ChaosConfigs.Database.Value.NanoCpus,
            Memory: chaosData.ChaosCodex.ChaosConfigs.Database.Value.Memory,
            MaxNumberOfPods: chaosData.ChaosCodex.ChaosConfigs.Database.Value.MaxNumberOfPods,
            KillRate: chaosData.ChaosCodex.ChaosConfigs.Database.Value.KillRate
          },
          Connector: {
            NanoCpus: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.NanoCpus,
            Memory: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.Memory,
            MaxNumberOfPods: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.MaxNumberOfPods,
            KillRate: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.KillRate
          }
        },
        AcceptableAverageLatencyInMs: chaosData.ChaosCodex.AcceptableAverageLatencyInMs,
        EndTestAt: endTestAtLocal.toISOString(),
        StartTestAt: startTestAtLocal.toISOString()
      },
      Coordinates:{
        StartingCoordinates: chaosData.Coordinates.StartingCoordinates,
        DestinationCoordinates: chaosData.Coordinates.DestinationCoordinates,
      }
    }


    this.ingressService.beginExperiment(payload, true)
  }

  defaultChaosConfig = {
    ChaosConfigs: {
      Computation: {
        Key: 0,
        Value: {
          NanoCpus: 0,
          Memory: 0,
          MaxNumberOfPods: 100,
          KillRate: 0
        }
      },
      Database: {
        Key: 1,
        Value: {
          NanoCpus: 0,
          Memory: 0,
          MaxNumberOfPods: 100,
          KillRate: 0
        }
      },
      Connector: {
        Key: 2,
        Value: {
          NanoCpus: 0,
          Memory: 0,
          MaxNumberOfPods: 100,
          KillRate: 0
        }
      }
    },
    AcceptableAverageLatencyInMs: 100000,
    EndTestAt: '2023-07-08T20:00:23.4011608Z',
    StartTestAt: '2023-04-09T20:00:23.4012561Z'
  };

  defaultCoordinates = {
    StartingCoordinates: '45.508275469136336, -73.57146349142822',
    DestinationCoordinates: '45.49797372251121, -73.58032551009549'
  };

}
