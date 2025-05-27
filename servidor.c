#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <pthread.h>
#include <fcntl.h>
#include <errno.h>
#include <mysql/mysql.h>
#include <signal.h>
#include <ctype.h>

#define DB_HOST "shiva2.upc.es"
#define DB_USER "root"
#define DB_PASS "mysql"
#define DB_NAME "T7_BBDD"
#define MAX_PLAYERS 100
#define MAX_PLAYERS_PER_GAME 3
#define NOTIFICATION_MSG_SIZE 512

MYSQL *conn;
volatile sig_atomic_t shutdown_requested = 0;

// Estructura para jugadores conectados
typedef struct {
    char nombre[50];
    int socket;
} Conectado;

typedef struct {
    Conectado conectados[MAX_PLAYERS];
    int num;
} ListaConectados;

// Declarar las variables globales para gestionar las partidas
typedef struct {
    char creador[50];
    char jugadores[MAX_PLAYERS_PER_GAME][50];
    int num_jugadores;
} Partida;

Partida partidas[MAX_PLAYERS];
int num_partidas = 0;
ListaConectados miLista = {0};

// Declarar el prototipo de la función BuscarPartidaPorCreador
int BuscarPartidaPorCreador(const char *creador);

void iniciar_partida(const char *creador, ListaConectados *miLista);
// Estructura para el sistema de notificaciones
typedef struct {
    char message[NOTIFICATION_MSG_SIZE];
    int broadcast;
} Notification;

typedef struct {
    int notification_pipe[2];
    pthread_t thread_id;
    ListaConectados *lista;
    pthread_mutex_t *mutex;
} NotificationHandler;

// Estructura para datos del cliente
typedef struct {
    int *socket;
    ListaConectados *lista;
    pthread_mutex_t *mutex;
    NotificationHandler *notif_handler;
    char nombre[50];
} ThreadData;

// Funciones de base de datos
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
void handle_signal(int signal) {
    if (signal == SIGINT || signal == SIGTERM) {
        printf("Señal de terminación recibida. Cerrando servidor...\n");
        shutdown_requested = 1;
    }
}
int register_player(MYSQL *conn, const char *username, const char *password) {
    if (username == NULL || password == NULL || strlen(username) == 0 || strlen(password) == 0) {
        fprintf(stderr, "Error: Usuario o contraseña vacíos\n");
        return -1;
    }

    char query[1024];
    snprintf(query, sizeof(query), 
             "SELECT username FROM players WHERE username = '%s'", username);
    
    if (mysql_query(conn, query)) {
        fprintf(stderr, "Database query error: %s\n", mysql_error(conn));
        return -1;
    }

    MYSQL_RES *result = mysql_store_result(conn);
    if (result == NULL) {
        fprintf(stderr, "Error storing result: %s\n", mysql_error(conn));
        return -1;
    }

    int num_rows = mysql_num_rows(result);
    mysql_free_result(result);

    if (num_rows > 0) {
        return 0; // Usuario ya existe
    }

    snprintf(query, sizeof(query),
             "INSERT INTO players (username, password) VALUES ('%s', '%s')",
             username, password);

    if (mysql_query(conn, query)) {
        fprintf(stderr, "Registration error: %s\n", mysql_error(conn));
        return -1;
    }

    return 1; // Registro exitoso
}

int check_login_credentials(MYSQL *conn, const char *username, const char *password) {
    char query[1024];
    snprintf(query, sizeof(query), 
             "SELECT username FROM players WHERE username = '%s' AND password = '%s'", 
             username, password);
    
    if (mysql_query(conn, query)) {
        fprintf(stderr, "Database query error: %s\n", mysql_error(conn));
        return -1;
    }

    MYSQL_RES *result = mysql_store_result(conn);
    if (result == NULL) {
        fprintf(stderr, "Error storing result: %s\n", mysql_error(conn));
        return -1;
    }

    int num_rows = mysql_num_rows(result);
    mysql_free_result(result);

    return num_rows > 0 ? 1 : 0;
}

// Funciones de gestión de jugadores
int NuevoJugador(ListaConectados *lista, const char *nombre, int socket) {
    if (lista->num == MAX_PLAYERS) {
        return -1;
    }
    
    for (int i = 0; i < lista->num; i++) {
        if (strcmp(lista->conectados[i].nombre, nombre) == 0) {
            return -2;
        }
    }
    
    strncpy(lista->conectados[lista->num].nombre, nombre, 
           sizeof(lista->conectados[lista->num].nombre) - 1);
    lista->conectados[lista->num].nombre[sizeof(lista->conectados[lista->num].nombre) - 1] = '\0';
    lista->conectados[lista->num].socket = socket;
    lista->num++;
    return 0;
}

int DamePosicion(ListaConectados *lista, const char *nombre) {
    for (int i = 0; i < lista->num; i++) {
        if (strcmp(lista->conectados[i].nombre, nombre) == 0) {
            return i;
        }
    }
    return -1;
}

int Eliminar(ListaConectados *lista, const char *nombre) {
    int pos = DamePosicion(lista, nombre);
    if (pos == -1) {
        return -1;
    }
    
    for (int i = pos; i < lista->num - 1; i++) {
        lista->conectados[i] = lista->conectados[i + 1];
    }
    lista->num--;
    return 0;
}

void DameConectados(ListaConectados *lista, char conectados[300]) {
    conectados[0] = '\0';
    
    if (lista == NULL || lista->num <= 0) {
        strcpy(conectados, "0/");
        return;
    }

    snprintf(conectados, 300, "%d", lista->num);
    
    for(int i = 0; i < lista->num; i++) {
        strncat(conectados, "/", 300 - strlen(conectados) - 1);
        strncat(conectados, lista->conectados[i].nombre, 300 - strlen(conectados) - 1);
    }
}

int nombres_iguales(const char *a, const char *b) {
    while (*a && *b) {
        if (tolower((unsigned char)*a) != tolower((unsigned char)*b)) return 0;
        a++; b++;
    }
    return *a == *b;
}

