INSERT INTO residue_types (
    name,
    residue_category_id,
    description,
    is_recyclable,
    classification_instructions,
    is_active
)
VALUES
    ('Papel', 1, 'Papel limpio y seco para reciclaje.', TRUE, 'Separar de residuos orgánicos y mantener seco.', TRUE),
    ('Cartón', 1, 'Cartón limpio y seco.', TRUE, 'Aplanar las cajas y retirar restos de comida.', TRUE),
    ('Plástico PET', 1, 'Envases de tereftalato de polietileno.', TRUE, 'Vaciar, enjuagar y aplastar los envases.', TRUE),
    ('Plástico HDPE', 1, 'Envases de polietileno de alta densidad.', TRUE, 'Vaciar y enjuagar los envases.', TRUE),
    ('Vidrio', 1, 'Envases y frascos de vidrio.', TRUE, 'Separar vidrio roto y protegerlo para su manipulación.', TRUE),
    ('Aluminio', 1, 'Latas y piezas de aluminio.', TRUE, 'Vaciar y enjuagar los envases.', TRUE),
    ('Metal ferroso', 1, 'Piezas y envases de hierro o acero.', TRUE, 'Retirar líquidos y elementos peligrosos.', TRUE),
    ('Residuos orgánicos de cocina', 2, 'Restos de alimentos de origen vegetal o animal.', TRUE, 'No mezclar con plásticos, vidrio ni metales.', TRUE),
    ('Restos de jardín', 2, 'Hojas, ramas pequeñas y césped.', TRUE, 'Mantenerlos separados de residuos inorgánicos.', TRUE),
    ('Residuo no aprovechable', 3, 'Residuo que no puede recuperarse mediante reciclaje o compostaje.', FALSE, 'No usar para solicitudes de recolección selectiva.', TRUE)
ON CONFLICT (name) DO NOTHING;
