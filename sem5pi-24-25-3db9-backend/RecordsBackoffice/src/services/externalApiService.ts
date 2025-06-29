import { Service } from "typedi";
import axios from 'axios';
import https from 'https';


@Service()
export default class ExternalApiService {

    // Validate Token with Auth Module
    async validateToken(loginDTO) {
        const url = `https://localhost:7058/Auth/Login/validate`;

        try {
            const response = await axios.post(
                url,
                loginDTO,
                {
                    headers: {
                        Authorization: `Bearer ${loginDTO.jwt}`,
                        'Content-Type': 'application/json'
                    },
                    httpsAgent: new https.Agent({
                        rejectUnauthorized: false 
                    })
                }
            );

            return response.data;
        } catch (error) {
            const errorMessage = error.response ? error.response.data.message : error.message;
            throw new Error(`Failed to authenticate user: ${errorMessage}`);
        }
    }

    // Check if user has required roles
    async checkHeader(roles, tokenHeader) {
        const token = tokenHeader.replace('Bearer ', '');

        const loginDTO = {
            jwt: token
        };


        const loginDTOResult = await this.validateToken(loginDTO);

        if (!loginDTOResult || !roles.includes(loginDTOResult.role)) {
            throw new Error('User does not have necessary roles!');
        }
        return true;
    }
}
