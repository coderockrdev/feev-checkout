import * as z from "zod";

import { TransactionStatusSchema } from "@modules/checkout/schemas/transaction-status.schema";

export type TransactionStatus = z.infer<typeof TransactionStatusSchema>;
