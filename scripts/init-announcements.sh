#!/bin/bash
# ============================================
# 初始化公告数据脚本
# 在 MySQL 容器启动后执行，导入真实公告数据
# ============================================

set -e

echo "=========================================="
echo "公告数据初始化脚本"
echo "=========================================="

# 等待 MySQL 就绪
echo "[1/4] 等待 MySQL 就绪..."
until docker exec hailong-mysql mysqladmin ping -h localhost -u root -p"$(grep MYSQL_ROOT_PASSWORD /home/sean/projects/friend-dev/EnterprisePortal/.runtime/secrets.env | cut -d= -f2)" --silent 2>/dev/null; do
    echo "  MySQL 尚未就绪，等待 5 秒..."
    sleep 5
done
echo "  ✅ MySQL 已就绪"

# 检查是否已有数据
echo "[2/4] 检查公告数据..."
COUNT=$(docker exec hailong-mysql mysql -u hailong_app -p"$(grep MYSQL_PASSWORD /home/sean/projects/friend-dev/EnterprisePortal/.runtime/secrets.env | cut -d= -f2)" -e "SELECT COUNT(*) FROM hailong_consulting.announcements;" -s -N 2>/dev/null)
echo "  当前公告数量: $COUNT"

if [ "$COUNT" -gt 100 ]; then
    echo "  ✅ 公告数据已存在，跳过导入"
else
    # 导入公告数据
    echo "[3/4] 导入公告数据..."
    for sql_file in 07_announcements_bidding.sql 08_announcements_result.sql 09_announcements_correction.sql; do
        docker exec -i hailong-mysql mysql -u hailong_app -p"$(grep MYSQL_PASSWORD /home/sean/projects/friend-dev/EnterprisePortal/.runtime/secrets.env | cut -d= -f2)" hailong_consulting < "/home/sean/projects/friend-dev/EnterprisePortal/SQL/$sql_file"
    done
fi

# 无论是否为新导入，都统一公告区域字段为行政区划编码。
echo "[4/4] 统一公告区域编码..."
docker exec -i hailong-mysql mysql -u hailong_app -p"$(grep MYSQL_PASSWORD /home/sean/projects/friend-dev/EnterprisePortal/.runtime/secrets.env | cut -d= -f2)" hailong_consulting < /home/sean/projects/friend-dev/EnterprisePortal/SQL/10_normalize_announcement_region_codes.sql

# 验证导入结果
COUNT=$(docker exec hailong-mysql mysql -u hailong_app -p"$(grep MYSQL_PASSWORD /home/sean/projects/friend-dev/EnterprisePortal/.runtime/secrets.env | cut -d= -f2)" -e "SELECT COUNT(*) FROM hailong_consulting.announcements;" -s -N 2>/dev/null)
echo "  ✅ 导入完成，当前公告数量: $COUNT"

echo "=========================================="
echo "公告数据初始化完成"
echo "=========================================="
