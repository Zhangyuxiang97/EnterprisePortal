-- ============================================
-- 海隆咨询官网数据库结构设计
-- 数据库名：hailong_consulting
-- MySQL版本：8.0+
-- 创建时间：2025-12-03
-- 更新时间：2025-12-08
-- ============================================

-- 设置字符集
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

-- 创建数据库
CREATE DATABASE IF NOT EXISTS `hailong_consulting` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE `hailong_consulting`;

-- ============================================
-- 0. 附件管理模块（新增）
-- ============================================

-- 0.1 附件表（统一管理所有附件和图片）
CREATE TABLE `attachments` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '附件ID',
  `file_name` VARCHAR(255) NOT NULL COMMENT '文件名称',
  `file_path` VARCHAR(500) NOT NULL COMMENT '文件路径',
  `file_url` VARCHAR(500) NOT NULL COMMENT '文件访问URL',
  `file_size` BIGINT UNSIGNED DEFAULT NULL COMMENT '文件大小（字节）',
  `file_type` VARCHAR(200) DEFAULT NULL COMMENT '文件类型（MIME类型）',
  `file_extension` VARCHAR(20) DEFAULT NULL COMMENT '文件扩展名',
  `category` VARCHAR(50) DEFAULT NULL COMMENT '附件分类：image-图片, document-文档, video-视频, other-其他',
  `related_type` VARCHAR(50) DEFAULT NULL COMMENT '关联类型：announcement-公告, info_publication-信息发布, company_profile-企业简介等',
  `related_id` INT UNSIGNED DEFAULT NULL COMMENT '关联记录ID',
  `upload_user_id` INT UNSIGNED DEFAULT NULL COMMENT '上传用户ID',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_related_type_id` (`related_type`, `related_id`),
  INDEX `idx_category` (`category`),
  INDEX `idx_upload_user_id` (`upload_user_id`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='附件表（统一管理所有附件和图片）';

-- ============================================
-- 1. 用户权限管理模块
-- ============================================

-- 1.1 用户表（用于API认证）
CREATE TABLE `users` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '用户ID',
  `username` VARCHAR(50) NOT NULL UNIQUE COMMENT '用户名',
  `password` VARCHAR(255) NOT NULL COMMENT '密码（加密存储）',
  `email` VARCHAR(100) NOT NULL COMMENT '邮箱',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '手机号',
  `real_name` VARCHAR(50) NOT NULL COMMENT '真实姓名',
  `role` VARCHAR(20) NOT NULL DEFAULT 'user' COMMENT '角色：admin-管理员，user-普通用户',
  `refresh_token` VARCHAR(255) DEFAULT NULL COMMENT '刷新令牌',
  `refresh_token_expiry` DATETIME DEFAULT NULL COMMENT '刷新令牌过期时间',
  `failed_login_count` INT NOT NULL DEFAULT 0 COMMENT '连续登录失败次数',
  `lockout_until` DATETIME DEFAULT NULL COMMENT '账号锁定截止时间（UTC）',
  `token_version` INT NOT NULL DEFAULT 0 COMMENT '访问令牌版本',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `last_login_time` DATETIME DEFAULT NULL COMMENT '最后登录时间',
  `last_login_ip` VARCHAR(50) DEFAULT NULL COMMENT '最后登录IP',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_username` (`username`),
  INDEX `idx_email` (`email`),
  INDEX `idx_refresh_token` (`refresh_token`),
  INDEX `idx_lockout_until` (`lockout_until`),
  INDEX `idx_role` (`role`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户表（API认证）';

CREATE TABLE `user_refresh_tokens` (
  `id` BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '刷新令牌会话ID',
  `user_id` INT UNSIGNED NOT NULL COMMENT '用户ID',
  `token_hash` CHAR(64) NOT NULL COMMENT '刷新令牌SHA-256摘要',
  `expires_at` DATETIME NOT NULL COMMENT '过期时间（UTC）',
  `created_at` DATETIME NOT NULL COMMENT '创建时间（UTC）',
  `last_used_at` DATETIME DEFAULT NULL COMMENT '最后使用时间（UTC）',
  `revoked_at` DATETIME DEFAULT NULL COMMENT '撤销时间（UTC）',
  UNIQUE KEY `uq_user_refresh_tokens_hash` (`token_hash`),
  KEY `idx_user_refresh_tokens_user_revoked` (`user_id`, `revoked_at`),
  KEY `idx_user_refresh_tokens_expires_at` (`expires_at`),
  CONSTRAINT `fk_user_refresh_tokens_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户刷新令牌会话（仅存摘要）';

-- ============================================
-- 2. 公告信息模块（核心）
-- ============================================

-- 2.1 统一公告表
CREATE TABLE `announcements` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '公告ID',
  `title` VARCHAR(255) NOT NULL COMMENT '公告标题',
  `business_type` VARCHAR(50) NOT NULL COMMENT '业务类型：GOV_PROCUREMENT-政府采购, CONSTRUCTION-建设工程',
  `notice_type` VARCHAR(50) NOT NULL COMMENT '公告类型：bidding-招标/采购公告, correction-更正公告, result-结果公告',
  `procurement_type` VARCHAR(50) DEFAULT NULL COMMENT '采购类型（仅政府采购）：goods-货物, service-服务, project-工程',
  `bidder` VARCHAR(255) DEFAULT NULL COMMENT '招标人',
  `winner` VARCHAR(255) DEFAULT NULL COMMENT '中标人',
  `budget_amount` DECIMAL(15,2) DEFAULT NULL COMMENT '预算金额（单位：万元）',
  `deadline` DATETIME DEFAULT NULL COMMENT '截止时间',
  `province` VARCHAR(50) DEFAULT NULL COMMENT '省级行政区划编码',
  `city` VARCHAR(50) DEFAULT NULL COMMENT '市级行政区划编码',
  `district` VARCHAR(50) DEFAULT NULL COMMENT '区县级行政区划编码',
  `project_region` VARCHAR(200) DEFAULT NULL COMMENT '项目区域（完整地址，用于显示和搜索）',
  `content` LONGTEXT NOT NULL COMMENT '公告内容（富文本）',
  `publisher` VARCHAR(50) DEFAULT NULL COMMENT '发布人',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `view_count` INT UNSIGNED DEFAULT 0 COMMENT '浏览次数',
  `attachment_ids` VARCHAR(500) DEFAULT NULL COMMENT '附件ID列表（JSON数组格式，如：[1,2,3]）',
  `is_top` TINYINT DEFAULT 0 COMMENT '是否置顶：0-否，1-是',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_business_type` (`business_type`),
  INDEX `idx_notice_type` (`notice_type`),
  INDEX `idx_province` (`province`),
  INDEX `idx_city` (`city`),
  INDEX `idx_district` (`district`),
  INDEX `idx_project_region` (`project_region`),
  INDEX `idx_publish_time` (`publish_time`),
  INDEX `idx_deadline` (`deadline`),
  INDEX `idx_budget_amount` (`budget_amount`),
  INDEX `idx_is_top` (`is_top`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`),
  FULLTEXT INDEX `ft_title_content` (`title`, `content`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='公告表';

-- ============================================
-- 3. 信息发布模块
-- ============================================

-- 3.1 统一信息发布表（新闻中心、政策法规）
CREATE TABLE `info_publications` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '信息ID',
  `type` VARCHAR(50) NOT NULL COMMENT '信息类型：COMPANY_NEWS-新闻中心, POLICY_REGULATION-政策法规',
  `category` VARCHAR(100) DEFAULT NULL COMMENT '二级分类：公司新闻/行业动态/通知公告（新闻）；法律法规/行政法规/地方政策（政策）',
  `title` VARCHAR(255) NOT NULL COMMENT '标题',
  `summary` VARCHAR(500) DEFAULT NULL COMMENT '摘要',
  `content` LONGTEXT NOT NULL COMMENT '内容（富文本）',
  `cover_image_id` INT UNSIGNED DEFAULT NULL COMMENT '封面图片ID（关联attachments表）',
  `author` VARCHAR(100) DEFAULT NULL COMMENT '作者',
  `publisher` VARCHAR(50) DEFAULT NULL COMMENT '发布人',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `view_count` INT UNSIGNED DEFAULT 0 COMMENT '浏览次数',
  `attachment_ids` VARCHAR(500) DEFAULT NULL COMMENT '附件ID列表（JSON数组格式，如：[1,2,3]）',
  `is_top` TINYINT DEFAULT 0 COMMENT '是否置顶：0-否，1-是',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_type` (`type`),
  INDEX `idx_category` (`category`),
  INDEX `idx_author` (`author`),
  INDEX `idx_publish_time` (`publish_time`),
  INDEX `idx_is_top` (`is_top`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`),
  FULLTEXT INDEX `ft_title_content` (`title`, `content`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='信息发布表';

-- ============================================
-- 4. 系统配置模块
-- ============================================

-- 4.1 企业简介表
CREATE TABLE `company_profile` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '简介ID',
  `title` VARCHAR(255) NOT NULL COMMENT '简介标题',
  `content` LONGTEXT NOT NULL COMMENT '简介内容（富文本）',
  `highlights` TEXT DEFAULT NULL COMMENT '企业特色标签（JSON数组格式，如：["专业资质齐全","经验丰富团队"]）',
  `image_ids` VARCHAR(500) DEFAULT NULL COMMENT '配图ID列表（JSON数组格式，如：[1,2,3]）',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='企业简介表';

-- 4.2 业务范围表
CREATE TABLE `business_scope` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '业务ID',
  `name` VARCHAR(255) NOT NULL COMMENT '业务名称',
  `description` TEXT DEFAULT NULL COMMENT '业务描述',
  `content` LONGTEXT DEFAULT NULL COMMENT '详细内容（富文本）',
  `features` TEXT DEFAULT NULL COMMENT '业务特点（JSON数组格式，如：["采购需求编制","招标文件制作"]）',
  `image_id` INT UNSIGNED DEFAULT NULL COMMENT '业务图片ID（关联attachments表）',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='业务范围表';

-- 4.3 企业资质表
CREATE TABLE `company_qualifications` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '资质ID',
  `name` VARCHAR(255) NOT NULL COMMENT '资质名称',
  `description` TEXT DEFAULT NULL COMMENT '资质描述',
  `image_id` INT UNSIGNED DEFAULT NULL COMMENT '资质证书图片ID（关联attachments表）',
  `certificate_no` VARCHAR(100) DEFAULT NULL COMMENT '证书编号',
  `issuing_authority` VARCHAR(200) DEFAULT NULL COMMENT '颁发机构',
  `issue_date` DATE DEFAULT NULL COMMENT '颁发日期',
  `expiry_date` DATE DEFAULT NULL COMMENT '有效期至',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_issue_date` (`issue_date`),
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='企业资质表';

-- 4.4 重要业绩表
CREATE TABLE `major_achievements` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '业绩ID',
  `project_name` VARCHAR(255) NOT NULL COMMENT '项目名称',
  `project_type` VARCHAR(50) NOT NULL COMMENT '项目类型：工程、服务、货物',
  `project_amount` DECIMAL(15,2) DEFAULT NULL COMMENT '项目金额（万元）',
  `client_name` VARCHAR(255) DEFAULT NULL COMMENT '客户名称',
  `description` TEXT DEFAULT NULL COMMENT '项目描述',
  `completion_date` DATE DEFAULT NULL COMMENT '完成日期',
  `image_ids` VARCHAR(500) DEFAULT NULL COMMENT '项目图片ID列表（JSON数组格式，如：[1,2,3]）',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_project_type` (`project_type`),
  INDEX `idx_completion_date` (`completion_date`),
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='重要业绩表（企业荣誉展示）';

-- 4.5 企业荣誉表
CREATE TABLE `company_honors` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '荣誉ID',
  `name` VARCHAR(255) NOT NULL COMMENT '荣誉名称',
  `description` TEXT DEFAULT NULL COMMENT '荣誉描述',
  `image_id` INT UNSIGNED DEFAULT NULL COMMENT '荣誉证书图片ID（关联attachments表）',
  `award_organization` VARCHAR(255) DEFAULT NULL COMMENT '颁发机构',
  `award_date` DATE DEFAULT NULL COMMENT '获奖日期',
  `certificate_no` VARCHAR(100) DEFAULT NULL COMMENT '证书编号',
  `honor_level` VARCHAR(50) DEFAULT NULL COMMENT '荣誉级别：国家级、省级、市级、行业级等',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_award_date` (`award_date`),
  INDEX `idx_honor_level` (`honor_level`),
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='企业荣誉表（荣誉证书展示）';

-- 4.6 轮播图表
CREATE TABLE `carousel_banners` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '轮播图ID',
  `title` VARCHAR(255) NOT NULL COMMENT '轮播图标题',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '轮播图描述',
  `image_id` INT UNSIGNED NOT NULL COMMENT '轮播图图片ID（关联attachments表）',
  `link_url` VARCHAR(500) DEFAULT NULL COMMENT '跳转链接',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='轮播图表';

-- 4.7 友情链接表
CREATE TABLE `friendly_links` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '链接ID',
  `name` VARCHAR(255) NOT NULL COMMENT '链接名称',
  `url` VARCHAR(500) NOT NULL COMMENT '链接地址',
  `logo_id` INT UNSIGNED DEFAULT NULL COMMENT 'Logo图片ID（关联attachments表）',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '链接描述',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
  `status` TINYINT DEFAULT 1 COMMENT '状态：0-禁用，1-启用',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_status` (`status`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='友情链接表';

-- ============================================
-- 5. 统计模块
-- ============================================

-- 5.1 访问统计表
CREATE TABLE `visit_statistics` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '统计ID',
  `visit_date` DATE NOT NULL COMMENT '访问日期',
  `page_url` VARCHAR(500) DEFAULT NULL COMMENT '页面URL',
  `page_title` VARCHAR(255) DEFAULT NULL COMMENT '页面标题',
  `visitor_ip` VARCHAR(50) DEFAULT NULL COMMENT '访问者IP',
  `user_agent` VARCHAR(500) DEFAULT NULL COMMENT '用户代理',
  `referer` VARCHAR(500) DEFAULT NULL COMMENT '来源页面',
  `visit_count` INT UNSIGNED DEFAULT 1 COMMENT '访问次数',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  INDEX `idx_visit_date` (`visit_date`),
  INDEX `idx_page_url` (`page_url`(255)),
  INDEX `idx_visitor_ip` (`visitor_ip`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='访问统计表';


-- ============================================
-- 6. 项目区域字典表（省市区三级结构）
-- ============================================

CREATE TABLE `region_dictionary` (
  `id` INT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '区域ID',
  `region_code` VARCHAR(50) NOT NULL COMMENT '区域代码（如：410000、410100、410101）',
  `region_name` VARCHAR(50) NOT NULL COMMENT '区域名称',
  `region_level` TINYINT NOT NULL COMMENT '区域级别：1-省份，2-城市，3-区县',
  `parent_code` VARCHAR(50) DEFAULT NULL COMMENT '父级区域代码',
  `sort_order` INT DEFAULT 0 COMMENT '排序顺序',
  `is_deleted` TINYINT DEFAULT 0 COMMENT '软删除：0-未删除，1-已删除',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  UNIQUE INDEX `idx_region_code` (`region_code`),
  INDEX `idx_region_level` (`region_level`),
  INDEX `idx_parent_code` (`parent_code`),
  INDEX `idx_sort_order` (`sort_order`),
  INDEX `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目区域字典表（省市区三级）';



-- ============================================
-- 7. 系统日志表
-- ============================================

CREATE TABLE `system_logs` (
  `id` BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '日志ID',
  `user_id` INT UNSIGNED DEFAULT NULL COMMENT '操作用户ID',
  `username` VARCHAR(50) DEFAULT NULL COMMENT '操作用户名',
  `action` VARCHAR(100) NOT NULL COMMENT '操作动作',
  `module` VARCHAR(50) DEFAULT NULL COMMENT '操作模块',
  `description` TEXT DEFAULT NULL COMMENT '操作描述',
  `ip_address` VARCHAR(50) DEFAULT NULL COMMENT 'IP地址',
  `user_agent` VARCHAR(500) DEFAULT NULL COMMENT '用户代理',
  `request_data` TEXT DEFAULT NULL COMMENT '请求数据（JSON）',
  `response_data` TEXT DEFAULT NULL COMMENT '响应数据（JSON）',
  `status` VARCHAR(20) DEFAULT 'success' COMMENT '操作状态：success、error',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  INDEX `idx_user_id` (`user_id`),
  INDEX `idx_action` (`action`),
  INDEX `idx_module` (`module`),
  INDEX `idx_status` (`status`),
  INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统操作日志表';

-- ============================================
-- 创建视图：首页统计数据视图（基于统一公告表）
-- ============================================

CREATE OR REPLACE VIEW `v_homepage_statistics` AS
SELECT
  (SELECT COUNT(*) FROM `announcements` WHERE `is_deleted` = 0 AND `status` = 1) AS total_projects,
  (SELECT COUNT(DISTINCT CONCAT_WS('-', `province`, `city`, `district`)) FROM `announcements` WHERE `is_deleted` = 0 AND `status` = 1) AS total_regions,
  (SELECT COUNT(*) FROM `announcements` WHERE `business_type` = 'GOV_PROCUREMENT' AND `is_deleted` = 0 AND `status` = 1) AS gov_procurement_count,
  (SELECT COUNT(*) FROM `announcements` WHERE `business_type` = 'CONSTRUCTION' AND `is_deleted` = 0 AND `status` = 1) AS construction_count;

-- ============================================
-- 数据库结构创建完成
-- ============================================
