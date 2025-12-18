import * as z from "zod";
import { AddressSchema } from "./address.schema";

export const CustomerSchema = z.object({
  name: z.string(),
  document: z.string().regex(/^\d+$/).min(11).max(14),
  email: z.email(),
  address: AddressSchema,
});
