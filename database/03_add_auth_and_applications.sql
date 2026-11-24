CREATE TABLE IF NOT EXISTS refresh_tokens (
    refresh_token_id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id bigint NOT NULL REFERENCES users (user_id) ON DELETE CASCADE,
    token_hash character varying(64) NOT NULL UNIQUE,
    family_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    expires_at timestamp with time zone NOT NULL,
    revoked_at timestamp with time zone,
    replaced_by_refresh_token_id bigint REFERENCES refresh_tokens (refresh_token_id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_refresh_tokens_family ON refresh_tokens (family_id);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_expiry ON refresh_tokens (user_id, expires_at);

CREATE TABLE IF NOT EXISTS collect_request_applications (
    collect_request_application_id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    collect_request_id bigint NOT NULL REFERENCES collect_requests (collect_request_id) ON DELETE CASCADE,
    gatherer_id bigint NOT NULL REFERENCES gatherers (gatherer_id) ON DELETE CASCADE,
    status_id smallint NOT NULL DEFAULT 1,
    applied_at timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    responded_at timestamp with time zone,
    CONSTRAINT uq_application_request_gatherer UNIQUE (collect_request_id, gatherer_id)
);

CREATE INDEX IF NOT EXISTS idx_applications_request_status
    ON collect_request_applications (collect_request_id, status_id);
