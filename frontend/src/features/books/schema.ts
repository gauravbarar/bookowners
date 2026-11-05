import type { Book, BookOwner as ApiBookOwner } from "@books/api-client";
import { z } from "zod";

// "satisfies" fails the typecheck if these schemas stop matching the API's published contract.
const bookSchema = z.object({
  name: z.string(),
  type: z.string(),
}) satisfies z.ZodType<Book>;

const bookOwnerSchema = z.object({
  name: z.string(),
  age: z.number(),
  books: z.array(bookSchema),
}) satisfies z.ZodType<ApiBookOwner>;

export const bookOwnersSchema = z.array(bookOwnerSchema);

export type BookOwner = z.infer<typeof bookOwnerSchema>;
