"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { getBookOwners } from "@/lib/api/client";
import type { BookOwner } from "../schema";

type BooksState =
  { status: "loading" } | { status: "success"; owners: BookOwner[] } | { status: "error" };

export function useBooks() {
  const [state, setState] = useState<BooksState>({ status: "loading" });
  const [hardcoverOnly, setHardcoverOnly] = useState(false);
  const activeRequest = useRef<AbortController | null>(null);

  const fetchBooks = useCallback(() => {
    // Cancel any earlier request so a slow response can't overwrite a newer one.
    activeRequest.current?.abort();
    const controller = new AbortController();
    activeRequest.current = controller;

    getBookOwners(controller.signal)
      .then((owners) => {
        if (!controller.signal.aborted) {
          setState({ status: "success", owners });
        }
      })
      .catch((error: unknown) => {
        if (!controller.signal.aborted) {
          console.error("Failed to load books", error);
          setState({ status: "error" });
        }
      });
  }, []);

  const loadBooks = useCallback(
    (showHardcoverOnly: boolean) => {
      setHardcoverOnly(showHardcoverOnly);
      setState({ status: "loading" });
      fetchBooks();
    },
    [fetchBooks],
  );

  useEffect(() => {
    fetchBooks();

    return () => activeRequest.current?.abort();
  }, [fetchBooks]);

  return { state, hardcoverOnly, loadBooks };
}
