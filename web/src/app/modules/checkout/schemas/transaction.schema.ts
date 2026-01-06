import * as z from "zod";

import { RawDateTimeSchema } from "@shared/schemas/date";

import { CustomerSchema } from "./customer.schema";
import { EstablishmentSchema } from "./establishment.schema";
import { PaymentRuleSchema } from "./payment-rule.schema";
import { ProductSchema } from "./product.schema";
import { TransactionStatusSchema } from "./transaction-status.schema";

export const TransactionSchema = z.object({
  id: z.guid(),
  establishment: EstablishmentSchema.nullable(),
  identifier: z.string(),
  description: z.string(),
  customer: CustomerSchema,
  totalAmount: z.int(),
  products: z.array(ProductSchema),
  paymentRules: z.array(PaymentRuleSchema),
  successfulPaymentAttemptId: z.string().nullable(),
  successfulPaymentAttempt: z.string().nullable(),
  expireAt: RawDateTimeSchema.nullable(),
  canceledAt: RawDateTimeSchema.nullable(),
  completedAt: RawDateTimeSchema.nullable(),
  createdAt: RawDateTimeSchema,
  status: TransactionStatusSchema,
});
