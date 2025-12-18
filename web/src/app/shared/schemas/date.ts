import * as z from "zod";

export const RawDateOnlySchema = z.iso.date().brand<"RawDateOnly">();

export const RawDateTimeSchema = z.iso.datetime().brand<"RawDateTime">();

export const DateOnlySchema = RawDateOnlySchema.pipe(z.coerce.date()).brand<"DateOnly">();

export const DateTimeSchema = RawDateTimeSchema.pipe(z.coerce.date()).brand<"DateTime">();
