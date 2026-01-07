import * as z from "zod";

export const RawDateOnlySchema = z.iso.date().brand<"RawDateOnly">();

export const RawDateTimeSchema = z
  .union([
    z.iso.datetime({ offset: true }),
    z.iso.datetime({ offset: false }),
    z.iso.datetime({ local: true }),
  ])
  .brand<"RawDateTime">();
