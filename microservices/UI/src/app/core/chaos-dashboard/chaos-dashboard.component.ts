import { Component, OnInit } from '@angular/core';
import {FormBuilder} from "@angular/forms";
import {HttpClient} from "@angular/common/http";
import {IngressService} from "../../../Infrastructure/ingress.service";
import {IChaosExperimentDto} from "../../../Dtos/IChaosExperimentDto";

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
        Duration: [this.defaultChaosConfig.Duration],
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
        CPUs: [config.Value.NanoCpus / 1000000000],
        Memory: [config.Value.Memory * 0.000001],
        MaxNumberOfPods: [config.Value.MaxNumberOfPods],
        KillRate: [config.Value.KillRate],
        HardwareFailures: [config.Value.HardwareFailures]
      })
    });
  }

  onSubmit() {
    const chaosData = this.chaosForm.value;

    const startTestAtUTC = new Date();

    let duration = chaosData.ChaosCodex.Duration

    // new Date object 5 minutes ahead of the current time
    const endTestAtUTC = new Date(startTestAtUTC.getTime() + duration * 60 * 1000);

    const payload: IChaosExperimentDto = {
      ChaosCodex:{
        ChaosConfigs: {
          Computation: {
            NanoCpus: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.CPUs * 1000000000,
            Memory: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.Memory / 0.000001,
            MaxNumberOfPods: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.MaxNumberOfPods,
            KillRate: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.KillRate,
            HardwareFailures: chaosData.ChaosCodex.ChaosConfigs.Computation.Value.HardwareFailures
          },
          Database: {
            NanoCpus: chaosData.ChaosCodex.ChaosConfigs.Database.Value.CPUs * 1000000000,
            Memory: chaosData.ChaosCodex.ChaosConfigs.Database.Value.Memory / 0.000001,
            MaxNumberOfPods: chaosData.ChaosCodex.ChaosConfigs.Database.Value.MaxNumberOfPods,
            KillRate: chaosData.ChaosCodex.ChaosConfigs.Database.Value.KillRate,
            HardwareFailures: chaosData.ChaosCodex.ChaosConfigs.Database.Value.HardwareFailures
          },
          Connector: {
            NanoCpus: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.CPUs * 1000000000,
            Memory: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.Memory / 0.000001,
            MaxNumberOfPods: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.MaxNumberOfPods,
            KillRate: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.KillRate,
            HardwareFailures: chaosData.ChaosCodex.ChaosConfigs.Connector.Value.HardwareFailures
          }
        },
        EndTestAt: endTestAtUTC.toISOString(),
        StartTestAt: startTestAtUTC.toISOString()
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
          NanoCpus: 10000000000,
          Memory: 16000000000,
          MaxNumberOfPods: 100,
          KillRate: 0,
          HardwareFailures: 0
        }
      },
      Database: {
        Key: 1,
        Value: {
          NanoCpus: 10000000000,
          Memory: 16000000000,
          MaxNumberOfPods: 100,
          KillRate: 0,
          HardwareFailures: 0
        }
      },
      Connector: {
        Key: 2,
        Value: {
          NanoCpus: 10000000000,
          Memory: 16000000000,
          MaxNumberOfPods: 100,
          KillRate: 0,
          HardwareFailures: 0
        }
      }
    },
    Duration: 0,
  };

  defaultCoordinates = {
    StartingCoordinates: '45.508275469136336, -73.57146349142822',
    DestinationCoordinates: '45.49797372251121, -73.58032551009549'
  };

}
