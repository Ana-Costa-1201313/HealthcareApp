import { OperationRequest } from "./operationRequest.model";

export class PlanningDTO {
    planType: string;
    selectedRoomNumber: string;
    date: Date;
    selectedOpRequests: OperationRequest[];

    constructor(planType: string, selectedRoomNumber: string, date: Date, selectedOpRequests: OperationRequest[]) {
        this.planType = planType;
        this.selectedRoomNumber = selectedRoomNumber;
        this.date = date;
        this.selectedOpRequests = selectedOpRequests;
    }
}
