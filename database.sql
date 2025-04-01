-- Elimina la base de datos si ya existe
DROP DATABASE IF EXISTS bd;

-- Crea la base de datos
CREATE DATABASE bd;

-- Usa la base de datos recién creada
USE bd;

-- Crea la tabla de jugadores (players)
CREATE TABLE players (
    username VARCHAR(50) NOT NULL PRIMARY KEY,
    password VARCHAR(50) NOT NULL
) ENGINE=InnoDB;

-- Crea la tabla de partidos (matches)
-- Corrección en la definición de tablas (cambiar [ ] por ( ))
CREATE TABLE matches (
    match_id INT AUTO_INCREMENT PRIMARY KEY,
    match_date DATETIME NOT NULL,
    duration INT NOT NULL,
    winner_name VARCHAR(50),
    FOREIGN KEY (winner_name) REFERENCES players(username)
) ENGINE=InnoDB;

CREATE TABLE match_players (
    match_id INT,
    player_name VARCHAR(50),
    result VARCHAR(50),
    PRIMARY KEY (match_id, player_name),
    FOREIGN KEY (match_id) REFERENCES matches(match_id),
    FOREIGN KEY (player_name) REFERENCES players(username)
) ENGINE=InnoDB;

CREATE TABLE match_logs (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    match_id INT,
    player_name VARCHAR(50),
    action VARCHAR(50),
    FOREIGN KEY (match_id) REFERENCES matches(match_id),
    FOREIGN KEY (player_name) REFERENCES players(username)
) ENGINE=InnoDB;

-- Corrección en los INSERT (eliminar punto y coma después del primer registro)
INSERT INTO players (username,password) VALUES
('player1', 'patata'),  -- Cambiado ; por ,
('player2', 'patata'),
('player3', 'patata');

-- Luego los partidos (solo dependen de players para winner_name)
INSERT INTO matches (match_date, duration, winner_name) VALUES
('2023-10-01 15:00:00', 30, 'player1'),  -- Este será match_id = 1
('2023-10-02 16:00:00', 25, 'player2');  -- Este será match_id = 2

-- Después las relaciones jugador-partido
INSERT INTO match_players (match_id, player_name, result) VALUES
(1, 'player1', 'win'),
(1, 'player2', 'lose'),
(2, 'player2', 'win'),
(2, 'player3', 'lose');

-- Corrección en el INSERT de match_logs (cambiar player_id por player_name)
INSERT INTO match_logs (match_id, player_name, action) VALUES
(1, 'player2', 'dribble'),
(1, 'player2', 'shoot'),
(1, 'player2', 'dribble'),
(2, 'player2', 'dribble'),
(2, 'player2', 'score'),
(2, 'player2', 'dribble');
