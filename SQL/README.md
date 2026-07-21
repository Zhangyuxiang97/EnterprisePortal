# 海隆咨询数据库初始化说明

## 文件说明

**重要提示：** 所有 SQL 文件已按执行顺序编号（00-08），MySQL Docker 容器会按字母顺序自动执行。

### 0. 00_set_charset.sql
**字符集设置文件**，统一使用 `utf8mb4`，应在导入其他文件前执行。

### 1. 01_hailong_consulting_schema.sql
**数据库结构文件**，包含：
- 数据库创建语句
- 所有表的创建语句（CREATE TABLE）
- 索引和视图定义

### 2. 02_hailong_consulting_init_data.sql
**主初始化数据文件**，包含：
- 用户账号数据（管理员和测试账号）
- 企业简介、业务范围、资质荣誉等基础数据
- 示例公告和信息发布数据
- 友情链接数据
- 区域字典由 03、04、05 三批初始化文件统一导入，本文件不重复导入

### 3. 03_region_dictionary_batch1_henan.sql
**区域字典初始化第一批**：河南省完整省市区县数据，共 140 条唯一区域编码。

### 4. 04_region_dictionary_batch2_north_east_east.sql
**区域字典初始化第二批**：华北、东北、华东区域数据（含安徽省），共 998 条唯一区域编码。

### 5. 05_region_dictionary_batch3_central_south_west.sql
**区域字典初始化第三批**：华中、华南、西南、西北及港澳台区域数据，共 1322 条唯一区域编码。

### 6. 06_announcements_bidding.sql
**招标公告数据**，1103条

### 7. 07_announcements_result.sql
**中标公示数据**，1061条

### 8. 08_announcements_correction.sql
**变更公告数据**，211条

公告 SQL 已在生成阶段直接写入区域编码，首次导入后无需再执行区域转换脚本。

如需从 `scripts/data/final-data.json` 重新生成公告 SQL，可执行：

```bash
cd scripts
npm run generate-initial-sql
```

> 数据来源：henanhailong.com，已清洗（HTML清理、区域映射、字段提取）
> **总计：2375条**
> 当前批次包含河南省 2323 条，以及已核实的北京、河北、内蒙古、辽宁、浙江、安徽、湖北、陕西、甘肃项目 52 条。

## 执行顺序

**Docker 部署：** 文件已按执行顺序编号（00-08），MySQL 容器启动时会自动按顺序执行，无需手动操作。

**手动执行顺序：**

```bash
# 0. 设置字符集
mysql -u root -p < 00_set_charset.sql

# 1. 首先执行数据库结构文件
mysql -u root -p < 01_hailong_consulting_schema.sql

# 2. 执行主初始化数据文件
mysql -u root -p hailong_consulting < 02_hailong_consulting_init_data.sql

# 3. 执行河南、安徽区域初始化
mysql -u root -p hailong_consulting < 03_region_dictionary_batch1_henan.sql

# 4. 执行第二批区域初始化
mysql -u root -p hailong_consulting < 04_region_dictionary_batch2_north_east_east.sql

# 5. 执行第三批区域初始化
mysql -u root -p hailong_consulting < 05_region_dictionary_batch3_central_south_west.sql

# 6. 导入招标公告
mysql -u root -p hailong_consulting < 06_announcements_bidding.sql

# 7. 导入中标公示
mysql -u root -p hailong_consulting < 07_announcements_result.sql

# 8. 导入变更公告
mysql -u root -p hailong_consulting < 08_announcements_correction.sql

```

或者在MySQL客户端中执行：

```sql
SOURCE /path/to/00_set_charset.sql;
SOURCE /path/to/01_hailong_consulting_schema.sql;
SOURCE /path/to/02_hailong_consulting_init_data.sql;
SOURCE /path/to/03_region_dictionary_batch1_henan.sql;
SOURCE /path/to/04_region_dictionary_batch2_north_east_east.sql;
SOURCE /path/to/05_region_dictionary_batch3_central_south_west.sql;
SOURCE /path/to/06_announcements_bidding.sql;
SOURCE /path/to/07_announcements_result.sql;
SOURCE /path/to/08_announcements_correction.sql;
```

## 数据覆盖范围

执行完所有SQL文件后，将包含：

### 省级行政区（34个）
1. 河南省（已含详细区县数据）
2. 安徽省（16个地级市及现行区县）
3. 北京市、天津市、上海市、重庆市（4个直辖市）
4. 河北、山西、内蒙古（华北地区）
5. 辽宁、吉林、黑龙江（东北地区）
6. 江苏、浙江、福建、江西、山东（华东地区）
7. 湖北、湖南、广东、广西、海南（华中、华南地区）
8. 四川、贵州、云南、西藏（西南地区）
9. 陕西、甘肃、青海、宁夏、新疆（西北地区）
10. 香港、澳门、台湾（特别行政区）

### 市级行政区
- 全国所有地级市、自治州、地区
- 共计约340+个市级行政区

### 区县级行政区
- **河南省**：所有区县（完整数据）
- **全国各批次覆盖省级区域**：按国家行政区划编码整理，批次之间无重复编码
- **数据特点**：市辖区合并为"市区"，县单独列出
- 共计约1800+个区县数据

### 历史公告数据
- 招标公告：1103条
- 中标公示：1061条
- 变更公告：211条
- 总计：2375条

## 数据统计

执行完成后，可以运行以下查询查看统计信息：

```sql
-- 查看各层级数据量
SELECT 
    CASE region_level
        WHEN 1 THEN '省级'
        WHEN 2 THEN '市级'
        WHEN 3 THEN '区县级'
    END AS '行政层级',
    COUNT(*) AS '数量'
FROM region_dictionary 
WHERE is_deleted = 0 
GROUP BY region_level 
ORDER BY region_level;

-- 查看公告数据统计
SELECT 
    notice_type AS '公告类型',
    COUNT(*) AS '数量'
FROM announcements
WHERE is_deleted = 0
GROUP BY notice_type;
```

## 注意事项

1. **执行前确认**：确保已经创建了数据库表结构
2. **字符编码**：确保数据库和表使用UTF-8编码
3. **数据冲突**：如果已有数据，可能会出现主键冲突，建议在空数据库中执行
4. **区县数据**：三批区域文件均按统一编码写入，河南、安徽为完整市县级数据
5. **数据更新**：行政区划可能会有调整，使用时请注意数据的时效性
6. **历史数据**：06、07、08 三个公告脚本包含从旧网站抓取的历史公告，数据来源为 henanhailong.com；公告 SQL 已直接写入区域编码，无需额外后处理脚本

## 数据格式说明

### 区县数据格式
本项目采用以下格式处理区县数据：

1. **市辖区合并**：将一个城市的所有市辖区合并为"XX市区"
   ```sql
   ('320101', '南京市区', 3, '320100', 1),  -- 包含玄武区、秦淮区等所有市辖区
   ```

2. **县单独列出**：县级行政区单独列出
   ```sql
   ('320118', '溧水区', 3, '320100', 2),     -- 原为县，现为区，但单独列出
   ('320117', '高淳区', 3, '320100', 3),     -- 原为县，现为区，但单独列出
   ```

3. **县级市**：县级市也单独列出
   ```sql
   ('320581', '常熟市', 3, '320500', 2),     -- 县级市
   ```

## 技术支持

如有问题，请参考：
- 数据库设计文档
- 项目README.md
- 或联系技术支持团队

---

**最后更新时间**：2026-07-16
