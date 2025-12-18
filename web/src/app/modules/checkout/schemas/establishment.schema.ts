import * as z from "zod";

export const EstablishmentSchema = z.object({
  id: z.guid(),
  shortName: z.string(),
  fullName: z.string(),
  cnpj: z.string(),
});
