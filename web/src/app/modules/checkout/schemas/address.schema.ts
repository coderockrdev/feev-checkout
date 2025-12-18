import * as z from "zod";

export const AddressSchema = z.object({
  street: z.string(),
  number: z.string().min(1).max(15),
  complement: z.string().max(50).nullable(),
  district: z.string().min(1).max(255),
  city: z.string().min(3).max(50),
  state: z.string().regex(/^[A-Z]{2}$/),
  zipCode: z.string().regex(/^\d+&/).length(8),
});
