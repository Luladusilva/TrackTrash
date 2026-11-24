BEGIN;

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
    (CASE rt.residue_category_id
        WHEN 1 THEN 'RECYCLABLE'
        WHEN 2 THEN 'ORGANIC'
        WHEN 3 THEN 'GENERAL'
    END)::varchar(40) AS category,
    (CASE rm.unit_id
        WHEN 1 THEN 'KG'
        WHEN 2 THEN 'UNIT'
        WHEN 3 THEN 'LITERS'
    END)::varchar(40) AS unit,
    COALESCE(SUM(rm.quantity), 0::numeric) AS recovered_quantity
FROM public.residue_types AS rt
LEFT JOIN public.recovered_materials AS rm
    ON rm.residue_type_id = rt.residue_type_id
GROUP BY rt.residue_type_id, rt.name, rt.residue_category_id, rm.unit_id;

COMMIT;
