BEGIN;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM public.collect_requests LIMIT 1)
       OR EXISTS (SELECT 1 FROM public.pickup_services LIMIT 1)
       OR EXISTS (SELECT 1 FROM public.request_history LIMIT 1) THEN
        RAISE EXCEPTION
            'Refusing to replace status catalogs: collect_requests, pickup_services, or request_history contains data.';
    END IF;
END;
$$;

INSERT INTO public.user_roles (role_id, code, name) VALUES
    (1, 'ADMIN', 'Admin'),
    (2, 'GENERATOR', 'Generator'),
    (3, 'ORGANIZATION', 'Organization'),
    (4, 'GATHERER', 'Gatherer')
ON CONFLICT (role_id) DO UPDATE SET code = EXCLUDED.code, name = EXCLUDED.name;

INSERT INTO public.organization_types (organization_type_id, code, name) VALUES
    (1, 'EDUCATIONAL_INSTITUTION', 'Educational Institution'),
    (2, 'COMPANY', 'Company'),
    (3, 'RESTAURANT', 'Restaurant'),
    (4, 'FOOD_INDUSTRY', 'Food Industry'),
    (5, 'OTHER', 'Other')
ON CONFLICT (organization_type_id) DO UPDATE SET code = EXCLUDED.code, name = EXCLUDED.name;

INSERT INTO public.subscription_plans (subscription_plan_id, code, name) VALUES
    (1, 'BASIC', 'Basic'),
    (2, 'PLAN_49', 'Plan 49'),
    (3, 'PLAN_99', 'Plan 99'),
    (4, 'PLAN_179', 'Plan 179')
ON CONFLICT (subscription_plan_id) DO UPDATE SET code = EXCLUDED.code, name = EXCLUDED.name;

INSERT INTO public.gatherer_types (gatherer_type_id, code, name) VALUES
    (1, 'INDEPENDENT', 'Independent'),
    (2, 'COMPANY', 'Company'),
    (3, 'RECYCLER', 'Recycler')
ON CONFLICT (gatherer_type_id) DO UPDATE SET code = EXCLUDED.code, name = EXCLUDED.name;

INSERT INTO public.verification_statuses (verification_status_id, code, name) VALUES
    (1, 'PENDING', 'Pending'),
    (2, 'VERIFIED', 'Verified'),
    (3, 'REJECTED', 'Rejected')
ON CONFLICT (verification_status_id) DO UPDATE SET code = EXCLUDED.code, name = EXCLUDED.name;

INSERT INTO public.residue_categories (residue_category_id, code, name) VALUES
    (1, 'RECYCLABLE', 'Recyclable'),
    (2, 'ORGANIC', 'Organic'),
    (3, 'GENERAL', 'General')
ON CONFLICT (residue_category_id) DO UPDATE SET code = EXCLUDED.code, name = EXCLUDED.name;

INSERT INTO public.measurement_units (measurement_unit_id, code, name) VALUES
    (1, 'KG', 'Kg'),
    (2, 'UNIT', 'Unit'),
    (3, 'LITERS', 'Liters')
ON CONFLICT (measurement_unit_id) DO UPDATE SET code = EXCLUDED.code, name = EXCLUDED.name;

DELETE FROM public.collect_request_statuses;
INSERT INTO public.collect_request_statuses
    (collect_request_status_id, code, name)
VALUES
    (1, 'PENDING', 'Pending'),
    (2, 'ASSIGNED', 'Assigned'),
    (3, 'IN_PROGRESS', 'In Progress'),
    (4, 'COMPLETED', 'Completed'),
    (5, 'CANCELLED', 'Cancelled');

DELETE FROM public.pickup_service_statuses;
INSERT INTO public.pickup_service_statuses
    (pickup_service_status_id, code, name)
VALUES
    (1, 'PENDING', 'Pending'),
    (2, 'ASSIGNED', 'Assigned'),
    (3, 'IN_PROGRESS', 'In Progress'),
    (4, 'COMPLETED', 'Completed'),
    (5, 'CANCELLED', 'Cancelled');

CREATE OR REPLACE VIEW public.vw_gatherer_grades AS
SELECT
    g.gatherer_id,
    u.first_name,
    u.last_name,
    COALESCE(ROUND(AVG(gr.score)::numeric, 2), 0::numeric) AS average_score,
    COUNT(gr.grade_id)::bigint AS total_grades
FROM public.gatherers AS g
JOIN public.users AS u ON u.user_id = g.user_id
LEFT JOIN public.gatherer_grades AS gg ON gg.gatherer_id = g.gatherer_id
LEFT JOIN public.grades AS gr ON gr.grade_id = gg.grade_id
GROUP BY g.gatherer_id, u.first_name, u.last_name;

CREATE OR REPLACE VIEW public.vw_general_indicators AS
SELECT
    (SELECT COUNT(*) FROM public.users WHERE is_active)::bigint AS total_users,
    (SELECT COUNT(*) FROM public.organizations WHERE is_active)::bigint AS total_organizations,
    (SELECT COUNT(*) FROM public.gatherers WHERE is_active AND verification_status_id = 2)::bigint AS verified_gatherers,
    (SELECT COUNT(*) FROM public.collect_requests)::bigint AS total_requests,
    (SELECT COUNT(*) FROM public.collect_requests WHERE status_id = 4)::bigint AS completed_requests,
    (SELECT COUNT(*) FROM public.pickup_services WHERE status_id = 4)::bigint AS completed_services,
    COALESCE(
        (SELECT SUM(quantity) FROM public.recovered_materials WHERE unit_id = 1),
        0::numeric
    ) AS recovered_kilograms;

CREATE OR REPLACE VIEW public.vw_residues_by_type AS
SELECT
    rt.residue_type_id,
    rt.name::varchar(100) AS residue_type,
    rc.code::varchar(40) AS category,
    mu.code::varchar(40) AS unit,
    COALESCE(SUM(rm.quantity), 0::numeric) AS recovered_quantity
FROM public.residue_types AS rt
JOIN public.residue_categories AS rc
    ON rc.residue_category_id = rt.residue_category_id
LEFT JOIN public.recovered_materials AS rm
    ON rm.residue_type_id = rt.residue_type_id
LEFT JOIN public.measurement_units AS mu
    ON mu.measurement_unit_id = rm.unit_id
GROUP BY rt.residue_type_id, rt.name, rc.code, mu.code;

COMMIT;
