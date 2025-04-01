#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <pthread.h>
#include <fcntl.h>       // Para fcntl(), F_GETFL, F_SETFL, O_NONBLOCK
#include <errno.h>       // Para errno, EAGAIN, EWOULDBLOCK
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

int register_player(MYSQL *conn, const char *username, const char *password) {
    char check_query[1024];
    snprintf(check_query, sizeof(check_query), 
             "SELECT username FROM players WHERE username = '%s'", username);
    
    if (mysql_query(conn, check_query)) {
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

    char insert_query[1024];
    snprintf(insert_query, sizeof(insert_query),
             "INSERT INTO players (username, password) VALUES ('%s', '%s')",
             username, password);

    if (mysql_query(conn, insert_query)) {
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

void login_player(const char *username) {
    char query[256];
    snprintf(query, sizeof(query), 
             "INSERT INTO match_logs (player_name, action) VALUES ('%s', 'login');", 
             username);
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
    result_buffer[0] = '\0';
    while ((row = mysql_fetch_row(result))) {
        strncat(result_buffer, row[0], buffer_size - strlen(result_buffer) - 1);
        strncat(result_buffer, ",", buffer_size - strlen(result_buffer) - 1);
    }
    mysql_free_result(result);
}

void get_all_matches(char *result_buffer, size_t buffer_size) {
    char query[] =
        "SELECT match_id, match_date, duration, "
        "IFNULL(winner_name, 'Sin ganador') AS winner "
        "FROM matches;";
    
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
    result_buffer[0] = '\0';
    
    while ((row = mysql_fetch_row(result))) {
        char match_info[256];
        snprintf(match_info, sizeof(match_info), "ID: %s, Fecha: %s, Duracion: %s min, Ganador: %s | ",
                 row[0], row[1], row[2], row[3]);
        
        strncat(result_buffer, match_info, buffer_size - strlen(result_buffer) - 1);
    }
    
    mysql_free_result(result);
}

typedef struct {
    char nombre[50];  
    int socket;
} Conectado;

typedef struct {
    Conectado conectados[100];
    int num;
} ListaConectados;

int NuevoJugador(ListaConectados *lista, const char *nombre, int socket) {
    if (lista->num == 100) {
        return -1;
    }
    
    for (int i = 0; i < lista->num; i++) {
        if (strcmp(lista->conectados[i].nombre, nombre) == 0) {
            return -2;
        }
    }
    
    strncpy(lista->conectados[lista->num].nombre, nombre, sizeof(lista->conectados[lista->num].nombre) - 1);
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
    // Inicializar el buffer
    conectados[0] = '\0';
    
    if (lista == NULL || lista->num <= 0) {
        strcpy(conectados, "0/"); // Formato cuando no hay jugadores
        return;
    }

    // Contar jugadores activos
    int activos = lista->num;
    char temp[300] = {0};

    // Formato: "numero/jugador1/jugador2"
    snprintf(conectados, 300, "%d", activos);
    
    for(int i = 0; i < lista->num; i++) {
        strncat(conectados, "/", 300 - strlen(conectados) - 1);
        strncat(conectados, lista->conectados[i].nombre, 300 - strlen(conectados) - 1);
    }
}

typedef struct {
    int socket;
    ListaConectados *lista;
    pthread_mutex_t *mutex;
} ThreadData;

void *handle_client(void *arg) {
    ThreadData *data = (ThreadData *)arg;
    int sock_conn = data->socket;
    ListaConectados *lista = data->lista;
    pthread_mutex_t *mutex = data->mutex;
    
    char buff[2048];
    char buff2[2048];
    char nombre[50] = "";
    int ret;

    // Configurar timeout de recepción (30 segundos)
    struct timeval tv;
    tv.tv_sec = 30;
    tv.tv_usec = 0;
    setsockopt(sock_conn, SOL_SOCKET, SO_RCVTIMEO, (const char*)&tv, sizeof(tv));

    // Establecer modo no bloqueante (opcional pero recomendado)
    int flags = fcntl(sock_conn, F_GETFL, 0);
    fcntl(sock_conn, F_SETFL, flags | O_NONBLOCK);

    printf("Nuevo cliente conectado. Hilo: %ld\n", pthread_self());


    while(1) {
        memset(buff, 0, sizeof(buff));
        memset(buff2, 0, sizeof(buff2));

        ret = read(sock_conn, buff, sizeof(buff) - 1);
        if (ret <= 0) {
            if (ret == 0) {
                printf("Cliente cerró conexión (hilo %ld)\n", pthread_self());
                break;
            } else if (errno == EAGAIN || errno == EWOULDBLOCK) {
                // Timeout normal, continuar esperando
                usleep(100000); // Esperar 100ms para evitar CPU alto
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

        // Guardar nombre si es login o registro
        if (codigo == 1 || codigo == 2) {
            if (p != NULL) {
                strncpy(nombre, p, sizeof(nombre) - 1);
                nombre[sizeof(nombre) - 1] = '\0';
            }
        }

        if (codigo == 1) { // Registro
            p = strtok(NULL, "|");
            char password[50];
            if (p != NULL) {
                strncpy(password, p, sizeof(password) - 1);
                password[sizeof(password) - 1] = '\0';
                
                int reg_status = register_player(conn, nombre, password);
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
                
                int login_status = check_login_credentials(conn, nombre, password);
                if (login_status == 1) {
                    login_player(nombre);
                    strcpy(buff2, "Login exitoso,");
                    
                    pthread_mutex_lock(mutex);
                    int result = NuevoJugador(lista, nombre, sock_conn);
                    pthread_mutex_unlock(mutex);
                    
                    if (result == -1) {
                        strcat(buff2, " Error: Lista llena,");
                    } else if (result == -2) {
                        strcat(buff2, " Error: Ya conectado,");
                    }
                } else if (login_status == 0) {
                    strcpy(buff2, "Error: Usuario o contraseña incorrectos,");
                } else {
                    strcpy(buff2, "Error: Fallo en login,");
                }
            } else {
                strcpy(buff2, "Error: Datos incompletos,");
            }
        }
        else if (codigo >= 3 && codigo <= 5) { // Operaciones que requieren login
            pthread_mutex_lock(mutex);
            int pos = DamePosicion(lista, nombre);
            pthread_mutex_unlock(mutex);
            
            if (pos == -1) {
                strcpy(buff2, "Error: No autenticado,");
            } else {
                if (codigo == 3) {
                    query_players(buff2, sizeof(buff2));
                } 
                else if (codigo == 4) {
                    char misConectados[300];
                    pthread_mutex_lock(mutex);
                    DameConectados(lista, misConectados);
                    pthread_mutex_unlock(mutex);
                    write(sock_conn, misConectados, strlen(misConectados));
                    continue; // Saltar el write final
                } 
                else if (codigo == 5) {
                    get_all_matches(buff2, sizeof(buff2));
                    write(sock_conn, buff2, strlen(buff2));
                    continue; // Saltar el write final
                }
            }
        }
        else if (codigo == 6) { // Logout
            pthread_mutex_lock(mutex);
            Eliminar(lista, nombre);
            pthread_mutex_unlock(mutex);
            strcpy(buff2, "Sesión cerrada correctamente,");
            write(sock_conn, buff2, strlen(buff2));
            break; // Salir del bucle while(1)
        }
        else {
            strcpy(buff2, "Error: Código no válido,");
        }

        // Enviar respuesta (excepto para casos 4 y 5 que ya enviaron)
        if (write(sock_conn, buff2, strlen(buff2)) <= 0) {
            perror("Error al enviar respuesta");
            break;
        }
    }

    // Limpieza final
    if (strlen(nombre) > 0) {
        pthread_mutex_lock(mutex);
        Eliminar(lista, nombre);
        pthread_mutex_unlock(mutex);
    }
    close(sock_conn);
    free(data);
    printf("Hilo %ld finalizado\n", pthread_self());
    pthread_exit(NULL);
}

int main(int argc, char *argv[]) {
    ListaConectados miLista;
    miLista.num = 0;
    int sock_conn, sock_listen;
    struct sockaddr_in serv_adr;
    pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
    
    connect_db();
    
    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
        printf("Error creant socket");
        exit(1);
    }
    
    memset(&serv_adr, 0, sizeof(serv_adr));
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
    serv_adr.sin_port = htons(9050);
    
    if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0) {
        printf("Error al bind");
        exit(1);
    }
    
    if (listen(sock_listen, 5) < 0) {
        printf("Error en el Listen");
        exit(1);
    }
    
    while (1) {
        printf("Escuchando\n");
        sock_conn = accept(sock_listen, NULL, NULL);
        if (sock_conn < 0) {
            printf("Error aceptando conexión");
            continue;
        }
        printf("He recibido conexión\n");
        
        pthread_t thread;
        ThreadData *data = (ThreadData *)malloc(sizeof(ThreadData));
        data->socket = sock_conn;
        data->lista = &miLista;
        data->mutex = &mutex;
        
        if (pthread_create(&thread, NULL, handle_client, (void *)data) != 0) {
            printf("Error al crear el hilo\n");
            free(data);
            close(sock_conn);
        }
    }
    
    mysql_close(conn);
    return 0;
}
