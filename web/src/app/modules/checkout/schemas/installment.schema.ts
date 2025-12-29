import * as z from "zod";

import { RawDateOnlySchema } from "@app/shared/schemas/date";

export const InstallmentSchema = z.object({
  number: z.int().min(0),
  dueAt: RawDateOnlySchema.nullable(),
  expireAt: RawDateOnlySchema.nullable(),
  fee: z.int().min(0).nullable(),
  feeType: z.string().nullable(),
  finalAmount: z.int().min(0),
});
