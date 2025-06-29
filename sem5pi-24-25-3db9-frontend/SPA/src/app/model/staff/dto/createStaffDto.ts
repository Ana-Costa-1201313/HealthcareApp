import { Role } from "../../role.model";

export type CreateStaffDto = {
  name?: string;
  licenseNumber?: number;
  phone?: string;
  specialization?: string;
  availabilitySlots?: string[];
  role?: Role;
  recruitmentYear?: number;
};
