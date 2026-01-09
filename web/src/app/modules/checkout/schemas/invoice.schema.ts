import { RawDateTimeSchema } from "@app/shared/schemas/date";
import * as z from "zod";

export const InvoiceSchema = z.preprocess(
  (payload: Record<string, unknown>) => ({
    number: payload["number"] ?? payload["Number"],
    dueAt: payload["dueAt"] ?? payload["DueAt"],
    digitableLine: payload["digitableLine"] ?? payload["DigitableLine"],
    link: payload["link"] ?? payload["Link"],
  }),
  z.object({
    number: z.int().positive(),
    dueAt: RawDateTimeSchema,
    digitableLine: z.string(),
    link: z.string(),
  }),
);
