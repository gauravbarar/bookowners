import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Books",
  description: "Books grouped by the age category of their owner",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
