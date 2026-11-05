import type { BookOwner } from "./schema";

const ADULT_AGE = 18;
const HARDCOVER = "Hardcover";

const titleCollator = new Intl.Collator("en", { sensitivity: "base" });

export type GroupedBooks = {
  adults: string[];
  children: string[];
};

function bookTitles(owners: BookOwner[], hardcoverOnly: boolean): string[] {
  return owners
    .flatMap((owner) => owner.books)
    .filter((book) => !hardcoverOnly || book.type === HARDCOVER)
    .map((book) => book.name)
    .sort(titleCollator.compare);
}

export function groupBooks(owners: BookOwner[], hardcoverOnly: boolean): GroupedBooks {
  return {
    adults: bookTitles(
      owners.filter((owner) => owner.age >= ADULT_AGE),
      hardcoverOnly,
    ),
    children: bookTitles(
      owners.filter((owner) => owner.age < ADULT_AGE),
      hardcoverOnly,
    ),
  };
}
