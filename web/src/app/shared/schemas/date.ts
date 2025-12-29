import * as z from "zod";

export const RawDateOnlySchema = z.iso.date().brand<"RawDateOnly">();

export const RawDateTimeSchema = z.iso.datetime().brand<"RawDateTime">();
