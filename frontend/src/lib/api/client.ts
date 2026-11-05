import { bookOwnersSchema, type BookOwner } from "@/features/books/schema";

const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";
const requestTimeoutMs = 30_000;

export class ApiError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "ApiError";
  }
}

export async function getBookOwners(signal: AbortSignal): Promise<BookOwner[]> {
  const response = await fetch(`${baseUrl}/api/v1/bookowners`, {
    cache: "no-store",
    signal: AbortSignal.any([signal, AbortSignal.timeout(requestTimeoutMs)]),
  });

  if (!response.ok) {
    throw new ApiError(`API request failed with status ${response.status}`);
  }

  // A body that isn't JSON becomes undefined, so it fails validation like any other bad shape.
  const body: unknown = await response.json().catch(() => undefined);
  const result = bookOwnersSchema.safeParse(body);

  if (!result.success) {
    throw new ApiError("API returned data in an unexpected shape");
  }

  return result.data;
}
