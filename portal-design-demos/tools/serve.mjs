import http from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const projectRoot = fileURLToPath(new URL('../', import.meta.url));
const root = path.resolve(projectRoot, process.argv.includes('--dist') ? 'dist' : '.');
const port = Number(process.env.PORT || 4177);
const types = { '.html': 'text/html', '.js': 'text/javascript', '.css': 'text/css', '.svg': 'image/svg+xml', '.png': 'image/png', '.ico': 'image/x-icon' };
const server = http.createServer(async (req, res) => {
  try {
    const relative = decodeURIComponent(new URL(req.url, 'http://localhost').pathname).replace(/^\/+/, '') || 'index.html';
    const target = path.resolve(root, relative);
    if (!target.startsWith(root + path.sep) || !['GET', 'HEAD'].includes(req.method)) {
      res.writeHead(403).end('Forbidden'); return;
    }
    if (!(await stat(target)).isFile()) { res.writeHead(404).end('Not found'); return; }
    const content = await readFile(target);
    res.writeHead(200, { 'Content-Type': (types[path.extname(target)] || 'application/octet-stream') + (['.html', '.js', '.css', '.svg'].includes(path.extname(target)) ? '; charset=utf-8' : ''), 'Cache-Control': 'no-store' });
    res.end(req.method === 'HEAD' ? undefined : content);
  } catch { res.writeHead(404).end('Not found'); }
});
server.listen(port, '127.0.0.1', () => console.log(`海隆门户设计演示 http://127.0.0.1:${port}  (${root})`));
server.on('error', error => { console.error(error.message); process.exitCode = 1; });
