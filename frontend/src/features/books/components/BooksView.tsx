"use client";

import { groupBooks } from "../groupBooks";
import { useBooks } from "../hooks/useBooks";
import { BookList } from "./BookList";
import styles from "./BooksView.module.css";

export function BooksView() {
  const { state, hardcoverOnly, loadBooks } = useBooks();

  const books = state.status === "success" ? groupBooks(state.owners, hardcoverOnly) : undefined;
  const headingPrefix = hardcoverOnly ? "Hardcover Books" : "Books";

  return (
    <>
      <div className={styles.lists}>
        <BookList heading={`${headingPrefix} owned by Adults`} titles={books?.adults} />
        <BookList heading={`${headingPrefix} owned by Children`} titles={books?.children} />
      </div>

      {state.status === "loading" && (
        <p role="status" className={styles.message}>
          Loading books...
        </p>
      )}

      {state.status === "error" && (
        <div role="alert" className={styles.error}>
          <p className={styles.message}>We couldn&apos;t load the books.</p>
          <button type="button" className={styles.link} onClick={() => loadBooks(hardcoverOnly)}>
            Try again
          </button>
        </div>
      )}

      <hr className={styles.divider} />

      <div className={styles.actions}>
        <button type="button" className={styles.link} onClick={() => loadBooks(true)}>
          Hardcover only
        </button>
        <button type="button" className={styles.primary} onClick={() => loadBooks(false)}>
          Get Books
        </button>
      </div>
    </>
  );
}
