import * as z from "zod";

export const CustomerSchema = z.object({
  name: z.string(),
  document: z.string(),
});