// Funciones de notificaciones
void *notification_thread(void *arg) {
    NotificationHandler *handler = (NotificationHandler *)arg;
    char buffer[sizeof(Notification)];
    ssize_t bytes_read;
    
    printf("Thread de notificaciones iniciado\n");
    
    while (!shutdown_requested) {
        memset(buffer, 0, sizeof(buffer));
        bytes_read = read(handler->notification_pipe[0], buffer, sizeof(buffer));
        
        if (bytes_read > 0) {
            Notification *notif = (Notification *)buffer;
            
            if (notif->broadcast) {
                pthread_mutex_lock(handler->mutex);
                
                char conectados[300];
                DameConectados(handler->lista, conectados);
                char broadcast_msg[350];
                snprintf(broadcast_msg, sizeof(broadcast_msg), "UPDATE|%s", conectados);
                
                for (int i = 0; i < handler->lista->num; i++) {
                    int sock = handler->lista->conectados[i].socket;
                    if (write(sock, broadcast_msg, strlen(broadcast_msg)) <= 0) {
                        // Cliente desconectado, lo manejaremos en el próximo broadcast
                    }
                }
                
                pthread_mutex_unlock(handler->mutex);
            }
            
            printf("Notificación: %s\n", notif->message);
        } else if (bytes_read == 0) {
            break;
        } else {
            if (errno != EAGAIN && errno != EWOULDBLOCK) {
                perror("Error en read del pipe");
                break;
            }
            usleep(200000);
        }
    }
    
    printf("Thread de notificaciones finalizado\n");
    return NULL;
}

void send_notification(NotificationHandler *handler, const char *message, int broadcast) {
    Notification notif;
    strncpy(notif.message, message, sizeof(notif.message) - 1);
    notif.message[sizeof(notif.message) - 1] = '\0';
    notif.broadcast = broadcast;
    
    if (write(handler->notification_pipe[1], &notif, sizeof(notif)) <= 0) {
        perror("Error enviando notificación");
    }
}

// Función para buscar una partida por el nombre del creador
int BuscarPartidaPorCreador(const char *creador) {
    for (int i = 0; i < num_partidas; i++) {
        if (strcmp(partidas[i].creador, creador) == 0) {
            return i;
        }
    }
    return -1;
}

// Función para añadir un jugador a una partida
int AñadirJugadorAPartida(const char *creador, const char *jugador) {
    int index = BuscarPartidaPorCreador(creador);
    if (index == -1) return -1; // Partida no encontrada

    if (partidas[index].num_jugadores >= MAX_PLAYERS_PER_GAME) return -2; // Partida llena

    strncpy(partidas[index].jugadores[partidas[index].num_jugadores], jugador, 50);
    partidas[index].num_jugadores++;
    return 0;
}

typedef struct {
    char jugadores[MAX_PLAYERS_PER_GAME][50];
    int sockets[MAX_PLAYERS_PER_GAME];
    int puntos[MAX_PLAYERS_PER_GAME];
    int ronda_actual;
    pthread_t thread_id;
    int apuesta_pipe[MAX_PLAYERS_PER_GAME][2]; // [0]=lectura, [1]=escritura
    int jugadores_salidos[MAX_PLAYERS_PER_GAME];
    time_t tiempo_inicio;
} PartidaActiva;

PartidaActiva partidas_activas[MAX_PLAYERS];
int num_partidas_activas = 0;
pthread_mutex_t partidas_mutex = PTHREAD_MUTEX_INITIALIZER;

// Función para manejar una partida
void *manejar_partida(void *arg) {
    PartidaActiva *partida = (PartidaActiva *)arg;
    printf("Partida iniciada con jugadores: %s, %s, %s\n",
           partida->jugadores[0], partida->jugadores[1], partida->jugadores[2]);

    for (int ronda = 1; ronda <= 5; ronda++) {
        pthread_mutex_lock(&partidas_mutex);
        partida->ronda_actual = ronda;
        pthread_mutex_unlock(&partidas_mutex);

        // Generar número aleatorio entre 1 y 6
        int numero_dado = (rand() % 6) + 1;
        printf("Ronda %d: Número del dado: %d\n", ronda, numero_dado);

        // Notificar a los jugadores el inicio de la ronda
        for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
            char mensaje[256];
            snprintf(mensaje, sizeof(mensaje), "RONDA|%d", ronda);
            write(partida->sockets[i], mensaje, strlen(mensaje));
        }

        // Esperar las apuestas de los jugadores
        int respuestas_recibidas = 0;
        int apuestas[MAX_PLAYERS_PER_GAME] = {-1, -1, -1};
        int jugadores_respondieron[MAX_PLAYERS_PER_GAME] = {0};

        time_t start_time = time(NULL);
        while (time(NULL) - start_time < 40 && respuestas_recibidas < MAX_PLAYERS_PER_GAME) {
            for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
                if (jugadores_respondieron[i]) continue;
                char buffer[256] = {0};
                int bytes_recibidos = read(partida->apuesta_pipe[i][0], buffer, sizeof(buffer) - 1);
                if (bytes_recibidos > 0) {
                    buffer[bytes_recibidos] = '\0';
                    if (strncmp(buffer, "APUESTA|", 8) == 0) {
                        char *nombre = strtok(buffer + 8, "|");
                        char *num_str = strtok(NULL, "|");
                        int num = num_str ? atoi(num_str) : 0;
                        if (nombre && num > 0 && num <= 6 && nombres_iguales(nombre, partida->jugadores[i]) == 1) {
                            apuestas[i] = num;
                            jugadores_respondieron[i] = 1;
                            respuestas_recibidas++;
                        }
                    }
                }
            }
            if (respuestas_recibidas == MAX_PLAYERS_PER_GAME)
                break;
            usleep(100000);
        }

        // Evaluar las apuestas y sumar puntos
        for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
            if (apuestas[i] == numero_dado) {
                partida->puntos[i]++;
            }
        }

        // Encontrar ganadores de la ronda
        char ganadores_ronda[200] = "";
        int primero = 1;
        for (int j = 0; j < MAX_PLAYERS_PER_GAME; j++) {
            if (apuestas[j] == numero_dado && apuestas[j] != -1) {
                if (!primero) strcat(ganadores_ronda, ", ");
                strcat(ganadores_ronda, partida->jugadores[j]);
                primero = 0;
            }
        }
        if (strlen(ganadores_ronda) == 0) strcpy(ganadores_ronda, "Nadie");

        // Enviar resultado de la ronda
        for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
            char estado[1024];
            snprintf(estado, sizeof(estado),
                "RESULTADO|%d|DADO|%d|JUGADORES|%s|%s|%s|PUNTOS|%d|%d|%d|GANADOR_RONDA|%s",
                ronda, numero_dado,
                partida->jugadores[0], partida->jugadores[1], partida->jugadores[2],
                partida->puntos[0], partida->puntos[1], partida->puntos[2],
                ganadores_ronda);

            // Agrega '\0' al final para que el cliente pueda delimitar el mensaje correctamente
            estado[strlen(estado)] = '\0';
    
            // Enviar incluyendo el terminador nulo
            write(partida->sockets[i], estado, strlen(estado) + 1);

            // Debug: imprimir lo que se envía por consola del servidor
            printf("[SERVER] Enviado a %s: %s\n", partida->jugadores[i], estado);
        }

        // Esperar confirmación de los jugadores para pasar a la siguiente ronda
        int confirmaciones_recibidas = 0;
        int jugadores_confirmaron[MAX_PLAYERS_PER_GAME] = {0};
        while (confirmaciones_recibidas < MAX_PLAYERS_PER_GAME) {
            for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
                if (jugadores_confirmaron[i]) continue;
                char buffer[256] = {0};
                int bytes_recibidos = read(partida->apuesta_pipe[i][0], buffer, sizeof(buffer) - 1);
                if (bytes_recibidos > 0) {
                    buffer[bytes_recibidos] = '\0';
                    if (strncmp(buffer, "SIGUIENTE_RONDA", 15) == 0 || strncmp(buffer, "SIGUIENTE_RONDA|", 16) == 0) {
                        jugadores_confirmaron[i] = 1;
                        confirmaciones_recibidas++;
                        printf("Jugador %s confirmó SIGUIENTE_RONDA (confirmaciones: %d/%d)\n",partida->jugadores[i], confirmaciones_recibidas, MAX_PLAYERS_PER_GAME);
                    }
                }
            }
            usleep(100000);
        }
        printf(">> Todas las confirmaciones recibidas. Pasando a la siguiente ronda...\n");
    }

    // Determinar el ganador final
    int max_puntos = -1;
    int ganador = -1;
    for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
        if (partida->puntos[i] > max_puntos) {
            max_puntos = partida->puntos[i];
            ganador = i;
        }
    }
    // Mensaje de ganador para depuración
    printf(">> %s es el ganador con %d puntos\n", partida->jugadores[ganador], max_puntos);
    // Notificar el resultado final a los jugadores
    for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
        char mensaje[256];
        if (i == ganador) {
            snprintf(mensaje, sizeof(mensaje), "FIN|GANADOR|%s|PUNTOS|%d", partida->jugadores[ganador], max_puntos);
        } else {
            snprintf(mensaje, sizeof(mensaje), "FIN|PERDEDOR|%s|PUNTOS|%d", partida->jugadores[ganador], partida->puntos[i]);
        }
        write(partida->sockets[i], mensaje, strlen(mensaje));
    }

    printf("Partida finalizada. Ganador: %s\n", partida->jugadores[ganador]);
    pthread_exit(NULL);
}

