ALTER TABLE users
    ADD COLUMN IF NOT EXISTS failed_login_count INT NOT NULL DEFAULT 0 COMMENT '连续登录失败次数' AFTER refresh_token_expiry,
    ADD COLUMN IF NOT EXISTS lockout_until DATETIME NULL COMMENT '账号锁定截止时间（UTC）' AFTER failed_login_count,
    ADD COLUMN IF NOT EXISTS token_version INT NOT NULL DEFAULT 0 COMMENT '访问令牌版本' AFTER lockout_until;

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
