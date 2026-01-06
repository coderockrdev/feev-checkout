import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";

import { PaymentMethodSchema } from "./payment-method.schema";
import { CardDtoSchema } from "./card-dto.schema";

const METHOD_LABEL = {
  [PaymentMethod.PIX]: "PIX",
  [PaymentMethod.Boleto]: "Boleto",
} as const;

export const PaymentRequestDtoSchema = z
  .object({
    method: PaymentMethodSchema,
    cardHolder: z.string().min(6).max(255).optional(),
    cardNumber: z.string().min(10).max(19).optional(),
    validThru: z
      .string()
      .regex(/^\d{2}\/\d{2}$/)
      .optional(),
    cvv: z
      .string()
      .regex(/^\d{3}$/)
      .optional(),
    installments: z
      .preprocess(
        (value) => (value === "" || value == null || value === undefined ? undefined : value),
        z.coerce.number().int().min(1),
      )
      .optional(),
  })
  .superRefine((payload, ctx) => {
    if (payload.method === PaymentMethod.Boleto || payload.method === PaymentMethod.PIX) {
      if (typeof payload.installments === "number") {
        ctx.addIssue({
          path: ["installments"],
          message: `Não é permitido parcelamento para o método ${METHOD_LABEL[payload.method]}.`,
          code: "custom",
        });
      }
    }

    if (payload.method === PaymentMethod.CreditCard) {
      if (!payload.installments) {
        ctx.addIssue({
          path: ["installments"],
          message: "Quantidade de parcelas é obrigatória",
          code: "custom",
        });
      }

      if (!payload.cardHolder || !payload.cardNumber || !payload.validThru) {
        ctx.addIssue({
          path: ["card"],
          message: "Dados do cartão são obrigatórios",
          code: "custom",
        });
      }
    }
  })
  .transform((payload) => {
    const isCreditCard = payload.method === PaymentMethod.CreditCard;

    return {
      method: payload.method,
      installments: isCreditCard ? payload.installments : null,
      card: isCreditCard ? CardDtoSchema.parse(payload) : null,
    };
  });