// Función para iniciar una partida
void iniciar_partida(const char *creador, ListaConectados *miLista) {
    pthread_mutex_lock(&partidas_mutex);
    int index = BuscarPartidaPorCreador(creador);
    if (index == -1 || partidas[index].num_jugadores != MAX_PLAYERS_PER_GAME) {
        pthread_mutex_unlock(&partidas_mutex);
        printf("Error: La partida no tiene suficientes jugadores o no existe\n");
        return;
    }

    PartidaActiva *nueva_partida = &partidas_activas[num_partidas_activas++];
    for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
        strncpy(nueva_partida->jugadores[i], partidas[index].jugadores[i], 50);
        int pos = DamePosicion(miLista, partidas[index].jugadores[i]);
        if (pos == -1) {
            printf("Error: Jugador %s no está conectado\n", partidas[index].jugadores[i]);
            pthread_mutex_unlock(&partidas_mutex);
            return;
        }
        nueva_partida->sockets[i] = miLista->conectados[pos].socket;
        nueva_partida->puntos[i] = 0;

        // Notificar a los jugadores que la partida ha comenzado
        char mensaje[256];
        snprintf(mensaje, sizeof(mensaje), "PARTIDA_INICIADA|%s", creador);
        if (write(nueva_partida->sockets[i], mensaje, strlen(mensaje)) <= 0) {
            perror("Error enviando mensaje de inicio de partida");
        } else {
            printf("Mensaje de inicio enviado a %s\n", partidas[index].jugadores[i]);
        }
    }
    nueva_partida->ronda_actual = 0;

    // Crear pipes para las apuestas
    for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
        if (pipe(nueva_partida->apuesta_pipe[i]) == -1) {
            perror("Error creando pipe de apuesta");
            // Maneja el error si quieres
        }
        // Opcional: poner los pipes en modo no bloqueante
        fcntl(nueva_partida->apuesta_pipe[i][0], F_SETFL, O_NONBLOCK);
        fcntl(nueva_partida->apuesta_pipe[i][1], F_SETFL, O_NONBLOCK);
    }

    nueva_partida->tiempo_inicio = time(NULL);

    pthread_create(&nueva_partida->thread_id, NULL, manejar_partida, nueva_partida);
    pthread_mutex_unlock(&partidas_mutex);
}

