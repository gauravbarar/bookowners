import { BooksView } from "@/features/books/components/BooksView";
import styles from "./page.module.css";

export default function HomePage() {
  return (
    <>
      <header className={styles.header}>
        <h1 className={styles.title}>Owners and Books</h1>
      </header>
      <main className={styles.main}>
        <BooksView />
      </main>
    </>
  );
}
