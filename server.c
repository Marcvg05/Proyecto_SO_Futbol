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

#define DB_HOST "shiva2.upc.es"
#define DB_USER "root"
#define DB_PASS "mysql"
#define DB_NAME "T7_BBDD"
#define MAX_PLAYERS 100
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

typedef struct{
    Conectado player;
    int puntuacion;
    int posicion;
}jugador;

typedef struct{
    jugador jugadores[3];
    int ronda;
}Partida;

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

int register_player(MYSQL *conn, const char *username, const char *password) {
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
        return 0;
    }

    snprintf(query, sizeof(query),
             "INSERT INTO players (username, password) VALUES ('%s', '%s')",
             username, password);

    if (mysql_query(conn, query)) {
        fprintf(stderr, "Registration error: %s\n", mysql_error(conn));
        return -1;
    }

    return 1;
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
            usleep(100000);
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

// Manejador de clientes
void *handle_client(void *arg) {
    ThreadData *data = (ThreadData *)arg;
    int sock_conn = *data->socket;
    ListaConectados *lista = data->lista;
    pthread_mutex_t *mutex = data->mutex;
    NotificationHandler *notif_handler = data->notif_handler;
    
    char buff[2048], buff2[2048];
    int ret;

    struct timeval tv = {30, 0};
    setsockopt(sock_conn, SOL_SOCKET, SO_RCVTIMEO, (const char*)&tv, sizeof(tv));

    int flags = fcntl(sock_conn, F_GETFL, 0);
    fcntl(sock_conn, F_SETFL, flags | O_NONBLOCK);

    printf("Nuevo cliente conectado. Hilo: %ld\n", pthread_self());

    while(!shutdown_requested) {
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

        printf("Hilo %ld recibió: %s\n", pthread_self(), buff);
        char *p = strtok(buff, "|");
        if (p == NULL) {
            strcpy(buff2, "Error: Formato inválido,");
            write(sock_conn, buff2, strlen(buff2));
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
        }
        else if (codigo == 2) { // Login
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
   			snprintf(notif_msg, sizeof(notif_msg), "LOGIN:%s", data->nombre); // Prefijo LOGIN
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
        }
        else if (codigo == 3) { // Crear partida
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
                const char* query = "SELECT m.match_id, mp1.player_name, mp2.player_name, m.winner_name "
                                   "FROM matches m "
                                   "JOIN match_players mp1 ON m.match_id = mp1.match_id AND mp1.result = 'win' "
                                   "JOIN match_players mp2 ON m.match_id = mp2.match_id AND mp2.result = 'lose'";
                
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
                                    row[0] ? row[0] : "NULL",
                                    row[1] ? row[1] : "NULL",
                                    row[2] ? row[2] : "NULL",
                                    row[3] ? row[3] : "NULL");
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
            write(sock_conn, buff2, strlen(buff2));
            break;
        }
        
        else {
            strcpy(buff2, "Error: Código no válido,");
        }

        if (write(sock_conn, buff2, strlen(buff2)) <= 0) {
            perror("Error al enviar respuesta");
            break;
        }
    }

    if (strlen(data->nombre) > 0) {
        pthread_mutex_lock(mutex);
        Eliminar(lista, data->nombre);
        
        char notif_msg[256];
        snprintf(notif_msg, sizeof(notif_msg), 
                 "Desconexión: %s", data->nombre);
        send_notification(notif_handler, notif_msg, 1);
        
        pthread_mutex_unlock(mutex);
    }
    
    close(sock_conn);
    free(data);
    printf("Hilo %ld finalizado\n", pthread_self());
    pthread_exit(NULL);
}

// Manejador de señales
void handle_signal(int sig) {
    shutdown_requested = 1;
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
    
    // Configurar opciones del socket
    int optval = 1;
    if (setsockopt(sock_listen, SOL_SOCKET, SO_REUSEADDR, &optval, sizeof(optval))) {
        perror("Error configurando SO_REUSEADDR");
        close(sock_listen);
        exit(EXIT_FAILURE);
    }
    
    int puerto = 50076;
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
    
    // Bucle principal del servidor
    while (!shutdown_requested) {
        fd_set readfds;
        FD_ZERO(&readfds);
        FD_SET(sock_listen, &readfds);
        
        struct timeval tv = {1, 0};
        int ready = select(sock_listen + 1, &readfds, NULL, NULL, &tv);
        
        if (ready < 0) {
            if (errno == EINTR) continue;
            perror("Error en select");
            break;
        }
        
        if (ready > 0 && FD_ISSET(sock_listen, &readfds)) {
            sock_conn = accept(sock_listen, NULL, NULL);
            if (sock_conn < 0) {
                perror("Error aceptando conexión");
                continue;
            }
            
            printf("Nueva conexión aceptada\n");
            
            ThreadData *data = malloc(sizeof(ThreadData));
            *data->socket = sock_conn;
            data->lista = &miLista;
            data->mutex = &mutex;
            data->notif_handler = &notif_handler;
            data->nombre[0] = '\0';
            
            pthread_t thread;
            if (pthread_create(&thread, NULL, handle_client, data) != 0) {
                perror("Error creando hilo para cliente");
                free(data);
                close(sock_conn);
            }
            
            pthread_detach(thread);
        }
    }
    
    // Limpieza al finalizar
    printf("Cerrando servidor...\n");
    
    // Cerrar pipe de notificaciones
    close(notif_handler.notification_pipe[0]);
    close(notif_handler.notification_pipe[1]);
    
    // Esperar a que el thread de notificaciones termine
    pthread_join(notif_handler.thread_id, NULL);
    
    // Cerrar socket y base de datos
    close(sock_listen);
    mysql_close(conn);
    
    printf("Servidor cerrado correctamente\n");
    return 0;
}
