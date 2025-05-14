-- ====================================================================================
-- ESQUEMA DE BASE DE DATOS 
-- Descripción: Define la estructura de las tablas para almacenar datos del juego.
-- ====================================================================================

PRAGMA foreign_keys = ON;

-- ------------------------------------------------------------------------------------
-- Tabla: Nave
-- Propósito: Almacena la información de la nave principal del jugador.
-- Se asume que solo habrá una nave en el juego para esta estructura inicial.
-- ------------------------------------------------------------------------------------
CREATE TABLE Nave (
    ID_Nave INTEGER PRIMARY KEY, -- Identificador único para la nave. En SQLite, INTEGER PRIMARY KEY es auto-incremental por defecto.
    Estado TEXT NOT NULL CHECK (Estado IN ('funcional', 'averiada')) -- Estado actual de la nave, solo puede ser 'funcional' o 'averiada'. No puede ser nulo.
);

-- ------------------------------------------------------------------------------------
-- Tabla: Nivel
-- Propósito: Define los diferentes niveles de progresión o dificultad en el juego.
-- Podría ser usado para niveles de tripulantes, niveles de misiones, etc.
-- ------------------------------------------------------------------------------------
CREATE TABLE Nivel (
    ID_Nivel INTEGER PRIMARY KEY, -- Identificador único para cada nivel de progresión.
    Nombre TEXT NOT NULL, -- Nombre descriptivo del nivel (ej. "Novato", "Veterano"). No puede ser nulo.
    Descripcion TEXT, -- Descripción opcional más detallada del nivel. Puede ser NULO si no se provee.
    ExperienciaRequerida INTEGER NOT NULL, -- Puntos de experiencia necesarios para alcanzar este nivel. No puede ser nulo.
    Estado TEXT NOT NULL CHECK (Estado IN ('desbloqueado', 'completado', 'bloqueado')) -- Estado del nivel en el contexto del juego (ej. si el jugador lo ha desbloqueado). No puede ser nulo.
);

-- ------------------------------------------------------------------------------------
-- Tabla: Recompensa
-- Propósito: Define los diferentes tipos de recompensas que se pueden obtener en el juego.
-- ------------------------------------------------------------------------------------
CREATE TABLE Recompensa (
    ID_Recompensa INTEGER PRIMARY KEY, -- Identificador único para cada tipo de recompensa.
    Tipo TEXT NOT NULL CHECK (Tipo IN ('suministro', 'experiencia', 'desbloqueo')), -- Categoría de la recompensa. No puede ser nulo.
    Contenido TEXT, -- Contenido específico de la recompensa (ej. un JSON como '{"oxigeno": 30, "comida": 20}' si es de tipo 'suministro', o el nombre de un objeto a desbloquear). Puede ser NULO.
    CantidadExperiencia INTEGER, -- Puntos de experiencia otorgados por esta recompensa, si aplica. Puede ser NULO si la recompensa no es de experiencia.
    ID_EventoDesbloqueado INTEGER -- Opcionalmente, el ID de un Evento que esta recompensa podría desbloquear. Puede ser NULO. Si se quisiera una FK estricta a la tabla Evento,
                                 
);

-- ------------------------------------------------------------------------------------
-- Tabla: Mision
-- Propósito: Almacena la información sobre las misiones disponibles en el juego.
-- ------------------------------------------------------------------------------------
CREATE TABLE Mision (
    ID_Mision INTEGER PRIMARY KEY, -- Identificador único para cada misión.
    Nombre TEXT NOT NULL, -- Nombre de la misión. No puede ser nulo.
    Descripcion TEXT, -- Descripción detallada de la misión. Puede ser NULO.
    Duracion INTEGER NOT NULL, -- Duración estimada o real de la misión en alguna unidad de tiempo (ej. segundos, turnos). No puede ser nulo.
    ID_Recompensa INTEGER NOT NULL, -- Clave foránea que vincula la misión a su recompensa. Una misión DEBE tener una recompensa.
    Estado TEXT NOT NULL CHECK (Estado IN ('pendiente', 'activa', 'completada')), -- Estado actual de la misión. No puede ser nulo.
    FOREIGN KEY (ID_Recompensa) REFERENCES Recompensa(ID_Recompensa) -- Define la relación con la tabla Recompensa.
);

