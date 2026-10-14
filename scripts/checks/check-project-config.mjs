import { readFileSync, existsSync } from 'node:fs'
import assert from 'node:assert/strict'
import { fileURLToPath } from 'node:url'
import path from 'node:path'

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../..')
const read = file => readFileSync(path.join(root, file), 'utf8')
const defaults = JSON.parse(read('config/portal-defaults.json'))
const staticConfig = JSON.parse(read('hailong-protral/config/site-config.json'))
for (const key of ['company', 'contact', 'transportation', 'faqs']) {
  assert.ok(defaults[key], `默认展示配置缺少 ${key}`)
  assert.ok(!(key in staticConfig), `静态配置不应重复维护 ${key}`)
}
assert.ok(defaults.company.slogan.trim())
assert.ok(Array.isArray(defaults.faqs))
assert.match(read('hailong-protral/src/utils/config.js'), /config\/portal-defaults\.json/)
assert.match(read('nginx/Dockerfile'), /COPY config\/portal-defaults\.json/)
assert.match(read('BackEnd/HailongConsulting.API/HailongConsulting.API.csproj'), /EmbeddedResource Include="\.\.\/\.\.\/config\/portal-defaults\.json"/)
assert.match(read('BackEnd/HailongConsulting.API/Dockerfile'), /COPY config\/portal-defaults\.json \/src\/config\/portal-defaults\.json/)
assert.match(read('docker-compose.yml'), /context: \.\s+dockerfile: BackEnd\/HailongConsulting.API\/Dockerfile/)
assert.ok(!read('docker-compose.yml').includes('init-announcements:'), '已有库不应按公告数量自动重导')
assert.match(read('docker-compose.yml'), /\.\/SQL\/init:\/docker-entrypoint-initdb\.d:ro/)
assert.match(read('.gitignore'), /^\.env$/m, '本机运行凭据不进入版本管理')
assert.match(read('docker-compose.yml'), /\$\{PORTAL_HTTP_PORT:-8082\}:80/)
assert.match(read('deploy-ubuntu22-docker.sh'), /health\/ready/)
assert.ok(!/up -d --build|^\$COMPOSE_CMD.* down/m.test(read('deploy-ubuntu22-docker.sh').split('cat > /root/')[0]), '更新流程不能提前停止容器')
for (const module of staticConfig.homeModules) {
  assert.ok(existsSync(path.join(root, `hailong-protral/src/components/home/modules/${module.component}.vue`)), `首页模块不存在：${module.component}`)
}
console.log('默认配置单一来源、Docker 构建引用、部署入口和首页模块检查通过')
