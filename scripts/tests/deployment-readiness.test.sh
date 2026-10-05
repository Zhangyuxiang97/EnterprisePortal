#!/usr/bin/env bash
set -euo pipefail
project_root=$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)
# 只提取验证阶段，所有外部命令均由函数替代，不访问 Docker 或网络。
verification=$(sed -n '/^# 等待MySQL初始化完成/,/^# 第九步：配置防火墙/p' "$project_root/deploy-ubuntu22-docker.sh")
helpers=$(cat <<'EOF'
set -e
MYSQL_ROOT_PASSWORD=unused-test-value
PORTAL_HTTP_PORT=8082
print_info() { :; }
print_error() { echo "$1"; }
sleep() { :; }
docker() {
    if [[ "$*" == *mysqladmin* ]]; then
        [[ "$SCENARIO" != mysql_failure ]]
    else
        if [ "$SCENARIO" = empty_schema ]; then echo 0; else echo 10; fi
    fi
}
curl() {
    if [[ "$*" == *health/ready* ]]; then
        [[ "$SCENARIO" != api_failure ]]
    else
        [[ "$SCENARIO" != portal_failure ]]
    fi
}
EOF
)
for scenario in success mysql_failure api_failure portal_failure empty_schema; do
    if output=$(SCENARIO="$scenario" bash -c "$helpers
$verification
echo verification_complete" 2>&1); then
        [ "$scenario" = success ] || { echo "失败场景被误报成功：$scenario"; exit 1; }
        [[ "$output" == *verification_complete* ]] || exit 1
    else
        [ "$scenario" != success ] || { echo "正常场景验证失败"; exit 1; }
        [[ "$output" != *verification_complete* ]] || exit 1
    fi
done
echo '部署验证：正常就绪、MySQL 超时、API 错误、门户错误和空数据库场景通过'