// Manejador de clientes
void *handle_client(void *arg) {
    ThreadData *data = (ThreadData *)arg;
    int sock_conn = *data->socket;
    ListaConectados *lista = data->lista;
    pthread_mutex_t *mutex = data->mutex;
    NotificationHandler *notif_handler = data->notif_handler;

    char buff[2048], buff2[2048];
    int ret;

    printf("Nuevo cliente conectado. Hilo: %ld\n", pthread_self());

    while (!shutdown_requested) {
        memset(buff, 0, sizeof(buff));
        memset(buff2, 0, sizeof(buff2));

        ret = read(sock_conn, buff, sizeof(buff) - 1);
        if (ret <= 0) {
            if (ret == 0) {
                printf("Cliente cerró conexión (hilo %ld)\n", pthread_self());
                break;
            } else if (errno == EAGAIN || errno == EWOULDBLOCK) {
                usleep(100000);
                continue;
            } else {
                perror("Error grave en read");
                break;
            }
        }
        buff[ret] = '\0';

        // Copia segura del mensaje original para imprimirlo
        char mensaje_original[2048];
        strncpy(mensaje_original, buff, sizeof(mensaje_original));
        mensaje_original[sizeof(mensaje_original) - 1] = '\0';

        //printf("Hilo %ld recibió: %s\n", pthread_self(), mensaje_original);
        char *p = strtok(buff, "|");
        if (p == NULL) {
            strcpy(buff2, "Error: Formato inválido,");
            write(sock_conn, buff2, strlen(buff2)+1);
            continue;
        }
        int codigo = atoi(p);
        p = strtok(NULL, "|");

        if (codigo == 1 || codigo == 2) {
            if (p != NULL) {
                strncpy(data->nombre, p, sizeof(data->nombre) - 1);
                data->nombre[sizeof(data->nombre) - 1] = '\0';
            }
        }

        if (codigo == 1) { // Registro
            p = strtok(NULL, "|");
            char password[50];
            if (p != NULL) {
                strncpy(password, p, sizeof(password) - 1);
                password[sizeof(password) - 1] = '\0';

                int reg_status = register_player(conn, data->nombre, password);
                if (reg_status == 1) {
                    strcpy(buff2, "Registro exitoso,");
                } else if (reg_status == 0) {
                    strcpy(buff2, "Error: Usuario ya existe,");
                } else {
                    strcpy(buff2, "Error: Fallo en registro,");
                }
            } else {
                strcpy(buff2, "Error: Datos incompletos,");
            }

            // Enviar respuesta al cliente
            if (write(sock_conn, buff2, strlen(buff2)) <= 0) {
            perror("Error enviando respuesta de registro");
            break; // Salir del bucle si hay un error al enviar
            }
        } else if (codigo == 2) { // Login
            p = strtok(NULL, "|");
            char password[50];
            if (p != NULL) {
                strncpy(password, p, sizeof(password) - 1);
                password[sizeof(password) - 1] = '\0';

                int login_status = check_login_credentials(conn, data->nombre, password);
                if (login_status == 1) {
                    strcpy(buff2, "Login exitoso,");

                    pthread_mutex_lock(mutex);
                    int result = NuevoJugador(lista, data->nombre, sock_conn);
                    if (result == 0) {
                        char notif_msg[256];
                        snprintf(notif_msg, sizeof(notif_msg), "LOGIN:%s", data->nombre);
                        send_notification(notif_handler, notif_msg, 1);
                    }
                    pthread_mutex_unlock(mutex);

                    if (result == -1) {
                        strcat(buff2, " Error: Lista llena,");
                    } else if (result == -2) {
                        strcat(buff2, " Error: Ya conectado,");
                    }
                } else {
                    strcpy(buff2, "Error: Credenciales inválidas,");
                }
            } else {
                strcpy(buff2, "Error: Datos incompletos,");
            }
        }else if (codigo == 3) { // Crear partida
            p=strtok(NULL,"|");
            
            
        }
        else if (codigo == 4) { // Jugadores conectados
            char misConectados[300];
            pthread_mutex_lock(mutex);
            DameConectados(lista, misConectados);
            pthread_mutex_unlock(mutex);
            
            snprintf(buff2, sizeof(buff2), "%s", misConectados);
            if (write(sock_conn, buff2, strlen(buff2)) <= 0) {
                perror("Error enviando respuesta a 4|");
            }
        }
        else if (codigo == 5) { // Historial de partidas
            pthread_mutex_lock(mutex);
            int pos = DamePosicion(lista, data->nombre);
            pthread_mutex_unlock(mutex);
            
            if (pos == -1) {
                strcpy(buff2, "Error: No autenticado");
            } else {
                const char* query = "SELECT m.match_id, m.winner_name, "
                                    "MAX(CASE WHEN mp.result = 'lose' THEN mp.player_name END) AS perdedor1, "
                                    "MIN(CASE WHEN mp.result = 'lose' THEN mp.player_name END) AS perdedor2 "
                                    "FROM matches m "
                                    "JOIN match_players mp ON m.match_id = mp.match_id "
                                    "GROUP BY m.match_id "
                                    "ORDER BY m.match_date DESC;";
                
                if (mysql_query(conn, query)) {
                    strcpy(buff2, "Error: DB query failed");
                } else {
                    MYSQL_RES *result = mysql_store_result(conn);
                    if (result == NULL) {
                        strcpy(buff2, "Error: No results");
                    } else {
                        buff2[0] = '\0';
                        MYSQL_ROW row;
                        while ((row = mysql_fetch_row(result))) {
                            char match_info[256];
                            snprintf(match_info, sizeof(match_info), "%s:%s:%s:%s|",
                                    row[0] ? row[0] : "NULL", // match_id
                                    row[1] ? row[1] : "NULL", // ganador
                                    row[2] ? row[2] : "NULL", // perdedor1
                                    row[3] ? row[3] : "NULL"  // perdedor2
                            );
                            strcat(buff2, match_info);
                        }
                        if (strlen(buff2) > 0 && buff2[strlen(buff2)-1] == '|') {
                            buff2[strlen(buff2)-1] = '\0';
                        }
                        mysql_free_result(result);
                    }
                }
            }
            if (write(sock_conn, buff2, strlen(buff2)) <= 0) {
                perror("Error enviando historial");
            }
        }
        else if (codigo == 6) { // Logout
            pthread_mutex_lock(mutex);
            Eliminar(lista, data->nombre);
            
            char notif_msg[256];
            snprintf(notif_msg, sizeof(notif_msg), 
                     "Logout: %s", data->nombre);
            send_notification(notif_handler, notif_msg, 1);
            
            pthread_mutex_unlock(mutex);
            
	    snprintf(notif_msg, sizeof(notif_msg), "LOGOUT:%s", data->nombre);
            strcpy(buff2, "Sesión cerrada correctamente,");
            write(sock_conn, buff2, strlen(buff2)+1);
            break;
        } 
        else if (codigo == 7) { // Chat
            char remitente[50];
            char mensaje_usuario[512]; // Reducir el tamaño máximo del mensaje

            if (p != NULL) {
                strncpy(remitente, p, sizeof(remitente) - 1);
                remitente[sizeof(remitente) - 1] = '\0';

                p = strtok(NULL, "|");
                if (p != NULL) {
                    strncpy(mensaje_usuario, p, sizeof(mensaje_usuario) - 1);
                    mensaje_usuario[sizeof(mensaje_usuario) - 1] = '\0';

                    pthread_mutex_lock(mutex);
                    int pos = DamePosicion(lista, remitente);
                    pthread_mutex_unlock(mutex);

                    if (pos == -1) {
                        strcpy(buff2, "Error: No autenticado");
                    } else {
                        // Aumentar el tamaño del buffer para evitar truncamientos
                        char mensaje_chat[NOTIFICATION_MSG_SIZE + 100]; // NOTIFICATION_MSG_SIZE es 512

                        // Validar el tamaño antes de usar snprintf
                        size_t required_size = strlen(remitente) + strlen(mensaje_usuario) + 7; // "CHAT|", ": ", y '\0'
                        if (required_size < sizeof(mensaje_chat)) {
                            snprintf(mensaje_chat, sizeof(mensaje_chat), "CHAT|%s: %s", remitente, mensaje_usuario);

                            pthread_mutex_lock(mutex);
                            for (int i = 0; i < lista->num; i++) {
                                int sock = lista->conectados[i].socket;
                                if (write(sock, mensaje_chat, strlen(mensaje_chat)) <= 0) {
                                    perror("Error enviando mensaje a cliente");
                                    Eliminar(lista, lista->conectados[i].nombre);
                                    i--;
                                }
                            }
                            pthread_mutex_unlock(mutex);

                            strcpy(buff2, "Mensaje enviado");
                        } else {
                            strcpy(buff2, "Error: Mensaje demasiado largo");
                        }
                    }
                } else {
                    strcpy(buff2, "Error: Mensaje vacío");
                }
            } else {
                strcpy(buff2, "Error: Datos incompletos");
            }
        } else if (codigo == 8) { // Crear partida
            pthread_mutex_lock(mutex);
            int pos = DamePosicion(lista, data->nombre);
            pthread_mutex_unlock(mutex);

            if (pos == -1) {
                strcpy(buff2, "Error: No autenticado");
            } else {
                if (BuscarPartidaPorCreador(data->nombre) != -1) {
                    strcpy(buff2, "Error: Ya tienes una partida creada");
                } else {
                    // Crear nueva partida
                    strncpy(partidas[num_partidas].creador, data->nombre, 50);
                    strncpy(partidas[num_partidas].jugadores[0], data->nombre, 50);
                    partidas[num_partidas].num_jugadores = 1;
                    num_partidas++;

                    snprintf(buff2, sizeof(buff2), "Partida creada por %s", data->nombre);
                    printf("Partida creada por %s\n", data->nombre);

                }
            }
            write(sock_conn, buff2, strlen(buff2)+1);
        } else if (codigo == 9) { // Enviar invitación
            char invitado[50];
            if (p != NULL) {
                strncpy(invitado, p, sizeof(invitado) - 1);
                invitado[sizeof(invitado) - 1] = '\0';

                pthread_mutex_lock(mutex);
                int pos_invitado = DamePosicion(lista, invitado);
                pthread_mutex_unlock(mutex);
                printf("9|%s|%s\n", data->nombre, invitado);

                if (pos_invitado == -1) {
                    strcpy(buff2, "Error: Jugador no encontrado o no conectado");
                } else {
                    int sock_invitado = lista->conectados[pos_invitado].socket;
                    char invitacion[256];
                    snprintf(invitacion, sizeof(invitacion), "INVITACION|%s te ha invitado a una partida", data->nombre);

                    if (write(sock_invitado, invitacion, strlen(invitacion)) <= 0) {
                        perror("Error enviando invitación");
                    } else {
                        strcpy(buff2, "Invitación enviada");
                    }
                }
            } else {
                strcpy(buff2, "Error: Datos incompletos");
            }
            write(sock_conn, buff2, strlen(buff2)+1);
        } else if (codigo == 10) { // Obtener jugadores de la partida
            pthread_mutex_lock(mutex);
            int pos = DamePosicion(lista, data->nombre);
            pthread_mutex_unlock(mutex);

            if (pos == -1) {
                strcpy(buff2, "Error: No autenticado");
            } else {
                int index = -1;

                // Buscar la partida en la que el jugador está participando
                for (int i = 0; i < num_partidas; i++) {
                    for (int j = 0; j < partidas[i].num_jugadores; j++) {
                        if (strcmp(partidas[i].jugadores[j], data->nombre) == 0) {
                            index = i;
                            break;
                        }
                    }
                    if (index != -1) break;
                }

                if (index == -1) {
                    strcpy(buff2, "Error: No estás en una partida");
                } else {
                    snprintf(buff2, sizeof(buff2), "Jugadores en la partida: ");
                    for (int i = 0; i < partidas[index].num_jugadores; i++) {
                        strncat(buff2, partidas[index].jugadores[i], sizeof(buff2) - strlen(buff2) - 1);
                        if (i < partidas[index].num_jugadores - 1) {
                            strncat(buff2, ", ", sizeof(buff2) - strlen(buff2) - 1);
                        }
                    }
                }
            }
            write(sock_conn, buff2, strlen(buff2)+1);
        } else if (codigo == 11) { // Unirse a una partida
            char creador[50];
            if (p != NULL) {
                strncpy(creador, p, sizeof(creador) - 1);
                creador[sizeof(creador) - 1] = '\0';

                pthread_mutex_lock(mutex);
                int pos = DamePosicion(lista, data->nombre);
                pthread_mutex_unlock(mutex);

                if (pos == -1) {
                    strcpy(buff2, "Error: No autenticado");
                } else {
                    int result = AñadirJugadorAPartida(creador, data->nombre);
                    if (result == 0) {
                        snprintf(buff2, sizeof(buff2), "Te has unido a la partida de %s", creador);
                    } else if (result == -1) {
                        strcpy(buff2, "Error: Partida no encontrada");
                    } else if (result == -2) {
                        strcpy(buff2, "Error: Partida llena");
                    }
                }
            }
        }else if (codigo == 12) { // Iniciar partida manualmente
            pthread_mutex_lock(mutex);
            int pos = DamePosicion(lista, data->nombre);
            pthread_mutex_unlock(mutex);

            if (pos == -1) {
                strcpy(buff2, "Error: No autenticado");
            } else {
                int index = BuscarPartidaPorCreador(data->nombre);
                if (index == -1) {
                    strcpy(buff2, "Error: No tienes una partida creada");
                } else if (partidas[index].num_jugadores < MAX_PLAYERS_PER_GAME) {
                    snprintf(buff2, sizeof(buff2), "Error: La partida aún no tiene suficientes jugadores (%d/%d)",
                                partidas[index].num_jugadores, MAX_PLAYERS_PER_GAME);
                } else {
                    iniciar_partida(data->nombre, lista);
                                snprintf(buff2, sizeof(buff2), "Partida Iniciada por %s\n", data->nombre); // <-- Aquí el \n
                }
            }
            write(sock_conn, buff2, strlen(buff2)+1);
        } else if (codigo == 13) { // Apostar (adivinar el número)
            char nombre_jugador[50] = "";
            int apuesta = -1;

            // p ya apunta al nombre, siguiente es la apuesta
            if (p != NULL) {
                strncpy(nombre_jugador, p, sizeof(nombre_jugador) - 1);
                nombre_jugador[sizeof(nombre_jugador) - 1] = '\0';
                p = strtok(NULL, "|");
                if (p != NULL) {
                    apuesta = atoi(p);
                    printf("Hilo %ld recibió: 13|%s|%d\n", pthread_self(), nombre_jugador, apuesta);
                } else {
                    printf("Hilo %ld recibió: 13|%s|?\n", pthread_self(), nombre_jugador);
                }
            } else {
                printf("Hilo %ld recibió: 13|?|?\n", pthread_self());
            }

            pthread_mutex_lock(&partidas_mutex);
            int partida_encontrada = -1, jugador_idx = -1;
            // Buscar en qué partida activa está este jugador
            for (int i = 0; i < num_partidas_activas; i++) {
                for (int j = 0; j < MAX_PLAYERS_PER_GAME; j++) {
                    if (strcmp(partidas_activas[i].jugadores[j], data->nombre) == 0) {
                        partida_encontrada = i;
                        jugador_idx = j;
                        break;
                    }
                }
                if (partida_encontrada != -1) break;
            }
            if (partida_encontrada == -1 || jugador_idx == -1) {
                snprintf(buff2, sizeof(buff2), "Error: No estás en una partida activa");
            } else if (apuesta < 1 || apuesta > 6) {
                snprintf(buff2, sizeof(buff2), "Error: Apuesta inválida (debe ser entre 1 y 6)");
            } else {
                // Enviar la apuesta al hilo de la partida usando el socket, incluyendo el nombre
                char apuesta_msg[64];
                snprintf(apuesta_msg, sizeof(apuesta_msg), "APUESTA|%s|%d", data->nombre, apuesta);
                apuesta_msg[strlen(apuesta_msg)] = '\0';
                if (write(partidas_activas[partida_encontrada].apuesta_pipe[jugador_idx][1],apuesta_msg, strlen(apuesta_msg) + 1) <= 0) {
                    snprintf(buff2, sizeof(buff2), "Error: No se pudo enviar la apuesta");
                } else {
                    snprintf(buff2, sizeof(buff2), "Apuesta recibida de %s: %d", data->nombre, apuesta);
                }
            }
            pthread_mutex_unlock(&partidas_mutex);
            write(sock_conn, buff2, strlen(buff2)+1);
        } else if (codigo == 14) { // Confirmación de siguiente ronda
            char nombre_jugador[50] = "";
            if (p != NULL) {
                strncpy(nombre_jugador, p, sizeof(nombre_jugador) - 1);
                nombre_jugador[sizeof(nombre_jugador) - 1] = '\0';
            } else {
                strncpy(nombre_jugador, data->nombre, sizeof(nombre_jugador) - 1);
                nombre_jugador[sizeof(nombre_jugador) - 1] = '\0';
            }

            pthread_mutex_lock(&partidas_mutex);
            int partida_encontrada = -1, jugador_idx = -1;
            // Buscar en qué partida activa está este jugador
            for (int i = 0; i < num_partidas_activas; i++) {
                for (int j = 0; j < MAX_PLAYERS_PER_GAME; j++) {
                    if (strcmp(partidas_activas[i].jugadores[j], nombre_jugador) == 0) {
                        partida_encontrada = i;
                        jugador_idx = j;
                        break;
                    }
                }
                if (partida_encontrada != -1) break;
            }
            if (partida_encontrada == -1 || jugador_idx == -1) {
                snprintf(buff2, sizeof(buff2), "Error: No estás en una partida activa");
            } else {
                // Enviar la confirmación al hilo de la partida usando el pipe
                char confirmacion_msg[80];
                snprintf(confirmacion_msg, sizeof(confirmacion_msg), "SIGUIENTE_RONDA|%s", nombre_jugador);
                if (write(partidas_activas[partida_encontrada].apuesta_pipe[jugador_idx][1], confirmacion_msg, strlen(confirmacion_msg)) <= 0) {
                    snprintf(buff2, sizeof(buff2), "Error: No se pudo enviar la confirmación de siguiente ronda");
                } else {
                    snprintf(buff2, sizeof(buff2), "Confirmación de siguiente ronda recibida de %s", nombre_jugador);
                }
            }
            pthread_mutex_unlock(&partidas_mutex);
            write(sock_conn, buff2, strlen(buff2)+1);
        } else if (codigo == 15) { // Jugador pulsa SALIR tras la partida
            pthread_mutex_lock(&partidas_mutex);
            int partida_encontrada = -1, jugador_idx = -1;
            // Buscar la partida activa y el índice del jugador
            for (int i = 0; i < num_partidas_activas; i++) {
                for (int j = 0; j < MAX_PLAYERS_PER_GAME; j++) {
                    if (strcmp(partidas_activas[i].jugadores[j], data->nombre) == 0) {
                        partida_encontrada = i;
                        jugador_idx = j;
                        break;
                    }
                }
                if (partida_encontrada != -1) break;
            }
            if (partida_encontrada != -1 && jugador_idx != -1) {
                partidas_activas[partida_encontrada].jugadores_salidos[jugador_idx] = 1;

                // ¿Han salido todos?
                int todos_salidos = 1;
                for (int k = 0; k < MAX_PLAYERS_PER_GAME; k++) {
                    if (partidas_activas[partida_encontrada].jugadores_salidos[k] == 0)
                        todos_salidos = 0;
                }

                if (todos_salidos) {
                    // Calcula la duración real
                    time_t tiempo_fin = time(NULL);
                    int duracion = (int)difftime(tiempo_fin, partidas_activas[partida_encontrada].tiempo_inicio);

                    // Guardar en la base de datos el historial de la partida
                    char query[512];
                    const char* winner = partidas_activas[partida_encontrada].jugadores[0];
                    int max_puntos = partidas_activas[partida_encontrada].puntos[0];
                    int ganador_idx = 0;
                    for (int i = 1; i < MAX_PLAYERS_PER_GAME; i++) {
                        if (partidas_activas[partida_encontrada].puntos[i] > max_puntos) {
                            max_puntos = partidas_activas[partida_encontrada].puntos[i];
                            winner = partidas_activas[partida_encontrada].jugadores[i];
                            ganador_idx = i;
                        }
                    }
                    snprintf(query, sizeof(query), "INSERT INTO matches (match_date, duration, winner_name) VALUES (NOW(), %d, '%s')", duracion, winner);
                    if (mysql_query(conn, query)) {
                        fprintf(stderr, "Error insertando en matches: %s\n", mysql_error(conn));
                    } else {
                        int match_id = (int)mysql_insert_id(conn);
                        // 2. Insertar en match_players
                        for (int i = 0; i < MAX_PLAYERS_PER_GAME; i++) {
                            const char* result = (i == ganador_idx) ? "win" : "lose";
                            snprintf(query, sizeof(query),
                                "INSERT INTO match_players (match_id, player_name, result) VALUES (%d, '%s', '%s')",
                                match_id, partidas_activas[partida_encontrada].jugadores[i], result);
                            if (mysql_query(conn, query)) {
                                fprintf(stderr, "Error insertando en match_players: %s\n", mysql_error(conn));
                            }
                        }
                    }

                    // Eliminar la partida activa (compactar el array)
                    for (int i = partida_encontrada; i < num_partidas_activas - 1; i++) {
                        partidas_activas[i] = partidas_activas[i + 1];
                    }
                    num_partidas_activas--;
                }
            }
            pthread_mutex_unlock(&partidas_mutex);

            // Opcional: enviar confirmación al cliente
            strcpy(buff2, "Has salido de la partida.");
        }
        else if (codigo == 16) { // Darse de baja
            pthread_mutex_lock(mutex);
            int pos = DamePosicion(lista, data->nombre);
            pthread_mutex_unlock(mutex);

            if (pos == -1) {
                strcpy(buff2, "Error: No autenticado");
            } else {
                // Borra primero de match_players y match_logs
                char query[256];
                snprintf(query, sizeof(query), "DELETE FROM match_players WHERE player_name='%s'", data->nombre);
                mysql_query(conn, query);
                snprintf(query, sizeof(query), "DELETE FROM match_logs WHERE player_name='%s'", data->nombre);
                mysql_query(conn, query);

                // Ahora borra de players
                snprintf(query, sizeof(query), "DELETE FROM players WHERE username='%s'", data->nombre);
                if (mysql_query(conn, query)) {
                    snprintf(buff2, sizeof(buff2), "Error eliminando usuario: %s", mysql_error(conn));
                } else {
                    pthread_mutex_lock(mutex);
                    Eliminar(lista, data->nombre);
                    pthread_mutex_unlock(mutex);

                    strcpy(buff2, "Se le ha desconectado y eliminado su usuario.");
                }
            }
            write(sock_conn, buff2, strlen(buff2)+1);
            usleep(200000);
            close(sock_conn);
            pthread_exit(NULL);
        }
        else if (codigo == 17) {
            pthread_mutex_lock(mutex);
            int pos = DamePosicion(lista, data->nombre);
            pthread_mutex_unlock(mutex);

            if (pos == -1) {
                strcpy(buff2, "Error: No autenticado");
            } else {
                char query[1024];
                snprintf(query, sizeof(query),
                    "SELECT DISTINCT mp2.player_name FROM match_players mp1 "
                    "JOIN match_players mp2 ON mp1.match_id = mp2.match_id "
                    "WHERE mp1.player_name = '%s' AND mp2.player_name != '%s';",
                    data->nombre, data->nombre);

                if (mysql_query(conn, query)) {
                    snprintf(buff2, sizeof(buff2), "Error: DB query failed");
                } else {
                    MYSQL_RES *result = mysql_store_result(conn);
                    if (!result) {
                        snprintf(buff2, sizeof(buff2), "Error: No results");
                    } else {
                        strcpy(buff2, "CONOCIDOS|");
                        MYSQL_ROW row;
                        int jugadores_agregados = 0;
                        while ((row = mysql_fetch_row(result))) {
                            if (row[0]) {
                                strncat(buff2, row[0], sizeof(buff2) - strlen(buff2) - 2);
                                strncat(buff2, "|", sizeof(buff2) - strlen(buff2) - 2);
                                jugadores_agregados++;
                            }
                        }
                        if (strlen(buff2) == 0) {
                            strcpy(buff2, "CONOCIDOS|No hay jugadores conocidos");

                        }
                        mysql_free_result(result);
                    }
                }
            }

            printf(">> Enviando jugadores conocidos: %s\n", buff2);
        }
        else if (codigo == 18) { // Historial entre dos jugadores concretos
            char jugador1[50] = "", jugador2[50] = "";
            if (p != NULL) {
                strncpy(jugador1, p, sizeof(jugador1) - 1);
                jugador1[sizeof(jugador1) - 1] = '\0';
                p = strtok(NULL, "|");
                if (p != NULL) {
                    strncpy(jugador2, p, sizeof(jugador2) - 1);
                    jugador2[sizeof(jugador2) - 1] = '\0';

                    const char* query_fmt =
                        "SELECT m.match_id, m.winner_name, "
                        "GROUP_CONCAT(CASE WHEN mp.result = 'lose' THEN mp.player_name END) AS perdedores "
                        "FROM matches m "
                        "JOIN match_players mp ON m.match_id = mp.match_id "
                        "WHERE m.match_id IN ("
                        "  SELECT match_id FROM match_players "
                        "  WHERE LOWER(player_name) IN (LOWER('%s'), LOWER('%s')) "
                        "  GROUP BY match_id "
                        "  HAVING COUNT(DISTINCT LOWER(player_name)) = 2"
                        ") "
                        "GROUP BY m.match_id "
                        "ORDER BY m.match_date DESC;";

                    char query[1024];
                    snprintf(query, sizeof(query), query_fmt, jugador1, jugador2);

                    if (mysql_query(conn, query)) {
                        snprintf(buff2, sizeof(buff2), "Error: DB query failed");
                    } else {
                        MYSQL_RES *result = mysql_store_result(conn);
                        if (result == NULL) {
                            snprintf(buff2, sizeof(buff2), "Error: No results");
                        } else {
                            buff2[0] = '\0';
                            MYSQL_ROW row;
                            int hay_partidas = 0;
                            while ((row = mysql_fetch_row(result))) {
                                hay_partidas = 1;
                                char match_info[256];
                                snprintf(match_info, sizeof(match_info), "%s:%s:%s|",
                                    row[0] ? row[0] : "NULL",
                                    row[1] ? row[1] : "NULL",
                                    row[2] ? row[2] : "NULL"
                                );
                                strncat(buff2, match_info, sizeof(buff2) - strlen(buff2) - 1);
                            }
                            mysql_free_result(result);
                            if (!hay_partidas) {
                                snprintf(buff2, sizeof(buff2), "NO_HAY_PARTIDAS");
                            } else if (strlen(buff2) > 0 && buff2[strlen(buff2)-1] == '|') {
                                buff2[strlen(buff2)-1] = '\0';
                            }
                        }
                    }
                } else {
                    snprintf(buff2, sizeof(buff2), "Error: Falta jugador2");
                }  
            } else {
                snprintf(buff2, sizeof(buff2), "Error: Falta jugador1");
            }
            write(sock_conn, buff2, strlen(buff2)+1);
        }
        else if (codigo == 19) { // Lista de partidas por fecha
            char fecha[20] = "";
            if (p != NULL) {
                strncpy(fecha, p, sizeof(fecha) - 1);
                fecha[sizeof(fecha) - 1] = '\0';

                // Convertir formato dd/mm/yyyy a yyyy-mm-dd
                int d, m, y;
                if (sscanf(fecha, "%d/%d/%d", &d, &m, &y) == 3) {
                    char fecha_sql[20];
                    snprintf(fecha_sql, sizeof(fecha_sql), "%04d-%02d-%02d", y, m, d);

                    char query[512];
                    snprintf(query, sizeof(query),
                        "SELECT m.match_id, m.match_date, m.winner_name, "
                        "GROUP_CONCAT(CASE WHEN mp.result = 'lose' THEN mp.player_name END) AS perdedores "
                        "FROM matches m "
                        "JOIN match_players mp ON m.match_id = mp.match_id "
                        "WHERE DATE(m.match_date) = '%s' "
                        "GROUP BY m.match_id "
                        "ORDER BY m.match_date DESC;",
                        fecha_sql);

                    if (mysql_query(conn, query)) {
                        snprintf(buff2, sizeof(buff2), "Error: DB query failed");
                    } else {
                        MYSQL_RES *result = mysql_store_result(conn);
                        if (result == NULL) {
                            snprintf(buff2, sizeof(buff2), "Error: No results");
                        } else {
                            buff2[0] = '\0';
                            MYSQL_ROW row;
                            int hay_partidas = 0;
                            while ((row = mysql_fetch_row(result))) {
                                hay_partidas = 1;
                                char match_info[256];
                                snprintf(match_info, sizeof(match_info), "%s:%s:%s:%s|",
                                    row[0] ? row[0] : "NULL", // match_id
                                    row[1] ? row[1] : "NULL", // match_date
                                    row[2] ? row[2] : "NULL", // ganador
                                    row[3] ? row[3] : "NULL"  // perdedores
                                );
                                strncat(buff2, match_info, sizeof(buff2) - strlen(buff2) - 1);
                            }
                            mysql_free_result(result);
                            if (!hay_partidas) {
                                snprintf(buff2, sizeof(buff2), "NO_HAY_PARTIDAS");
                            } else if (strlen(buff2) > 0 && buff2[strlen(buff2)-1] == '|') {
                                buff2[strlen(buff2)-1] = '\0';
                            }
                        }
                    }
                } else {
                    snprintf(buff2, sizeof(buff2), "Error: Formato de fecha inválido");
                }
            } else {
                snprintf(buff2, sizeof(buff2), "Error: Falta fecha");
            }
            write(sock_conn, buff2, strlen(buff2)+1);
        }
        else {
            strcpy(buff2, "Error: Código no válido,");
        }

        if (write(sock_conn, buff2, strlen(buff2)+1) <= 0) {
            perror("Error al enviar respuesta");
            continue;
        }
    }

    if (strlen(data->nombre) > 0) {
    pthread_mutex_lock(mutex);
    Eliminar(lista, data->nombre);

    char notif_msg[256];
    snprintf(notif_msg, sizeof(notif_msg), "LOGOUT:%s", data->nombre);
    send_notification(notif_handler, notif_msg, 1);

    pthread_mutex_unlock(mutex);
    }

    close(sock_conn);
    free(data->socket);
    free(data);
    printf("Hilo %ld finalizado\n", pthread_self());
}

