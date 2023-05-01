import {ArtefactDto} from "./artefactDto";

export class ExperimentReportDto {
  timestamp: Date | undefined;
  runningInstances: ArtefactDto[] | undefined;
  averageLatency: number | undefined;
  errorCount: number | undefined;
  message: string | undefined;
}
