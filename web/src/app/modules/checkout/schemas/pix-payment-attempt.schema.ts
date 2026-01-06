import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";
import { RawDateTimeSchema } from "@shared/schemas/date";

export const PixPaymentAttemptSchema = z.object({
  extraData: z.object({
    code: z.string(),
    location: z.url(),
    createdAt: RawDateTimeSchema,
    dueAt: RawDateTimeSchema,
    expireAt: RawDateTimeSchema,
  }),
  success: z.boolean(),
  method: z.literal(PaymentMethod.PIX),
  externalId: z.string(),
});
