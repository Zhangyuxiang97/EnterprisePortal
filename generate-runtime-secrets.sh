#!/usr/bin/env bash
set -euo pipefail

secrets_dir=".runtime"
secrets_file="$secrets_dir/secrets.env"

if [[ -e "$secrets_file" ]]; then
  echo "Runtime secret file already exists: $secrets_file"
  echo "It was left unchanged. Remove it explicitly only when rotating credentials."
  exit 0
fi

mkdir -p "$secrets_dir"
umask 077

mysql_root_password=$(openssl rand -base64 32 | tr -d '\n')
mysql_app_password=$(openssl rand -base64 32 | tr -d '\n')
jwt_key=$(openssl rand -base64 48 | tr -d '\n')

cat > "$secrets_file" <<EOF
MYSQL_ROOT_PASSWORD=$mysql_root_password
MYSQL_PASSWORD=$mysql_app_password
ConnectionStrings__DefaultConnection=Server=mysql;Port=3306;Database=hailong_consulting;User=hailong_app;Password=$mysql_app_password;CharSet=utf8mb4;
Jwt__Key=$jwt_key
EOF

chmod 600 "$secrets_file"
echo "Runtime secrets were generated at $secrets_file (mode 600)."
echo "========== Docker 运行凭据（请立即保存） =========="
echo "MySQL root 密码: $mysql_root_password"
echo "MySQL 应用密码: $mysql_app_password"
echo "JWT Key: $jwt_key"
echo "凭据文件: $secrets_file"
echo "================================================="
echo "Run: docker compose up -d --build"
echo "Do not commit this file or share terminal output."
