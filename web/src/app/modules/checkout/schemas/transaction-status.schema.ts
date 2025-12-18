import * as z from "zod";

export const TransactionStatusSchema = z.enum(["available", "expired", "canceled"]);
