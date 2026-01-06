import * as z from "zod";

import { PixPaymentAttemptSchema } from "./pix-payment-attempt.schema";
import { BoletoPaymentAttemptSchema } from "./boleto-payment-attempt.schema";
import { CreditCardPaymentAttemptSchema } from "./credit-card-payment-attempt.schema";

export const PaymentAttemptSchema = z.discriminatedUnion("method", [
  PixPaymentAttemptSchema,
  BoletoPaymentAttemptSchema,
  CreditCardPaymentAttemptSchema,
]);
