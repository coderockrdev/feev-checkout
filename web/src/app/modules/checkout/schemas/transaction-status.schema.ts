import * as z from "zod";

import { TransactionStatus } from "@modules/checkout/enums/transaction-status";

export const TransactionStatusSchema = z.enum(TransactionStatus);
