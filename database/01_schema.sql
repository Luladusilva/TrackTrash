CREATE TABLE locations (
    location_id bigint GENERATED ALWAYS AS IDENTITY,
    address character varying(250) NOT NULL,
    reference character varying(250),
    district character varying(100) NOT NULL,
    province character varying(100) DEFAULT ('Arequipa'::character varying),
    department character varying(100) DEFAULT ('Arequipa'::character varying),
    latitude numeric(10,7),
    longitude numeric(10,7),
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT locations_pkey PRIMARY KEY (location_id)
);


CREATE TABLE residue_types (
    residue_type_id bigint GENERATED ALWAYS AS IDENTITY,
    name character varying(100) NOT NULL,
    residue_category_id smallint NOT NULL,
    description text,
    is_recyclable boolean NOT NULL,
    classification_instructions text,
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT residue_types_pkey PRIMARY KEY (residue_type_id)
);


CREATE TABLE collection_points (
    collection_point_id bigint GENERATED ALWAYS AS IDENTITY,
    name character varying(150) NOT NULL,
    manager_name character varying(120),
    phone character varying(20),
    location_id bigint NOT NULL,
    opening_hours character varying(150),
    description text,
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT collection_points_pkey PRIMARY KEY (collection_point_id),
    CONSTRAINT collection_points_location_id_fkey FOREIGN KEY (location_id) REFERENCES locations (location_id)
);


CREATE TABLE users (
    user_id bigint GENERATED ALWAYS AS IDENTITY,
    first_name character varying(120) NOT NULL,
    last_name character varying(120) NOT NULL,
    email character varying(150) NOT NULL,
    phone character varying(20),
    password_hash character varying(255) NOT NULL,
    role_id smallint NOT NULL DEFAULT 2,
    location_id bigint,
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    updated_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT users_pkey PRIMARY KEY (user_id),
    CONSTRAINT users_location_id_fkey FOREIGN KEY (location_id) REFERENCES locations (location_id)
);


CREATE TABLE collection_point_residue_types (
    collection_point_id bigint NOT NULL,
    residue_type_id bigint NOT NULL,
    CONSTRAINT collection_point_residue_types_pkey PRIMARY KEY (collection_point_id, residue_type_id),
    CONSTRAINT collection_point_residue_types_collection_point_id_fkey FOREIGN KEY (collection_point_id) REFERENCES collection_points (collection_point_id) ON DELETE CASCADE,
    CONSTRAINT collection_point_residue_types_residue_type_id_fkey FOREIGN KEY (residue_type_id) REFERENCES residue_types (residue_type_id)
);


CREATE TABLE gatherers (
    gatherer_id bigint GENERATED ALWAYS AS IDENTITY,
    user_id bigint NOT NULL,
    gatherer_type_id smallint NOT NULL,
    description text,
    verification_document character varying(50),
    verification_status_id smallint NOT NULL DEFAULT 1,
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT gatherers_pkey PRIMARY KEY (gatherer_id),
    CONSTRAINT gatherers_user_id_fkey FOREIGN KEY (user_id) REFERENCES users (user_id)
);


CREATE TABLE organizations (
    organization_id bigint GENERATED ALWAYS AS IDENTITY,
    user_id bigint NOT NULL,
    business_name character varying(180) NOT NULL,
    tax_id character varying(11),
    organization_type_id smallint NOT NULL,
    location_id bigint,
    subscription_plan_id smallint NOT NULL DEFAULT 1,
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT organizations_pkey PRIMARY KEY (organization_id),
    CONSTRAINT organizations_location_id_fkey FOREIGN KEY (location_id) REFERENCES locations (location_id),
    CONSTRAINT organizations_user_id_fkey FOREIGN KEY (user_id) REFERENCES users (user_id)
);


CREATE TABLE gatherer_residue_types (
    gatherer_id bigint NOT NULL,
    residue_type_id bigint NOT NULL,
    CONSTRAINT gatherer_residue_types_pkey PRIMARY KEY (gatherer_id, residue_type_id),
    CONSTRAINT gatherer_residue_types_gatherer_id_fkey FOREIGN KEY (gatherer_id) REFERENCES gatherers (gatherer_id) ON DELETE CASCADE,
    CONSTRAINT gatherer_residue_types_residue_type_id_fkey FOREIGN KEY (residue_type_id) REFERENCES residue_types (residue_type_id)
);


