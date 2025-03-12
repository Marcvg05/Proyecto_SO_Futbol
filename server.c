#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql/mysql.h>

#define DB_HOST "localhost"
#define DB_USER "root"
#define DB_PASS "mysql"
#define DB_NAME "bd"

MYSQL *conn;



void connect_db() {
	conn = mysql_init(NULL);
	if (conn == NULL) {
		fprintf(stderr, "mysql_init() failed\n");
		exit(1);
	}
	if (mysql_real_connect(conn, DB_HOST, DB_USER, DB_PASS, DB_NAME, 0, NULL, 0) == NULL) {
		fprintf(stderr, "mysql_real_connect() failed: %s\n", mysql_error(conn));
		mysql_close(conn);
		exit(1);
	}
}

int get_player_id(const char *username) {
	char query[256];
	snprintf(query, sizeof(query), "SELECT player_id FROM players WHERE username='%s';", username);
	
	if (mysql_query(conn, query)) {
		fprintf(stderr, "Error en la consulta: %s\n", mysql_error(conn));
		return -1; // Error al ejecutar la consulta
	}
	
	MYSQL_RES *result = mysql_store_result(conn);
	if (result == NULL) {
		fprintf(stderr, "Error al obtener resultados: %s\n", mysql_error(conn));
		return -1; // Error al obtener el resultado
	}
	
	MYSQL_ROW row = mysql_fetch_row(result);
	int player_id = -1; // Valor por defecto en caso de que no se encuentre el usuario
	
	if (row) {
		player_id = atoi(row[0]); // Convertimos el ID a entero
	}
	
	mysql_free_result(result); // Liberamos la memoria del resultado
	return player_id; // Devolvemos el ID del jugador o -1 si no se encontrÃ³
}


void register_player(const char *username) {
	char query[256];
	snprintf(query, sizeof(query), "INSERT INTO players (username, is_adult) VALUES ('%s', 1);", username);
	if (mysql_query(conn, query)) {
		fprintf(stderr, "Register error: %s\n", mysql_error(conn));
	}
}

void login_player(const char *username) {
	char query[256];
	snprintf(query, sizeof(query), "INSERT INTO match_logs (player_id, action) SELECT player_id, 'login' FROM players WHERE username='%s';", username);
	if (mysql_query(conn, query)) {
		fprintf(stderr, "Login error: %s\n", mysql_error(conn));
	}
}

void query_players(char *result_buffer, size_t buffer_size) {
	if (mysql_query(conn, "SELECT username FROM players;")) {
		fprintf(stderr, "Query error: %s\n", mysql_error(conn));
		snprintf(result_buffer, buffer_size, "Error al consultar jugadores,");
		return;
	}
	MYSQL_RES *result = mysql_store_result(conn);
	if (result == NULL) {
		fprintf(stderr, "Store result error: %s\n", mysql_error(conn));
		snprintf(result_buffer, buffer_size, "Error al almacenar resultados,");
		return;
	}
	MYSQL_ROW row;
	result_buffer[0] = '\0'; // Inicializamos el buffer como cadena vacía
	while ((row = mysql_fetch_row(result))) {
		strncat(result_buffer, row[0], buffer_size - strlen(result_buffer) - 1);
		strncat(result_buffer, ",", buffer_size - strlen(result_buffer) - 1);
	}
	mysql_free_result(result);
}

