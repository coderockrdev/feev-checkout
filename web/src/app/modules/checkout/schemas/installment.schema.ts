import * as z from "zod";

export const InstallmentSchema = z.object({
  number: z.int().positive(),
  dueAt: z.iso.date().nullable(),
  expireAt: z.iso.date().nullable(),
  fee: z.int().positive().nullable(),
  feeType: z.string().nullable(),
  finalAmount: z.int().positive(),
});