CREATE TABLE collect_requests (
    collect_request_id bigint GENERATED ALWAYS AS IDENTITY,
    user_id bigint NOT NULL,
    organization_id bigint,
    location_id bigint NOT NULL,
    requested_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    scheduled_date date NOT NULL,
    scheduled_time time without time zone NOT NULL,
    status_id smallint NOT NULL DEFAULT 1,
    observations text,
    updated_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT collect_requests_pkey PRIMARY KEY (collect_request_id),
    CONSTRAINT collect_requests_location_id_fkey FOREIGN KEY (location_id) REFERENCES locations (location_id),
    CONSTRAINT collect_requests_organization_id_fkey FOREIGN KEY (organization_id) REFERENCES organizations (organization_id),
    CONSTRAINT collect_requests_user_id_fkey FOREIGN KEY (user_id) REFERENCES users (user_id)
);


CREATE TABLE pickup_services (
    pickup_service_id bigint GENERATED ALWAYS AS IDENTITY,
    collect_request_id bigint NOT NULL,
    gatherer_id bigint NOT NULL,
    assigned_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    started_at timestamp with time zone,
    completed_at timestamp with time zone,
    status_id smallint NOT NULL DEFAULT 1,
    commission_percentage numeric(5,2) NOT NULL DEFAULT 10.0,
    observations text,
    CONSTRAINT pickup_services_pkey PRIMARY KEY (pickup_service_id),
    CONSTRAINT pickup_services_collect_request_id_fkey FOREIGN KEY (collect_request_id) REFERENCES collect_requests (collect_request_id),
    CONSTRAINT pickup_services_gatherer_id_fkey FOREIGN KEY (gatherer_id) REFERENCES gatherers (gatherer_id)
);


CREATE TABLE request_details (
    request_detail_id bigint GENERATED ALWAYS AS IDENTITY,
    collect_request_id bigint NOT NULL,
    residue_type_id bigint NOT NULL,
    estimated_quantity numeric(12,2) NOT NULL,
    unit_id smallint NOT NULL DEFAULT 1,
    observations text,
    CONSTRAINT request_details_pkey PRIMARY KEY (request_detail_id),
    CONSTRAINT request_details_collect_request_id_fkey FOREIGN KEY (collect_request_id) REFERENCES collect_requests (collect_request_id) ON DELETE CASCADE,
    CONSTRAINT request_details_residue_type_id_fkey FOREIGN KEY (residue_type_id) REFERENCES residue_types (residue_type_id)
);


CREATE TABLE request_history (
    request_history_id bigint GENERATED ALWAYS AS IDENTITY,
    collect_request_id bigint NOT NULL,
    user_id bigint,
    previous_status_id smallint,
    new_status_id smallint NOT NULL,
    observations text,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT request_history_pkey PRIMARY KEY (request_history_id),
    CONSTRAINT request_history_collect_request_id_fkey FOREIGN KEY (collect_request_id) REFERENCES collect_requests (collect_request_id) ON DELETE CASCADE,
    CONSTRAINT request_history_user_id_fkey FOREIGN KEY (user_id) REFERENCES users (user_id)
);


CREATE TABLE grades (
    grade_id bigint GENERATED ALWAYS AS IDENTITY,
    pickup_service_id bigint NOT NULL,
    user_id bigint NOT NULL,
    score smallint NOT NULL,
    comment text,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT grades_pkey PRIMARY KEY (grade_id),
    CONSTRAINT grades_pickup_service_id_fkey FOREIGN KEY (pickup_service_id) REFERENCES pickup_services (pickup_service_id),
    CONSTRAINT grades_user_id_fkey FOREIGN KEY (user_id) REFERENCES users (user_id)
);


