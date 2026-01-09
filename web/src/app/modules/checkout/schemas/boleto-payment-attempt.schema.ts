import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";

import { InvoiceSchema } from "./invoice.schema";
import { PaymentStatusSchema } from "./payment-status.schema";

export const BoletoPaymentAttemptSchema = z.object({
  extraData: z.preprocess(
    (payload: Record<string, unknown>) => ({
      code: payload["code"] ?? payload["Code"],
      link: payload["link"] ?? payload["Link"],
      invoices: payload["invoices"] ?? payload["Invoices"],
    }),
    z.object({
      code: z.int(),
      link: z.url(),
      invoices: z.array(InvoiceSchema),
    }),
  ),
  status: PaymentStatusSchema.nullable().optional().default(null),
  method: z.literal(PaymentMethod.Boleto),
});
