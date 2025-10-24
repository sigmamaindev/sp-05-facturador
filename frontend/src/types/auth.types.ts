import z from "zod";

import type { Business } from "./business.types";
import type { Establishment } from "./establishment.types";
import type { EmissionPoint } from "./emissionPoint.types";

const authSchema = z.object({
  username: z.string(),
  password: z.string(),
});

type Auth = z.infer<typeof authSchema>;

export type UserLoginForm = Pick<Auth, "username" | "password">;

export interface AuthData {
  id: number;
  document: string;
  username: string;
  fullName: string;
  email: string;
  token: string;
  roles: string[];
  business: Business;
  establishment: Establishment[];
  emissionPoint: EmissionPoint[];
}
