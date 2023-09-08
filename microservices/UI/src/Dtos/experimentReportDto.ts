import {ArtefactDto} from "./artefactDto";

export class StateRequestDto {
  state: string | undefined;
  version: string | undefined;
  secure: boolean | undefined;
  dirty: boolean | undefined;
  experimentReportDto : string | undefined | ExperimentReportDto
}

export class ExperimentReportDto{
  Timestamp: string | undefined;
  RunningInstances: ArtefactDto[]| undefined;
  AverageLatency: number| undefined;
  Stability: number | undefined
  ErrorCount: number| undefined;
  Message: string| undefined;
}
