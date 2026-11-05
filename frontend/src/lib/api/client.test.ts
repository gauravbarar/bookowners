import { afterEach, describe, expect, it, vi } from "vitest";
import { ApiError, getBookOwners } from "./client";

const signal = new AbortController().signal;

function stubFetch(body: string, status = 200) {
  vi.stubGlobal("fetch", vi.fn().mockResolvedValue(new Response(body, { status })));
}

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("getBookOwners", () => {
  it("returns owners from a valid response", async () => {
    stubFetch('[{"name":"Jane","age":23,"books":[{"name":"Hamlet","type":"Hardcover"}]}]');

    await expect(getBookOwners(signal)).resolves.toEqual([
      { name: "Jane", age: 23, books: [{ name: "Hamlet", type: "Hardcover" }] },
    ]);
  });

  it("bypasses the browser cache so each request reaches the server", async () => {
    stubFetch("[]");

    await getBookOwners(signal);

    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining("/api/v1/bookowners"),
      expect.objectContaining({ cache: "no-store" }),
    );
  });

  it("rejects an error status", async () => {
    stubFetch('{"title":"Bad gateway"}', 502);

    await expect(getBookOwners(signal)).rejects.toBeInstanceOf(ApiError);
  });

  it("rejects data in the wrong shape", async () => {
    stubFetch('[{"name":"Charles","AGE":17,"":[]}]');

    await expect(getBookOwners(signal)).rejects.toBeInstanceOf(ApiError);
  });

  it("rejects a body that is not JSON", async () => {
    stubFetch("");

    await expect(getBookOwners(signal)).rejects.toBeInstanceOf(ApiError);
  });
});
