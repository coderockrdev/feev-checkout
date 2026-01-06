import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";

export const BoletoPaymentAttemptSchema = z.object({
  extraData: z.object({}),
  success: z.boolean(),
  method: z.literal(PaymentMethod.Boleto),
  externalId: z.string(),
});
