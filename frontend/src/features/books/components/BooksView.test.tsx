import { fireEvent, render, screen, within } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { getBookOwners } from "@/lib/api/client";
import { BooksView } from "./BooksView";

vi.mock("@/lib/api/client");

const owners = [
  {
    name: "Jane",
    age: 23,
    books: [
      { name: "Wuthering Heights", type: "Paperback" },
      { name: "Hamlet", type: "Hardcover" },
    ],
  },
  { name: "Charles", age: 17, books: [{ name: "The Hobbit", type: "Ebook" }] },
];

function titlesUnder(heading: string) {
  const section = screen.getByRole("heading", { name: heading }).closest("section");
  return within(section!)
    .getAllByRole("listitem")
    .map((item) => item.textContent);
}

function click(name: string) {
  fireEvent.click(screen.getByRole("button", { name }));
}

beforeEach(() => {
  vi.mocked(getBookOwners).mockReset();
  vi.spyOn(console, "error").mockImplementation(() => {});
});

describe("BooksView", () => {
  it("shows all books grouped and sorted when the page opens", async () => {
    vi.mocked(getBookOwners).mockResolvedValue(owners);
    render(<BooksView />);

    await screen.findByText("Hamlet");
    expect(getBookOwners).toHaveBeenCalledTimes(1);
    expect(titlesUnder("Books owned by Adults")).toEqual(["Hamlet", "Wuthering Heights"]);
    expect(titlesUnder("Books owned by Children")).toEqual(["The Hobbit"]);
  });

  it("fetches fresh data on Get Books", async () => {
    vi.mocked(getBookOwners)
      .mockResolvedValueOnce(owners)
      .mockResolvedValueOnce([
        { name: "Newer", age: 30, books: [{ name: "Dune", type: "Ebook" }] },
      ]);
    render(<BooksView />);
    await screen.findByText("Hamlet");

    click("Get Books");

    await screen.findByText("Dune");
    expect(screen.queryByText("Hamlet")).toBeNull();
  });

  it("shows only hardcover books and changes the headings on Hardcover only", async () => {
    vi.mocked(getBookOwners).mockResolvedValue(owners);
    render(<BooksView />);
    await screen.findByText("Hamlet");

    click("Hardcover only");

    await screen.findByText("Hamlet");
    expect(titlesUnder("Hardcover Books owned by Adults")).toEqual(["Hamlet"]);
    expect(
      screen.getByRole("heading", { name: "Hardcover Books owned by Children" }),
    ).toBeDefined();
    expect(screen.getByText("No books to show.")).toBeDefined();
  });

  it("returns to all books when Get Books follows Hardcover only", async () => {
    vi.mocked(getBookOwners).mockResolvedValue(owners);
    render(<BooksView />);
    await screen.findByText("Hamlet");

    click("Hardcover only");
    await screen.findByText("Hamlet");
    click("Get Books");

    await screen.findByText("Wuthering Heights");
    expect(screen.getByRole("heading", { name: "Books owned by Adults" })).toBeDefined();
  });

  it("shows a loading message while fetching", () => {
    vi.mocked(getBookOwners).mockReturnValue(new Promise(() => {}));
    render(<BooksView />);

    expect(screen.getByRole("status").textContent).toBe("Loading books...");
  });

  it("shows an error when the first load fails, and recovers with Try again", async () => {
    vi.mocked(getBookOwners).mockRejectedValueOnce(new Error("502")).mockResolvedValue(owners);
    render(<BooksView />);

    await screen.findByRole("alert");
    click("Try again");

    await screen.findByText("Wuthering Heights");
    expect(screen.queryByRole("alert")).toBeNull();
  });

  it("shows an error and recovers with Try again, keeping the filter", async () => {
    vi.mocked(getBookOwners)
      .mockResolvedValueOnce(owners)
      .mockRejectedValueOnce(new Error("502"))
      .mockResolvedValue(owners);
    render(<BooksView />);
    await screen.findByText("Hamlet");

    click("Hardcover only");
    await screen.findByRole("alert");
    click("Try again");

    await screen.findByText("Hamlet");
    expect(screen.queryByRole("alert")).toBeNull();
    expect(screen.queryByText("Wuthering Heights")).toBeNull();
  });

  it("ignores a slow response when a newer request has started", async () => {
    let resolveFirst: (value: typeof owners) => void = () => {};
    vi.mocked(getBookOwners)
      .mockReturnValueOnce(new Promise((resolve) => (resolveFirst = resolve)))
      .mockResolvedValueOnce([
        { name: "Newer", age: 30, books: [{ name: "Dune", type: "Ebook" }] },
      ]);
    render(<BooksView />);

    click("Get Books");
    await screen.findByText("Dune");
    resolveFirst(owners);
    await Promise.resolve();

    expect(screen.queryByText("Hamlet")).toBeNull();
  });
});