-- ------------------------------------------------------------------------------------
-- Tabla: Evento
-- Propósito: Define eventos aleatorios o programados que pueden ocurrir durante el juego.
-- ------------------------------------------------------------------------------------
CREATE TABLE Evento (
    ID_Evento INTEGER PRIMARY KEY, -- Identificador único para cada evento.
    Tipo TEXT NOT NULL CHECK (Tipo IN ('fallo', 'sabotaje', 'etc.')), -- Categoría del evento. No puede ser nulo. ('etc.' podría expandirse a más tipos específicos).
    Descripcion TEXT NOT NULL, -- Descripción de lo que sucede en el evento. No puede ser nulo.
    TiempoLimite INTEGER, -- Tiempo límite para resolver el evento, si aplica. Puede ser NULO.
    SuministroAfectado TEXT CHECK (SuministroAfectado IN ('oxígeno', 'energía', 'comida', 'chatarra')), -- Tipo de suministro afectado por el evento, si aplica. Puede ser NULO.
    Impacto TEXT, -- Descripción del impacto o consecuencia del evento (ej. 'Disminuye oxígeno en 20%'). Puede ser NULO.
    ID_Mision INTEGER, -- Clave foránea opcional que vincula el evento a una misión específica. Puede ser NULO si el evento es genérico y no parte de una misión.
    FOREIGN KEY (ID_Mision) REFERENCES Mision(ID_Mision) -- Define la relación con la tabla Mision.
);

-- ------------------------------------------------------------------------------------
-- Tabla: SuministroNave
-- Propósito: Almacena los diferentes tipos de suministros y sus cantidades en la nave.
-- ------------------------------------------------------------------------------------
CREATE TABLE SuministroNave (
    ID_SuministroNave INTEGER PRIMARY KEY, -- Identificador único para cada registro de suministro en la nave.
    ID_Nave INTEGER NOT NULL, -- Clave foránea que indica a qué nave pertenece este suministro.
    TipoSuministro TEXT NOT NULL CHECK (TipoSuministro IN ('oxígeno', 'energía', 'comida', 'chatarra')), -- Tipo de suministro. No puede ser nulo.
    Cantidad INTEGER NOT NULL, -- Cantidad actual de este suministro. No puede ser nulo.
    FOREIGN KEY (ID_Nave) REFERENCES Nave(ID_Nave) -- Define la relación con la tabla Nave.
);

-- ------------------------------------------------------------------------------------
-- Tabla: Modulo
-- Propósito: Define los diferentes módulos o secciones de la nave.
-- ------------------------------------------------------------------------------------
CREATE TABLE Modulo (
    ID_Modulo INTEGER PRIMARY KEY, -- Identificador único para cada módulo de la nave.
    Nombre TEXT NOT NULL, -- Nombre del módulo (ej. "Motor Principal", "Laboratorio"). No puede ser nulo.
    Estado TEXT NOT NULL CHECK (Estado IN ('funcional', 'dañado', 'destruido')), -- Estado actual del módulo. No puede ser nulo.
    ID_Nave INTEGER NOT NULL, -- Clave foránea que indica a qué nave pertenece este módulo.
    FOREIGN KEY (ID_Nave) REFERENCES Nave(ID_Nave) -- Define la relación con la tabla Nave.
);

