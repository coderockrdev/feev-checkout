import * as z from "zod";

import { TransactionSchema } from "@modules/checkout/schemas/transaction.schema";

export type Transaction = z.infer<typeof TransactionSchema>;
