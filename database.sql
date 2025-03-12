-- Elimina la base de datos si ya existe
DROP DATABASE IF EXISTS bd;

-- Crea la base de datos
CREATE DATABASE bd;

-- Usa la base de datos recién creada
USE bd;

-- Crea la tabla de jugadores (players)
CREATE TABLE players (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    is_adult BOOLEAN NOT NULL
) ENGINE=InnoDB;

-- Crea la tabla de partidos (matches)
CREATE TABLE matches (
    match_id INT AUTO_INCREMENT PRIMARY KEY,
    match_date DATETIME NOT NULL,
    duration INT NOT NULL,
    winner_id INT,
    FOREIGN KEY (winner_id) REFERENCES players(player_id)
) ENGINE=InnoDB;

-- Crea la tabla de relación entre jugadores y partidos (match_players)
CREATE TABLE match_players (
    match_id INT,
    player_id INT,
    result VARCHAR(50),
    PRIMARY KEY (match_id, player_id),
    FOREIGN KEY (match_id) REFERENCES matches(match_id),
    FOREIGN KEY (player_id) REFERENCES players(player_id)
) ENGINE=InnoDB;

-- Crea la tabla de logs de partidos (match_logs)
CREATE TABLE match_logs (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    match_id INT,
    player_id INT,
    action VARCHAR(50),
    FOREIGN KEY (match_id) REFERENCES matches(match_id),
    FOREIGN KEY (player_id) REFERENCES players(player_id)
) ENGINE=InnoDB;

-- Inserta datos de ejemplo en la tabla de jugadores (players)
INSERT INTO players (username, is_adult) VALUES
('player1', 1),
('player2', 0),
('player3', 1);

-- Inserta datos de ejemplo en la tabla de partidos (matches)
INSERT INTO matches (match_date, duration, winner_id) VALUES
('2023-10-01 15:00:00', 30, 1),
('2023-10-02 16:00:00', 25, 2);

-- Inserta datos de ejemplo en la tabla de relación entre jugadores y partidos (match_players)
INSERT INTO match_players (match_id, player_id, result) VALUES
(1, 1, 'win'),
(1, 2, 'lose'),
(2, 2, 'win'),
(2, 3, 'lose');

-- Inserta datos de ejemplo en la tabla de logs de partidos (match_logs)
INSERT INTO match_logs (match_id, player_id, action) VALUES
(1, 1, 'dribble'),
(1, 1, 'shoot'),
(1, 2, 'dribble'),
(2, 2, 'dribble'),
(2, 2, 'score'),
(2, 3, 'dribble');
