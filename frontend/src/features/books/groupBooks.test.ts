import { describe, expect, it } from "vitest";
import { groupBooks } from "./groupBooks";
import type { BookOwner } from "./schema";

const owners: BookOwner[] = [
  {
    name: "Adult at the boundary",
    age: 18,
    books: [
      { name: "Wuthering Heights", type: "Paperback" },
      { name: "hamlet", type: "Hardcover" },
    ],
  },
  { name: "Child at the boundary", age: 17, books: [{ name: "The Hobbit", type: "Ebook" }] },
  { name: "Older adult", age: 40, books: [{ name: "Jane Eyre", type: "Hardcover" }] },
];

describe("groupBooks", () => {
  it("treats 18 as an adult and 17 as a child", () => {
    const { adults, children } = groupBooks(owners, false);

    expect(adults).toContain("Wuthering Heights");
    expect(children).toEqual(["The Hobbit"]);
  });

  it("sorts alphabetically across owners, ignoring case", () => {
    expect(groupBooks(owners, false).adults).toEqual(["hamlet", "Jane Eyre", "Wuthering Heights"]);
  });

  it("keeps only hardcover books when filtered", () => {
    expect(groupBooks(owners, true)).toEqual({ adults: ["hamlet", "Jane Eyre"], children: [] });
  });

  it("keeps a title once per copy owned", () => {
    const sameBook = { name: "Hamlet", type: "Hardcover" };
    const twoOwners = [
      { name: "A", age: 30, books: [sameBook] },
      { name: "B", age: 31, books: [sameBook] },
    ];

    expect(groupBooks(twoOwners, false).adults).toEqual(["Hamlet", "Hamlet"]);
  });

  it("does not change the data passed in", () => {
    const before = JSON.stringify(owners);

    groupBooks(owners, true);

    expect(JSON.stringify(owners)).toBe(before);
  });
});
