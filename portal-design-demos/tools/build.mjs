import { cp, mkdir } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
const root = fileURLToPath(new URL('../', import.meta.url));
await mkdir(path.join(root, 'dist'), { recursive: true });
for (const name of ['index.html', 'styles.css', 'app.js', 'data.js', 'assets']) {
  await cp(path.join(root, name), path.join(root, 'dist', name), { recursive: true });
}
console.log('静态构建完成：dist/index.html，可直接打开或放到静态服务器。');
