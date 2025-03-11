const { createServer } = require('http');
const { parse } = require('url');
const next = require('next');

const app = next({ dev: false });
const handle = app.getRequestHandler();

app.prepare().then(() => {
    createServer((req, res) => {
        // Parse request url to handle custom routes if needed
        const parsedUrl = parse(req.url, true);
        handle(req, res, parsedUrl);
    }).listen(process.env.PORT || 3001, (err) => {
        if (err) throw err;
        console.log(`> Ready on http://localhost:${process.env.PORT || 3001}`);
    });
});