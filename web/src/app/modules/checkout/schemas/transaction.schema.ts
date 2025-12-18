import * as z from "zod";

import { CustomerSchema } from "./customer.schema";
import { EstablishmentSchema } from "./establishment.schema";
import { PaymentRuleSchema } from "./payment-rule.schema";
import { ProductSchema } from "./product.schema";
import { TransactionStatusSchema } from "./transaction-status.schema";

export const TransactionSchema = z.object({
  id: z.guid(),
  establishment: EstablishmentSchema,
  identifier: z.string(),
  description: z.string(),
  customer: CustomerSchema,
  totalAmount: z.int(),
  products: z.array(ProductSchema),
  paymentRules: z.array(PaymentRuleSchema),
  successfulPaymentAttemptId: z.string().nullable(),
  successfulPaymentAttempt: z.string().nullable(),
  expireAt: z.iso.datetime().nullable(),
  canceledAt: z.iso.datetime().nullable(),
  completedAt: z.iso.datetime().nullable(),
  createdAt: z.iso.datetime(),
  status: TransactionStatusSchema,
});