CREATE TABLE recovered_materials (
    recovered_material_id bigint GENERATED ALWAYS AS IDENTITY,
    pickup_service_id bigint NOT NULL,
    residue_type_id bigint NOT NULL,
    quantity numeric(12,2) NOT NULL,
    unit_id smallint NOT NULL DEFAULT 1,
    collection_point_id bigint,
    created_at timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    observations text,
    CONSTRAINT recovered_materials_pkey PRIMARY KEY (recovered_material_id),
    CONSTRAINT recovered_materials_collection_point_id_fkey FOREIGN KEY (collection_point_id) REFERENCES collection_points (collection_point_id),
    CONSTRAINT recovered_materials_pickup_service_id_fkey FOREIGN KEY (pickup_service_id) REFERENCES pickup_services (pickup_service_id),
    CONSTRAINT recovered_materials_residue_type_id_fkey FOREIGN KEY (residue_type_id) REFERENCES residue_types (residue_type_id)
);


CREATE TABLE gatherer_grades (
    gatherer_grade_id bigint GENERATED ALWAYS AS IDENTITY,
    gatherer_id bigint NOT NULL,
    grade_id bigint NOT NULL,
    CONSTRAINT gatherer_grades_pkey PRIMARY KEY (gatherer_grade_id),
    CONSTRAINT gatherer_grades_gatherer_id_fkey FOREIGN KEY (gatherer_id) REFERENCES gatherers (gatherer_id) ON DELETE CASCADE,
    CONSTRAINT gatherer_grades_grade_id_fkey FOREIGN KEY (grade_id) REFERENCES grades (grade_id) ON DELETE CASCADE
);


CREATE INDEX idx_requests_date ON collect_requests (scheduled_date);


CREATE INDEX idx_requests_organization ON collect_requests (organization_id);


CREATE INDEX idx_requests_status ON collect_requests (status_id);


CREATE INDEX idx_requests_user ON collect_requests (user_id);


CREATE INDEX "IX_collect_requests_location_id" ON collect_requests (location_id);


CREATE INDEX "IX_collection_point_residue_types_residue_type_id" ON collection_point_residue_types (residue_type_id);


CREATE INDEX "IX_collection_points_location_id" ON collection_points (location_id);


CREATE UNIQUE INDEX gatherer_grades_grade_id_key ON gatherer_grades (grade_id);


CREATE INDEX "IX_gatherer_grades_gatherer_id" ON gatherer_grades (gatherer_id);


CREATE INDEX "IX_gatherer_residue_types_residue_type_id" ON gatherer_residue_types (residue_type_id);


CREATE UNIQUE INDEX gatherers_user_id_key ON gatherers (user_id);


CREATE INDEX idx_gatherers_verification ON gatherers (verification_status_id);


CREATE INDEX "IX_grades_user_id" ON grades (user_id);


CREATE UNIQUE INDEX uq_grade_service_user ON grades (pickup_service_id, user_id);


CREATE INDEX idx_locations_district ON locations (district);


CREATE INDEX idx_organizations_type ON organizations (organization_type_id);


CREATE INDEX "IX_organizations_location_id" ON organizations (location_id);


CREATE UNIQUE INDEX organizations_tax_id_key ON organizations (tax_id);


CREATE UNIQUE INDEX organizations_user_id_key ON organizations (user_id);


CREATE INDEX idx_services_gatherer ON pickup_services (gatherer_id);


CREATE INDEX idx_services_status ON pickup_services (status_id);


CREATE UNIQUE INDEX pickup_services_collect_request_id_key ON pickup_services (collect_request_id);


CREATE INDEX idx_recovered_service ON recovered_materials (pickup_service_id);


CREATE INDEX "IX_recovered_materials_collection_point_id" ON recovered_materials (collection_point_id);


CREATE INDEX "IX_recovered_materials_residue_type_id" ON recovered_materials (residue_type_id);


CREATE INDEX "IX_request_details_residue_type_id" ON request_details (residue_type_id);


CREATE UNIQUE INDEX uq_request_residue ON request_details (collect_request_id, residue_type_id);


CREATE INDEX idx_history_request ON request_history (collect_request_id, created_at);


CREATE INDEX "IX_request_history_user_id" ON request_history (user_id);


CREATE INDEX idx_residue_types_category ON residue_types (residue_category_id);


CREATE UNIQUE INDEX residue_types_name_key ON residue_types (name);


CREATE INDEX idx_users_role ON users (role_id);


CREATE INDEX "IX_users_location_id" ON users (location_id);


CREATE UNIQUE INDEX users_email_key ON users (email);


