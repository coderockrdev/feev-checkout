import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";
import { RawDateTimeSchema } from "@shared/schemas/date";

import { PaymentStatusSchema } from "./payment-status.schema";

export const PixPaymentAttemptSchema = z.object({
  extraData: z.preprocess(
    (payload: Record<string, unknown>) => ({
      code: payload["code"] ?? payload["Code"],
      location: payload["location"] ?? payload["Location"],
      createdAt: payload["createdAt"] ?? payload["CreatedAt"],
      dueAt: payload["dueAt"] ?? payload["DueAt"],
      expireAt: payload["expireAt"] ?? payload["ExpireAt"],
    }),
    z
      .object({
        code: z.string(),
        location: z.url(),
        createdAt: RawDateTimeSchema,
        dueAt: RawDateTimeSchema,
        expireAt: RawDateTimeSchema,
      })
      .transform((payload) => ({
        ...payload,
        expireAt: payload.expireAt.replace(/([+-]\d{2}:\d{2}|Z)$/, "-03:00"), // INFO: Cast to America/Sao_Paulo TZ
      })),
  ),
  status: PaymentStatusSchema.nullable().optional().default(null),
  method: z.literal(PaymentMethod.PIX),
});