void get_all_matches(char *result_buffer, size_t buffer_size) {
	char query[] =
		"SELECT m.match_id, m.match_date, m.duration, "
		"IFNULL(p.username, 'Sin ganador') AS winner "
		"FROM matches m "
		"LEFT JOIN players p ON m.winner_id = p.player_id;";
	
	if (mysql_query(conn, query)) {
		fprintf(stderr, "Error en la consulta de partidos: %s\n", mysql_error(conn));
		snprintf(result_buffer, buffer_size, "Error al obtener partidos,");
		return;
	}
	
	MYSQL_RES *result = mysql_store_result(conn);
	if (result == NULL) {
		fprintf(stderr, "Error al almacenar resultados: %s\n", mysql_error(conn));
		snprintf(result_buffer, buffer_size, "Error al almacenar resultados,");
		return;
	}
	MYSQL_ROW row;
	result_buffer[0] = '\0'; // Inicializamos el buffer como cadena vacÃ­a
	
	while ((row = mysql_fetch_row(result))) {
		char match_info[256];
		snprintf(match_info, sizeof(match_info), "ID: %s, Fecha: %s, DuraciÃ³n: %s min, Ganador: %s | ",
				 row[0], row[1], row[2], row[3]);
		
		strncat(result_buffer, match_info, buffer_size - strlen(result_buffer) - 1);
	}
	
	
	mysql_free_result(result);
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char buff[512];
	char buff2[1024]; // Aumentamos el tamaño para almacenar la lista de jugadores
	
	// Inicializar la conexión a la base de datos
	connect_db();
	
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	
	// Fem el bind al port
	memset(&serv_adr, 0, sizeof(serv_adr)); // inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(9050);
	
	if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0)
		printf("Error al bind");
	
	// La cola de peticiones pendientes no podrá ser superior a 4
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	
	int i;
	// Atenderemos solo 5 peticiones
	for (i = 0; i < 7; i++) {
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("He recibido conexión\n");
		
		// Ahora recibimos su nombre, que dejamos en buff
		ret = read(sock_conn, buff, sizeof(buff) - 1);
		if (ret <= 0) { // Cliente cerrÃ³ la conexiÃ³n
			printf("Cliente desconectado inesperadamente.\n");
			close(sock_conn);
			continue; // Volvemos a escuchar
		}
		buff[ret] = '\0';
		

		
		char *p = strtok(buff, "|");
		int codigo = atoi(p);
		p = strtok(NULL, "|"); // No pedir nombre si es la opciÃ³n 3

		char nombre[20] = ""; // Inicializamos nombre como cadena vacía
		
		// Solo copiamos el nombre si es necesario (opciones 1 y 2)
		if (codigo == 1 || codigo == 2 || codigo == 4) {
			if (p != NULL) {
				strncpy(nombre, p, sizeof(nombre) - 1); // Copiamos el nombre si existe
				nombre[sizeof(nombre) - 1] = '\0'; // Aseguramos que esté terminado en NULL
			} else {
				strcpy(buff2, "Error: Nombre no proporcionado,");
				write(sock_conn, buff2, strlen(buff2));
				close(sock_conn);
				continue; // Saltamos al siguiente ciclo del bucle
			}
		}
		
		if (codigo == 1) { // Registrar jugador
			register_player(nombre);
			strcpy(buff2, "Jugador registrado,");
		} else if (codigo == 2) { // Login jugador
			login_player(nombre);
			strcpy(buff2, "Jugador logueado,");
		} else if (codigo == 3) { // Consultar jugadores
			query_players(buff2, sizeof(buff2));
		}else if (codigo == 4) { // Obtener ID del jugador
			if (p != NULL) {
				strncpy(nombre, p, sizeof(nombre) - 1);
				nombre[sizeof(nombre) - 1] = '\0'; // Asegurar que termine en NULL
				
				int player_id = get_player_id(nombre);
				if (player_id != -1) {
					snprintf(buff2, sizeof(buff2), "ID del jugador: %d,", player_id);
				} else {
					strcpy(buff2, "Error: Usuario no encontrado,");
				}
			} else {
				strcpy(buff2, "Error: Nombre no proporcionado,");
			}
			write(sock_conn, buff2, strlen(buff2));
		}else if (codigo == 5) { // Obtener todos los partidos jugados
			get_all_matches(buff2, sizeof(buff2));
			write(sock_conn, buff2, strlen(buff2));
		}else if (codigo == 6) { // Salir
			strcpy(buff2, "Saliendo del servidor,");
			write(sock_conn, buff2, strlen(buff2));
			close(sock_conn); // Cerramos SOLO la conexiÃ³n del cliente
			printf("Cliente desconectado.\n");
			continue; // Volvemos a escuchar nuevas conexiones
		}else {
			strcpy(buff2, "Opción no válida,");
		}
		
		printf("%s\n", buff2);
		// Y lo enviamos
		write(sock_conn, buff2, strlen(buff2));
		
		// Se acabo el servicio para este cliente
		close(sock_conn);
	}
	
	// Cerrar la conexión a la base de datos
	mysql_close(conn);
	return 0;
}
