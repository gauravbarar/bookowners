# Owners and Books

This is a React and Next.js client that shows books, organized by the owner's age.
When you open the page, all books are listed alphabetically. They're split into "Books owned by Adults" (ages 18 and up) and "Books owned by Children" (ages 17 and under). If you click `Hardcover only`, the list filters to just hardcover books, and the headings change to reflect that.
Both the `Get Books` and `Hardcover only` buttons fetch the latest data from the API. So "Get Books" also works as a refresh. This behavior is intentional; the page won't update its data unless you click one of those buttons.

See [ASSUMPTIONS.md](ASSUMPTIONS.md) for the decisions made along the way.

## Requires the book owners API

The browser calls the BE API directly. The API must allow
FE's origin in its CORS settings. It needs to serve `GET /api/v1/bookowners`:

```json
[{ "name": "Jane", "age": 23, "books": [{ "name": "Hamlet", "type": "Hardcover" }] }]
```

`NEXT_PUBLIC_API_BASE_URL` sets the API address and defaults to `http://localhost:5080`. It must be
the address the browser uses, and it is compiled into the browser code, so changing it means
rebuilding.

## API contract

`@books/api-client` holds the API's types. The backend generates and versions it, and the `.tgz` in
`vendor/` is the installed copy. `src/features/books/schema.ts` still validates responses at runtime,
and `satisfies` makes `npm run typecheck` fail if that schema stops matching the package.

To upgrade, copy the new `.tgz` into `vendor/`, delete the old one, and run:

```bash
npm install ./vendor/books-api-client-<version>.tgz
npm run typecheck
```

## Run with Docker

```bash
docker compose up --build -d
```

Open http://localhost:3000. Stop it with `docker compose down`.

To use a different API, set the URL when building:

```bash
NEXT_PUBLIC_API_BASE_URL=https://api.example.com docker compose up --build -d
```

To build only the image: `docker build -t books-frontend .`

## Run for local development

Needs Node.js 24.

```bash
cp .env.example .env.local
npm install
npm run dev
```

## Error handling

If the API fails, times out or returns data in an unexpected shape, the page shows an error message
with a **Try again** button that keeps the current filter. Partial data is never shown.

## Scripts

`npm run dev`, `build`, `lint`, `typecheck`, `test`, `format`.
