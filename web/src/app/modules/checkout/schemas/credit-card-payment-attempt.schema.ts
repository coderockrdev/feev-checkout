import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";

export const CreditCardPaymentAttemptSchema = z.object({
  extraData: z.object({}),
  success: z.boolean(),
  method: z.literal(PaymentMethod.CreditCard),
  externalId: z.string(),
});