-- ------------------------------------------------------------------------------------
-- Tabla: Tarea
-- Propósito: Define tareas específicas que los tripulantes pueden realizar.
-- Estas tareas pueden estar asociadas a módulos, eventos, misiones, etc.
-- ------------------------------------------------------------------------------------
CREATE TABLE Tarea (
    ID_Tarea INTEGER PRIMARY KEY, -- Identificador único para cada tarea.
    Descripcion TEXT NOT NULL, -- Descripción de la tarea. No puede ser nulo.
    Duracion INTEGER NOT NULL, -- Duración necesaria para completar la tarea. No puede ser nulo.
    Estado TEXT NOT NULL CHECK (Estado IN ('pendiente', 'en curso', 'completada')), -- Estado actual de la tarea. No puede ser nulo.
    ID_Modulo INTEGER NOT NULL,     -- Clave foránea que indica en qué módulo se realiza o a qué módulo afecta la tarea.
    ID_Evento INTEGER,      -- Clave foránea opcional que vincula la tarea a un evento (ej. una tarea para reparar un fallo). Puede ser NULO.
    ID_Mision INTEGER,      -- Clave foránea opcional que vincula la tarea como parte de una misión.
    NivelNecesario INTEGER, -- Clave foránea opcional que indica el nivel mínimo requerido (ej. de un tripulante) para realizar la tarea. Puede ser NULO si no todas las tareas requieren un nivel específico.
    ID_Recompensa INTEGER,  -- Clave foránea opcional que indica la recompensa obtenida al completar esta tarea. Puede ser NULO si la tarea no da una recompensa directa.
    FOREIGN KEY (ID_Modulo) REFERENCES Modulo(ID_Modulo),
    FOREIGN KEY (ID_Evento) REFERENCES Evento(ID_Evento),
    FOREIGN KEY (ID_Mision) REFERENCES Mision(ID_Mision),
    FOREIGN KEY (NivelNecesario) REFERENCES Nivel(ID_Nivel),
    FOREIGN KEY (ID_Recompensa) REFERENCES Recompensa(ID_Recompensa)
);

-- ------------------------------------------------------------------------------------
-- Tabla: Tripulante
-- Propósito: Almacena la información de los miembros de la tripulación.
-- ------------------------------------------------------------------------------------
CREATE TABLE Tripulante (
    ID_Tripulante INTEGER PRIMARY KEY, -- Identificador único para cada tripulante.
    Nombre TEXT NOT NULL, -- Nombre del tripulante. No puede ser nulo.
    Salud INTEGER NOT NULL CHECK (Salud BETWEEN 0 AND 100), -- Nivel de salud del tripulante (0-100). No puede ser nulo.
    Hambre INTEGER NOT NULL CHECK (Hambre BETWEEN 0 AND 100), -- Nivel de hambre del tripulante (0-100). No puede ser nulo.
    ID_TareaActual INTEGER, -- Clave foránea opcional que indica la tarea que el tripulante está realizando actualmente. Puede ser NULO si el tripulante no está asignado a ninguna tarea.
    ExperienciaActual INTEGER NOT NULL DEFAULT 0, -- Puntos de experiencia acumulados por el tripulante. Por defecto es 0. No puede ser nulo.
    FOREIGN KEY (ID_TareaActual) REFERENCES Tarea(ID_Tarea) -- Define la relación con la tabla Tarea.
);

-- ------------------------------------------------------------------------------------
-- Tabla: Tripulante_Mision (Tabla de Unión o Intermedia)
-- Propósito: Implementa la relación Muchos-a-Muchos entre Tripulantes y Misiones.
-- Un tripulante puede participar en varias misiones, y una misión puede tener varios tripulantes.
-- Esta tabla es crucial para cumplir el requisito de tener al menos una relación M:N.
-- ------------------------------------------------------------------------------------
CREATE TABLE Tripulante_Mision (
    ID_Tripulante_Mision INTEGER PRIMARY KEY, -- Identificador único para cada asignación de un tripulante a una misión.
    ID_Tripulante INTEGER NOT NULL, -- Clave foránea que referencia al tripulante.
    ID_Mision INTEGER NOT NULL,     -- Clave foránea que referencia a la misión.
    FOREIGN KEY (ID_Tripulante) REFERENCES Tripulante(ID_Tripulante), -- Define la relación con la tabla Tripulante.
    FOREIGN KEY (ID_Mision) REFERENCES Mision(ID_Mision), -- Define la relación con la tabla Mision.
    UNIQUE (ID_Tripulante, ID_Mision) -- Restricción de unicidad: asegura que la misma combinación de tripulante y misión no se inserte más de una vez.
);

-- ====================================================================================
-- Fin 
-- ====================================================================================
