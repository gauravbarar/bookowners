# Assumptions for this exercise:

- The client handles all the **grouping**. The brief mentioned the server returning "similar" data, so I'm keeping the API shape as is. The client then takes care of grouping, sorting, and filtering.
- Books load right when the page opens. The brief only talked about buttons, so I decided to show all books on load. This way, the first screen isn't empty.
- **The "Get Books" action also works as a refresh**. Both buttons always fetch the newest data from the API, bypassing the browser cache. They don't just reuse what's already on screen. So, if data changes on the server, the next click will show it. The page itself doesn't update automatically because the brief asked for button-driven actions, not live updates.
- **Duplicates are kept**. If you own multiple copies of a title, it shows up for each copy. Also, the same title can appear in both the "Adults" and "Children" sections, just like in the screenshots.
- **Sorting** ignores whether letters are uppercase or lowercase. "Hardcover" is matched exactly to the book's type.
- For the layout, mobile cards have a border and shadow, but desktop columns don't. This matches the screenshots. Once the screen width goes above 600px, the layout switches to two columns.
- The API contract comes from a vendored package. Since there's no private registry yet, the backend's `@books/api-client` package is stored in `vendor/`. The app will only see an API contract change after that package gets upgraded.
- The browser calls the API directly. This means the API address is set when the app is built, and the API needs to allow this app's origin through CORS.