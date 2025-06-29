import { Service, Inject } from "typedi";
import ExternalApiService from "./externalApiService";

@Service()
export default class AuthService {
    constructor(
        @Inject(() => ExternalApiService) private externalApiService: ExternalApiService
    ) {}

    public async isAuthorized(req, roles: string[]): Promise<boolean> {
        const tokenHeader = req.headers['authorization'];

        if (!tokenHeader) {
            throw new Error("Error: Authorization header is missing");
        }

        try {
            return await this.externalApiService.checkHeader(roles, tokenHeader);
        } catch (error) {
            throw new Error("Error: User not authenticated");
        }
    }
}
