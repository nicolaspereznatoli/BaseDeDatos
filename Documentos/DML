-- ====================================================================================
-- DATOS INICIALES PARA LA BASE DE DATOS DEL JUEGO
-- ====================================================================================

PRAGMA foreign_keys = ON;

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Nave
-- ------------------------------------------------------------------------------------
INSERT INTO Nave (ID_Nave, Estado) VALUES
(1, 'funcional'); -- La nave principal del jugador, inicialmente funcional.

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Nivel
-- ------------------------------------------------------------------------------------
INSERT INTO Nivel (ID_Nivel, Nombre, Descripcion, ExperienciaRequerida, Estado) VALUES
(1, 'Novato', 'Recién asignado a la misión espacial.', 0, 'desbloqueado'),
(2, 'Experimentado', 'Tiene algo de experiencia con tareas espaciales.', 100, 'bloqueado'), -- Inicialmente bloqueado, se desbloquea con experiencia.
(3, 'Veterano', 'Un tripulante con amplia experiencia y habilidades.', 250, 'bloqueado');

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Recompensa
-- ------------------------------------------------------------------------------------
INSERT INTO Recompensa (ID_Recompensa, Tipo, Contenido, CantidadExperiencia, ID_EventoDesbloqueado) VALUES
(1, 'experiencia', NULL, 50, NULL), -- Recompensa de 50 puntos de experiencia.
(2, 'suministro', '{"recurso": "energia", "cantidad": 100}', NULL, NULL), -- Recompensa de 100 unidades de energía (ejemplo de contenido JSON).
(3, 'desbloqueo', 'Modulo: Escaner Avanzado', NULL, NULL); -- Recompensa que desbloquea algo (el contenido es descriptivo).

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Mision
-- ------------------------------------------------------------------------------------
INSERT INTO Mision (ID_Mision, Nombre, Descripcion, Duracion, ID_Recompensa, Estado) VALUES
(1, 'Exploración Inicial', 'Explorar el sector Gamma-7 en busca de anomalías.', 120, 1, 'pendiente'), -- Misión inicial, da 50 exp (Recompensa ID 1).
(2, 'Recolección de Partes', 'Buscar componentes de repuesto en el cinturón de asteroides.', 180, 2, 'pendiente'); -- Otra misión, da energía (Recompensa ID 2).

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Evento
-- ------------------------------------------------------------------------------------
INSERT INTO Evento (ID_Evento, Tipo, Descripcion, TiempoLimite, SuministroAfectado, Impacto, ID_Mision) VALUES
(1, 'fallo', 'Fallo crítico en el soporte vital del Módulo de Habitaciones.', 60, 'oxígeno', 'Consumo de oxígeno aumentado un 50%.', 1), -- Evento ligado a la Misión 1.
(2, 'sabotaje', 'Un sistema desconocido interfiere con los escáneres de la nave.', NULL, 'energía', 'Drenaje leve de energía de los sistemas.', NULL); -- Evento genérico, no ligado a misión.

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: SuministroNave
-- ------------------------------------------------------------------------------------
INSERT INTO SuministroNave (ID_SuministroNave, ID_Nave, TipoSuministro, Cantidad) VALUES
(1, 1, 'oxígeno', 100),
(2, 1, 'energía', 200),
(3, 1, 'comida', 150),
(4, 1, 'chatarra', 50);

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Modulo
-- ------------------------------------------------------------------------------------
INSERT INTO Modulo (ID_Modulo, Nombre, Estado, ID_Nave) VALUES
(1, 'Puente de Mando', 'funcional', 1),
(2, 'Motor Principal', 'funcional', 1),
(3, 'Laboratorio Científico', 'funcional', 1),
(4, 'Soporte Vital', 'funcional', 1);

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Tarea
-- ------------------------------------------------------------------------------------
INSERT INTO Tarea (ID_Tarea, Descripcion, Duracion, Estado, ID_Modulo, ID_Evento, ID_Mision, NivelNecesario, ID_Recompensa) VALUES
(1, 'Calibrar sensores del Puente de Mando', 30, 'pendiente', 1, NULL, 1, 1, 1), -- Tarea para Misión 1, en Puente, req Nivel 1, da Recompensa 1.
(2, 'Reparar fuga de oxígeno en Soporte Vital', 60, 'pendiente', 4, 1, 1, 2, NULL), -- Tarea para Evento 1 (ligado a Misión 1), en Soporte Vital, req Nivel 2.
(3, 'Analizar datos de escáner', 45, 'pendiente', 3, NULL, NULL, 1, NULL); -- Tarea genérica en Laboratorio.

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Tripulante
-- ------------------------------------------------------------------------------------
INSERT INTO Tripulante (ID_Tripulante, Nombre, Salud, Hambre, ID_TareaActual, ExperienciaActual) VALUES
(1, 'Comandante Eva Rostova', 100, 10, NULL, 50),
(2, 'Ingeniero Jian Li', 95, 25, NULL, 20),
(3, 'Científica Anya Sharma', 90, 15, NULL, 30);

-- ------------------------------------------------------------------------------------
-- Datos para la tabla: Tripulante_Mision (Relación Muchos-a-Muchos)
-- ------------------------------------------------------------------------------------

INSERT INTO Tripulante_Mision (ID_Tripulante, ID_Mision) VALUES
(1, 1),
(2, 1);

INSERT INTO Tripulante_Mision (ID_Tripulante, ID_Mision) VALUES
(3, 2);

INSERT INTO Tripulante_Mision (ID_Tripulante, ID_Mision) VALUES
(1, 2);

-- ====================================================================================
-- Fin
-- ====================================================================================
