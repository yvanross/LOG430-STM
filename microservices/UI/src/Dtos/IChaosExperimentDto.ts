export interface IChaosExperimentDto
{
  ChaosCodex:
    {
      ChaosConfigs:
        {
          Connector:
            {
              MaxNumberOfPods: any;
              KillRate: any;
              NanoCpus: any;
              Memory: any;
              HardwareFailures: any;
            };
          Computation:
            {
              MaxNumberOfPods: any;
              KillRate: any;
              NanoCpus: any;
              Memory: any;
              HardwareFailures: any;
            };
          Database:
            {
              MaxNumberOfPods: any;
              KillRate: any;
              NanoCpus: any;
              Memory: any;
              HardwareFailures: any;
            }
        };
      StartTestAt: string;
      EndTestAt: string;
    };
  Coordinates:
    {
      StartingCoordinates: any;
      DestinationCoordinates: any
    };
}
