import * as z from "zod";

import { PixPaymentAttemptSchema } from "../schemas/pix-payment-attempt.schema";

export type PixPaymentAttempt = z.infer<typeof PixPaymentAttemptSchema>;
