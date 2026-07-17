-- 添加认证相关字段到 users 表
-- 每个 ALTER TABLE 单独执行，如果列已存在会报错，但迁移运行器会忽略已应用的迁移

-- 添加 failed_login_count 列
SET @column_exists = (
    SELECT COUNT(*)
    FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name = 'users'
      AND column_name = 'failed_login_count'
);

SET @sql = IF(@column_exists = 0,
    'ALTER TABLE users ADD COLUMN failed_login_count INT NOT NULL DEFAULT 0 COMMENT ''连续登录失败次数'' AFTER refresh_token_expiry',
    'SELECT 1'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 添加 lockout_until 列
SET @column_exists = (
    SELECT COUNT(*)
    FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name = 'users'
      AND column_name = 'lockout_until'
);

SET @sql = IF(@column_exists = 0,
    'ALTER TABLE users ADD COLUMN lockout_until DATETIME NULL COMMENT ''账号锁定截止时间（UTC）'' AFTER failed_login_count',
    'SELECT 1'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 添加 token_version 列
SET @column_exists = (
    SELECT COUNT(*)
    FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name = 'users'
      AND column_name = 'token_version'
);

SET @sql = IF(@column_exists = 0,
    'ALTER TABLE users ADD COLUMN token_version INT NOT NULL DEFAULT 0 COMMENT ''访问令牌版本'' AFTER lockout_until',
    'SELECT 1'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 创建 user_refresh_tokens 表
CREATE TABLE IF NOT EXISTS user_refresh_tokens (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY COMMENT '刷新令牌会话ID',
    user_id INT UNSIGNED NOT NULL COMMENT '用户ID',
    token_hash CHAR(64) NOT NULL COMMENT '刷新令牌SHA-256摘要',
    expires_at DATETIME NOT NULL COMMENT '过期时间（UTC）',
    created_at DATETIME NOT NULL COMMENT '创建时间（UTC）',
    last_used_at DATETIME NULL COMMENT '最后使用时间（UTC）',
    revoked_at DATETIME NULL COMMENT '撤销时间（UTC）',
    UNIQUE KEY uq_user_refresh_tokens_hash (token_hash),
    KEY idx_user_refresh_tokens_user_revoked (user_id, revoked_at),
    KEY idx_user_refresh_tokens_expires_at (expires_at),
    CONSTRAINT fk_user_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户刷新令牌会话（仅存摘要）';