int main(int argc, char *argv[]) {
    ListaConectados miLista = {0};
    int sock_listen, sock_conn;
    struct sockaddr_in serv_adr;
    pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

    // Configurar manejador de señales
    struct sigaction sa;
    sa.sa_handler = handle_signal;
    sigemptyset(&sa.sa_mask);
    sa.sa_flags = 0;
    sigaction(SIGINT, &sa, NULL);
    sigaction(SIGTERM, &sa, NULL);

    // Inicializar sistema de notificaciones
    NotificationHandler notif_handler;
    notif_handler.lista = &miLista;
    notif_handler.mutex = &mutex;

    if (pipe(notif_handler.notification_pipe) == -1) {
        perror("Error creando pipe");
        exit(EXIT_FAILURE);
    }

    for (int i = 0; i < 2; i++) {
        int flags = fcntl(notif_handler.notification_pipe[i], F_GETFL);
        fcntl(notif_handler.notification_pipe[i], F_SETFL, flags | O_NONBLOCK);
    }

    // Conectar a la base de datos
    connect_db();

    // Inicializar la semilla del generador de números aleatorios
    srand(time(NULL));

    // Crear thread de notificaciones
    if (pthread_create(&notif_handler.thread_id, NULL, notification_thread, &notif_handler) != 0) {
        perror("Error creando thread de notificaciones");
        exit(EXIT_FAILURE);
    }

    // Configurar socket del servidor
    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
        perror("Error creando socket");
        exit(EXIT_FAILURE);
    }

    int optval = 1;
    if (setsockopt(sock_listen, SOL_SOCKET, SO_REUSEADDR, &optval, sizeof(optval))) {
        perror("Error configurando SO_REUSEADDR");
        close(sock_listen);
        exit(EXIT_FAILURE);
    }

    int puerto = 50077;
    memset(&serv_adr, 0, sizeof(serv_adr));
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
    serv_adr.sin_port = htons(puerto);

    if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0) {
        perror("Error en bind");
        close(sock_listen);
        exit(EXIT_FAILURE);
    }

    if (listen(sock_listen, 5) < 0) {
        perror("Error en listen");
        close(sock_listen);
        exit(EXIT_FAILURE);
    }

    printf("Servidor iniciado. Esperando conexiones...\n");

    while (!shutdown_requested) {
        sock_conn = accept(sock_listen, NULL, NULL);
        if (sock_conn < 0) {
            if (errno == EINTR) continue;
            perror("Error aceptando conexión");
            continue;
        }

        printf("Nueva conexión aceptada\n");

        ThreadData *data = malloc(sizeof(ThreadData));
        if (data == NULL) {
            perror("Error asignando memoria para ThreadData");
            close(sock_conn);
            continue;
        }

        data->socket = malloc(sizeof(int));
        if (data->socket == NULL) {
            perror("Error asignando memoria para socket");
            free(data);
            close(sock_conn);
            continue;
        }

        *(data->socket) = sock_conn;
        data->lista = &miLista;
        data->mutex = &mutex;
        data->notif_handler = &notif_handler;
        data->nombre[0] = '\0';

        pthread_t thread;
        if (pthread_create(&thread, NULL, handle_client, data) != 0) {
            perror("Error creando hilo para cliente");
            free(data->socket);
            free(data);
            close(sock_conn);
        }

        pthread_detach(thread);
    }

    printf("Cerrando servidor...\n");

    close(notif_handler.notification_pipe[0]);
    close(notif_handler.notification_pipe[1]);
    pthread_join(notif_handler.thread_id, NULL);
    close(sock_listen);
    mysql_close(conn);

    printf("Servidor cerrado correctamente\n");
    return 0;
}
