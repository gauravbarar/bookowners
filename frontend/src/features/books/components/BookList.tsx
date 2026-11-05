import styles from "./BookList.module.css";

type BookListProps = {
  heading: string;
  /** Undefined until books have loaded. */
  titles?: string[];
};

export function BookList({ heading, titles }: BookListProps) {
  return (
    <section className={styles.card}>
      <h2 className={styles.heading}>{heading}</h2>
      {titles && titles.length === 0 && <p className={styles.empty}>No books to show.</p>}
      {titles && titles.length > 0 && (
        <ul className={styles.list}>
          {titles.map((title, index) => (
            <li key={`${title}-${index}`}>{title}</li>
          ))}
        </ul>
      )}
    </section>
  );
}
