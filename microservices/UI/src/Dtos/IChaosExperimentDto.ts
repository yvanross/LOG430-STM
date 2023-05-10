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
              Memory: any
            };
          Computation:
            {
              MaxNumberOfPods: any;
              KillRate: any;
              NanoCpus: any;
              Memory: any
            };
          Database:
            {
              MaxNumberOfPods: any;
              KillRate: any;
              NanoCpus: any;
              Memory: any
            }
        };
      StartTestAt: string;
      EndTestAt: string;
      AcceptableAverageLatencyInMs: any
    };
  Coordinates:
    {
      StartingCoordinates: any;
      DestinationCoordinates: any
    };
}
