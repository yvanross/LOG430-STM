import {ArtefactDto} from "./artefactDto";

export class ExperimentReportDto {
  Timestamp: string | undefined;
  RunningInstances: ArtefactDto[]| undefined;
  AverageLatency: number| undefined;
  ErrorCount: number| undefined;
  Message: string| undefined;
}
