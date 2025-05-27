DROP DATABASE IF EXISTS T7_BBDD;
-- Crea la base de datos
CREATE DATABASE T7_BBDD;

-- Usa la base de datos recién creada
USE T7_BBDD;

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
    FOREIGN KEY (winner_name) REFERENCES players(username) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE match_players (
    match_id INT,
    player_name VARCHAR(50),
    result VARCHAR(50),
    PRIMARY KEY (match_id, player_name),
    FOREIGN KEY (match_id) REFERENCES matches(match_id),
    FOREIGN KEY (player_name) REFERENCES players(username) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE match_logs (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    match_id INT,
    player_name VARCHAR(50),
    action VARCHAR(50),
    FOREIGN KEY (match_id) REFERENCES matches(match_id),
    FOREIGN KEY (player_name) REFERENCES players(username) ON DELETE CASCADE
) ENGINE=InnoDB;


SELECT 
    m.match_id,
    m.match_date,
    m.duration,
    m.winner_name AS ganador,
    MAX(CASE WHEN mp.result = 'lose' THEN mp.player_name END) AS perdedor1,
    MIN(CASE WHEN mp.result = 'lose' THEN mp.player_name END) AS perdedor2
FROM matches m
JOIN match_players mp ON m.match_id = mp.match_id
GROUP BY m.match_id
ORDER BY m.match_date DESC;
