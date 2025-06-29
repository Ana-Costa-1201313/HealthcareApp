import { Role } from '../role.model';

export type Staff = {
  id?: string;
  name?: string;
  licenseNumber?: number;
  email?: string;
  phone?: string;
  specialization?: string;
  availabilitySlots?: string[];
  role?: Role;
  mechanographicNum?: string;
  active?: boolean;
};
