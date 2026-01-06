import * as z from "zod";

export const EstablishmentSchema = z.object({
  id: z.guid(),
  name: z.string(),
  bankNumber: z.string().nullable(),
  bankAgency: z.string().nullable(),
  bankAccount: z.string().nullable(),
  checkingAccountNumber: z.string().nullable(),
});
