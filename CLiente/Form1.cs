using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Cliente
{
    public partial class Form1 : Form
    {

        Socket server;
        private Thread receiveThread;
        private bool isReceiving = true;
        private DataTable connectedPlayersTable = new DataTable();
        private DataTable notificationsTable = new DataTable();
        private Image[] dadoImages;
        private int rondaActual = 1;
        private string ganadorPartidaNombre = null;
        private string ganadorPartidaPuntos = null;
        private bool finalizarPulsado = false;
        
        public Form1()
        {
            InitializeComponent();
            InitializePlayersTable();
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            lblBienvenida.Visible = false;
            TITULO.Visible = false;
            button1.Visible = false;
            GanadorPartida.Visible = false;
            finalizarPulsado = false;
            SALIR.Visible = false;
            DarseBaja.Visible = false;
            JugadoresConocidos.Visible = false;
            button7.Visible = false;
            textBox1.Visible = false;
            DiaMesAño.Visible = false;
            button8.Visible = false;
            MostrarBienvenida();
            CargarImagenesDado();

        }


        private void MostrarBienvenida()
        {
            float scaleX = (float)this.Width / 1920;
            float scaleY = (float)this.Height / 1080;

            lblBienvenida.Location = new Point(
                (int)(314 * scaleX),
                (int)(242 * scaleY));

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 4000;
            timer.Tick += (s, e) =>
            {
                lblBienvenida.Visible = false;
                timer.Stop();
                timer.Dispose();

                MostrarControlesSecundarios();
            };

            lblBienvenida.Visible = true;
            timer.Start();
        }
        private void MostrarControlesSecundarios()
        {
            TITULO.Visible = true;
            button1.Visible = true;

        }
        private void InitializePlayersTable()
        {
            connectedPlayersTable = new DataTable();
            connectedPlayersTable.Columns.Add("Historial", typeof(string));

            connectedPlayersDataGridView.DataSource = connectedPlayersTable;

            // Configuración visual mejorada
            connectedPlayersDataGridView.ColumnHeadersVisible = false;
            connectedPlayersDataGridView.RowHeadersVisible = false;
            connectedPlayersDataGridView.BackgroundColor = SystemColors.Control;
            connectedPlayersDataGridView.BorderStyle = BorderStyle.None;
            connectedPlayersDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            connectedPlayersDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            connectedPlayersDataGridView.Columns["Historial"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private DataTable chatTable = new DataTable();

        private void InitializeChatTable()
        {
            chatTable.Columns.Add("Mensaje", typeof(string));
            chatDataGridView.DataSource = chatTable;

            // Configuración visual
            chatDataGridView.Columns["Mensaje"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            chatDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AddNotification($"Jugador conectado: {MiUsuario.Text}");
            label1.Visible = false;
            label2.Visible = false;
            MiUsuario.Visible = false;
            MiPassword.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            connectedPlayersDataGridView.Visible = false;
            statusLabel.Visible = false;
            ChatBox.Visible = false;
            Enviar_Mensaje.Visible = false;
            chatDataGridView.Visible = false;
            rondaLabel.Visible = false;
            puntosLabel.Visible = false;
            dadoComboBox.Visible = false;
            Enviar.Visible = false;
            dadoPictureBox.Visible = false;
            SiguienteRonda.Visible = false;
            GanadorRonda.Visible = false;
            IniciarPartidaButton.Visible = false;
            DarseBaja.Visible = false;


        }

        // Método modificado para la nueva estructura
        private void AddNotification(string rawMessage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { AddNotification(rawMessage); });
                return;
            }

            string message;
            string playerName = rawMessage.Contains(":") ? rawMessage.Split(':')[1].Trim() : "Jugador";

            if (rawMessage.StartsWith("LOGIN:"))
            {
                message = $"{playerName} se ha conectado";
            }
            else if (rawMessage.StartsWith("LOGOUT:"))
            {
                message = $"{playerName} se ha desconectado";
            }
            else
            {
                return; // Ignorar otros mensajes
            }

            // Insertar al principio del DataTable
            DataRow newRow = connectedPlayersTable.NewRow();
            newRow["Historial"] = message;
            connectedPlayersTable.Rows.InsertAt(newRow, 0);

            // Limitar el historial a 20 entradas
            while (connectedPlayersTable.Rows.Count > 20)
            {
                connectedPlayersTable.Rows.RemoveAt(connectedPlayersTable.Rows.Count - 1);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {

            if (server != null && server.Connected)
            {
                MessageBox.Show("Ya estás conectado al servidor");
                return;
            }

            try
            {
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc, 50077);

                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.ReceiveTimeout = 5000;

                IAsyncResult result = server.BeginConnect(ipep, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                if (!success)
                {
                    server.Close();
                    throw new SocketException((int)SocketError.TimedOut);
                }

                server.EndConnect(result);
                MessageBox.Show("Conectado");
                TITULO.Visible = false;
                button1.Visible = false;
                label1.Visible = true;
                label2.Visible = true;
                MiUsuario.Visible = true;
                MiPassword.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button4.Visible = false;
                button5.Visible = false;
                connectedPlayersDataGridView.Visible = false;
                statusLabel.Visible = false;
                ChatBox.Visible = false;
                Enviar_Mensaje.Visible = false;
                this.ChatBox.AutoSize = false;
                this.ChatBox.Size = new System.Drawing.Size(207, 100);
                // Configuración visual
                chatDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                chatDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                chatDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                chatDataGridView.ReadOnly = true;
                chatDataGridView.ScrollBars = ScrollBars.Vertical;
                chatDataGridView.RowHeadersVisible = false;
                InitializeChatTable();   // El nuevo para el chat
                chatDataGridView.Visible = false;
                // Inicializar DataTable para el chat
                chatTable = new DataTable();
                chatTable.Columns.Add("Mensaje", typeof(string));
                chatDataGridView.DataSource = chatTable;

                StartReceivingThread();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
                server?.Close();
                server = null;
            }
        }

        private void button6_Click(object sender, EventArgs e) //Salir
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e) // Registrarse
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor");
                return;
            }

            string username = MiUsuario.Text.Trim();
            string password = MiPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Usuario y contraseña son obligatorios");
                return;
            }

            try
            {
                string mensaje = $"1|{username}|{password}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Esperar respuesta del servidor
                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                if (respuesta.Contains("Registro exitoso"))
                {
                    MessageBox.Show("¡Registro completado con éxito!");
                    statusLabel.Text = "Usuario registrado - " + username;
                }
                else if (respuesta.Contains("Usuario ya existe"))
                {
                    MessageBox.Show("Error: El usuario ya existe");
                }
                else
                {
                    MessageBox.Show("Error: Fallo en el registro");
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
                statusLabel.Text = "Error en registro";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e) // Iniciar Sesión
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor");
                return;
            }

            try
            {
                string mensaje = $"2|{MiUsuario.Text}|{MiPassword.Text}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                // NO HAGAS Receive aquí. La respuesta la procesará el hilo receptor.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                server.ReceiveTimeout = 10000;
                string mensaje = "4|";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                string[] partes = respuesta.Split('/');
                if (partes.Length > 0 && int.TryParse(partes[0], out int numJugadores))
                {
                    string lista = $"Jugadores conectados ({numJugadores}):\n";
                    for (int i = 1; i < partes.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(partes[i]))
                        {
                            lista += $"- {partes[i]}\n";
                        }
                    }
                    MessageBox.Show(lista);
                }
                else
                {
                    MessageBox.Show("Formato de respuesta inesperado del servidor");
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                MessageBox.Show("Timeout: El servidor no respondió a tiempo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                server.ReceiveTimeout = 30000;
                string mensaje = "5|";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[8192];
                int bytesReceived = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesReceived).Trim();

                MostrarHistorialPartidas(respuesta);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void MostrarHistorialPartidas(string datosPartidas)
        {
            try
            {
                DataTable tabla = new DataTable();
                tabla.Columns.Add("ID Partida");
                tabla.Columns.Add("Ganador");
                tabla.Columns.Add("Perdedor 1");
                tabla.Columns.Add("Perdedor 2");

                string[] partidas = datosPartidas.Split('|');

                foreach (string partida in partidas)
                {
                    if (!string.IsNullOrEmpty(partida))
                    {
                        string[] campos = partida.Split(':');
                        if (campos.Length == 4)
                        {
                            tabla.Rows.Add(
                                campos[0], // ID
                                campos[1], // Ganador
                                campos[2], // Perdedor 1
                                campos[3]  // Perdedor 2
                            );
                        }
                    }
                }

                Form form = new Form();
                form.Text = "Historial de Partidas";
                form.Width = 600;
                form.Height = 400;

                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.DataSource = tabla;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.ReadOnly = true;

                form.Controls.Add(dgv);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar historial: {ex.Message}");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isReceiving = false;
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Join(1000);
            }

            if (server != null && server.Connected)
            {
                try
                {
                    string logoutMsg = "6";
                    byte[] msg = Encoding.ASCII.GetBytes(logoutMsg);
                    server.Send(msg);
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
                catch { }
            }
        }

        private void StartReceivingThread()
        {
            isReceiving = true;
            receiveThread = new Thread(() =>
            {
                while (isReceiving && server != null && server.Connected)
                {
                    try
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRec = server.Receive(buffer);
                        if (bytesRec == 0) // Conexión cerrada por el servidor
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("El servidor cerró la conexión");
                            });
                            break;
                        }

                        string respuestaCompleta = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                        string[] mensajes = respuestaCompleta.Split('\0');
                        Debug.WriteLine($"[DEBUG RAW] Recibido: {respuestaCompleta.Replace("\0", "|END|")}");

                        foreach (string mensaje in mensajes)
                        {
                            string msg = mensaje.Trim();
                            if (string.IsNullOrEmpty(msg)) continue;

                            // Intentamos separar manualmente si hay dos mensajes pegados
                            if (msg.Contains("RESULTADO|") && !msg.StartsWith("RESULTADO|"))
                            {
                                int idx = msg.IndexOf("RESULTADO|");

                                string previo = msg.Substring(0, idx).Trim();
                                string resultado = msg.Substring(idx).Trim();

                                if (!string.IsNullOrEmpty(previo))
                                {
                                    this.Invoke((MethodInvoker)(() => ProcessServerMessage(previo)));
                                }

                                if (!string.IsNullOrEmpty(resultado))
                                {
                                    this.Invoke((MethodInvoker)(() => ProcessServerMessage(resultado)));
                                }
                            }
                            else
                            {
                                this.Invoke((MethodInvoker)(() => ProcessServerMessage(msg)));
                            }
                        }
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        if (isReceiving)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show($"Error en recepción: {ex.Message}");
                                if (!server.Connected)
                                {
                                    statusLabel.Text = "Desconectado";
                                }
                            });
                        }
                        break;
                    }
                }
            });
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ProcessServerMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            message = message.Trim().TrimEnd(',');

            try
            {
                Debug.WriteLine($"Recibido mensaje: {message}");
                if (message.StartsWith("Error: Credenciales inválidas"))
                {
                    MessageBox.Show("Error al iniciar sesión. Inténtelo de nuevo o regístrese antes.", "Inicio de sesión fallido");
                    AddNotification("Error al iniciar sesión. Inténtelo de nuevo o regístrese antes.");
                    MiPassword.Text = "";
                    return;
                }
                if (message.Contains("Se le ha desconectado y eliminado su usuario."))
                {
                    MessageBox.Show("Se le ha desconectado y eliminado su usuario.");

                    // Mostrar solo login y registro
                    TITULO.Visible = true;
                    button1.Visible = true;
                    label1.Visible = true;
                    label2.Visible = true;
                    MiUsuario.Visible = true;
                    MiPassword.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;

                    // Ocultar todo lo demás
                    button4.Visible = false;
                    button5.Visible = false;
                    connectedPlayersDataGridView.Visible = false;
                    statusLabel.Visible = false;
                    ChatBox.Visible = false;
                    Enviar_Mensaje.Visible = false;
                    chatDataGridView.Visible = false;
                    CrearPartidaButton.Visible = false;
                    InvitarJugadorButton.Visible = false;
                    InvitarJugadorTextBox.Visible = false;
                    MostrarJugadoresButton.Visible = false;
                    DarseBaja.Visible = false;
                    rondaLabel.Visible = false;
                    puntosLabel.Visible = false;
                    dadoComboBox.Visible = false;
                    Enviar.Visible = false;
                    dadoPictureBox.Visible = false;
                    SiguienteRonda.Visible = false;
                    GanadorRonda.Visible = false;
                    IniciarPartidaButton.Visible = false;
                    GanadorPartida.Visible = false;
                    SALIR.Visible = false;

                    // Limpia usuario y contraseña
                    MiUsuario.Text = "";
                    MiPassword.Text = "";
                    finalizarPulsado = false;
                    ganadorPartidaNombre = null;
                    ganadorPartidaPuntos = null;
                    rondaActual = 1;
                    // Si quieres, puedes cerrar y reabrir la conexión aquí si el socket está cerrado
                    return;
                }
                else if (message.StartsWith("Login exitoso"))
                {
                    MessageBox.Show("Login exitoso.", "Sesión iniciada");

                    AddNotification($"Jugador conectado: {MiUsuario.Text}");
                    TITULO.Visible = false;
                    button1.Visible = false;
                    label1.Visible = false;
                    label2.Visible = false;
                    MiUsuario.Visible = false;
                    MiPassword.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = true;
                    button5.Visible = true;
                    connectedPlayersDataGridView.Visible = true;
                    Enviar_Mensaje.Visible = true;
                    ChatBox.Visible = true;
                    chatDataGridView.Visible = true;
                    CrearPartidaButton.Visible = true;
                    InvitarJugadorButton.Visible = true;
                    InvitarJugadorTextBox.Visible = true;
                    MostrarJugadoresButton.Visible = true;
                    DarseBaja.Visible = true;
                    JugadoresConocidos.Visible = true;
                    button7.Visible = true;
                    textBox1.Visible = true;
                    DiaMesAño.Visible = true;
                    button8.Visible = true;



                }
                else if (message.StartsWith("LOGIN:"))
                {
                    string playerName = message.Substring(6); // Elimina "LOGIN:"
                    AddNotification($"LOGIN:{playerName}");
                    if (playerName.Equals(MiUsuario.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        statusLabel.Text = "Sesión iniciada - " + MiUsuario.Text;
                    }
                }
                else if (message.StartsWith("CONOCIDOS|"))
                {
                    string conocidos = message.Substring("CONOCIDOS|".Length);
                    MostrarJugadoresConocidos(conocidos);
                }
                else if (message.StartsWith("CHAT|")) // Mensajes de chat
                {
                    string chatMessage = message.Substring(5); // Elimina "CHAT|"

                    // Extrae el nombre del jugador
                    int sep = chatMessage.IndexOf(':');
                    if (sep > 0)
                    {
                        string sender = chatMessage.Substring(0, sep).Trim();
                        string contenido = chatMessage.Substring(sep + 1).Trim();

                        // Evita mostrar el mensaje dos veces si viene de uno mismo
                        if (!sender.Equals(MiUsuario.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            AddChatMessage($"{sender}: {contenido}");
                        }
                    }
                    else
                    {
                        // Si por alguna razón no hay ':', lo mostramos tal cual
                        AddChatMessage(chatMessage);
                    }
                }
                else if (message.StartsWith("UPDATE|"))
                {
                    // Formato esperado: "UPDATE|num_jugadores/jugador1/jugador2/..."
                    string[] parts = message.Split('|');

                    if (parts.Length >= 2)
                    {
                        string[] playersInfo = parts[1].Split('/');

                        if (playersInfo.Length > 0 && int.TryParse(playersInfo[0], out int playerCount))
                        {
                            // Aquí puedes actualizar tu UI con la lista de jugadores
                            UpdateConnectedPlayersList(playersInfo.Skip(1).ToArray());
                        }
                    }
                }

                else if (message.StartsWith("PARTIDA_INICIADA"))
                {
                    // Muestra controles de partida en Form1

                    rondaLabel.Visible = true;
                    puntosLabel.Visible = true;
                    dadoComboBox.Visible = true;
                    Enviar.Visible = true;
                    dadoPictureBox.Visible = false;
                    SiguienteRonda.Visible = true;
                    GanadorRonda.Visible = true;
                    IniciarRonda(1); // O espera a RONDA|1
                }
                else if (message.StartsWith("LOGOUT:"))
                {
                    string playerName = message.Substring(7); // Elimina "LOGOUT:"
                    AddNotification($"LOGOUT:{playerName}");
                }
                else if (message.Contains("exitoso"))
                {
                    // Mensajes de éxito del servidor
                    string cleanMessage = message.Split(',')[0];
                    MessageBox.Show(cleanMessage);
                }
                else if (message.StartsWith("INVITACION|"))
                {
                    string invitacion = message.Substring(11); // Elimina "INVITACION|"
                    DialogResult result = MessageBox.Show(invitacion, "Invitación", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        // Extraer el nombre del creador de la partida
                        string creador = invitacion.Split(' ')[0]; // Asume que el mensaje tiene formato "Creador te ha invitado..."
                        try
                        {
                            string mensaje = $"11|{creador}|{MiUsuario.Text}"; // Código 11 para unirse a una partida
                            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);

                            MessageBox.Show("Te has unido a la partida");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al unirse a la partida: {ex.Message}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Has rechazado la invitación");
                    }
                }
                else if (message.StartsWith("UNIRSE|"))
                {
                    // Nuevo código para unirse a una partida
                    string[] partes = message.Split('|');
                    if (partes.Length == 2)
                    {
                        string creador = partes[1];

                        // Mostrar mensaje de éxito al unirse a la partida
                        MessageBox.Show($"Te has unido a la partida de {creador}");
                    }
                }
                else if (message.StartsWith("RONDA|"))
                {
                    Debug.WriteLine($"[{MiUsuario.Text}] Ronda recibida: {message}");
                    int ronda = 1;
                    if (int.TryParse(message.Split('|')[1], out ronda))
                    {
                        IniciarRonda(ronda);
                    }
                }
                else if (message.StartsWith("RESULTADO|"))
                {
                    string[] parts = message.Split('|');
                    int numeroDado = int.Parse(parts[3]);
                    string jugador1 = parts[5];
                    string jugador2 = parts[6];
                    string jugador3 = parts[7];
                    int puntosJugador1 = int.Parse(parts[9]);
                    int puntosJugador2 = int.Parse(parts[10]);
                    int puntosJugador3 = int.Parse(parts[11]);
                    string ganadorRonda = "";
                    if (parts.Length >= 14 && parts[12] == "GANADOR_RONDA")
                        ganadorRonda = parts[13];
                    //Debug.WriteLine("Recibido RESULTADO para: " + MiUsuario.Text + " -> " + message);
                    //MessageBox.Show($"RESULTADO recibido en {MiUsuario.Text}", "DEBUG");

                    // Llama directamente al método de Form1
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            MostrarResultado(
                                numeroDado,
                                puntosJugador1,
                                puntosJugador2,
                                puntosJugador3,
                                ganadorRonda,
                                jugador1,
                                jugador2,
                                jugador3
                            );
                        }));
                    }
                    else
                    {
                        Debug.WriteLine("⚠ Form1 no está listo para mostrar resultado");
                    }
                }
                else if (message.StartsWith("FIN|GANADOR|"))
                {
                    string[] parts = message.Split('|');
                    ganadorPartidaNombre = parts[2];
                    ganadorPartidaPuntos = parts[4];
                    if (finalizarPulsado)
                    {
                        GanadorPartida.Visible = true;
                        GanadorPartida.Text = $"EL GANADOR DE LA PARTIDA HA SIDO {ganadorPartidaNombre}!!!";
                        SALIR.Visible = true;
                    }
                }
                else if (message.StartsWith("FIN|PERDEDOR|"))
                {
                    string[] parts = message.Split('|');
                    ganadorPartidaNombre = parts[2];
                    ganadorPartidaPuntos = parts[4];
                    if (finalizarPulsado)
                    {
                        GanadorPartida.Visible = true;
                        GanadorPartida.Text = $"EL GANADOR DE LA PARTIDA HA SIDO {ganadorPartidaNombre}!!!";
                        SALIR.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error procesando mensaje: {ex.Message}");
                // Opcional: Mostrar mensaje al usuario
                MessageBox.Show($"Error al procesar respuesta del servidor: {message}");
            }
        }

        private void UpdateConnectedPlayersList(string[] playerNames)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateConnectedPlayersList(playerNames); });
                return;
            }

            // Limpiar la lista actual
            connectedPlayersTable.Rows.Clear();

            // Agregar nuevos jugadores
            foreach (string player in playerNames)
            {
                if (!string.IsNullOrWhiteSpace(player))
                {
                    DataRow newRow = connectedPlayersTable.NewRow();
                    newRow["Historial"] = $"{player} conectado";
                    connectedPlayersTable.Rows.Add(newRow);
                }
            }
        }

        private void statusLabel_Click(object sender, EventArgs e) { }

        private void connectedPlayersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void Enviar_Mensaje_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor");
                return;
            }

            if (string.IsNullOrWhiteSpace(ChatBox.Text))
            {
                MessageBox.Show("Escribe un mensaje primero");
                return;
            }

            try
            {
                string mensaje = $"7|{MiUsuario.Text}|{ChatBox.Text}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Mostrar el mensaje localmente
                AddChatMessage($"Tú: {ChatBox.Text}");

                ChatBox.Text = "";
                ChatBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar mensaje: {ex.Message}");
            }
        }
        private void AddChatMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { AddChatMessage(message); });
                return;
            }

            DataRow newRow = chatTable.NewRow();
            newRow["Mensaje"] = message;
            chatTable.Rows.Add(newRow);

            // Desplazar automáticamente al final
            chatDataGridView.FirstDisplayedScrollingRowIndex = chatDataGridView.RowCount - 1;
        }

        private void CrearPartidaButton_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor");
                return;
            }

            try
            {
                string mensaje = $"8|{MiUsuario.Text}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                MessageBox.Show(respuesta);

                // Mostrar el botón de "Iniciar Partida" si la partida se creó correctamente
                if (respuesta.StartsWith("Partida creada"))
                {
                    IniciarPartidaButton.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear partida: {ex.Message}");
            }
        }

        private void InvitarJugadorButton_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor");
                return;
            }

            string invitado = InvitarJugadorTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(invitado))
            {
                MessageBox.Show("Escribe el nombre del jugador a invitar");
                return;
            }

            try
            {
                string mensaje = $"9|{invitado}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                MessageBox.Show(respuesta);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al invitar jugador: {ex.Message}");
            }
        }

        private void MostrarJugadoresButton_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor");
                return;
            }

            try
            {
                string mensaje = "10|";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                MessageBox.Show(respuesta);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener jugadores: {ex.Message}");
            }
        }
        private void TITULO_Click(object sender, EventArgs e)
        {

        }

        private void lblBienvenida_Click(object sender, EventArgs e)
        {

        }

        private void IniciarPartidaButton_Click_1(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor");
                return;
            }

            try
            {
                string mensaje = $"12|{MiUsuario.Text}"; // Código 12 para iniciar partida
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();
                // Solo mostrar la primera línea o hasta el primer '|'
                string mensajeMostrar = respuesta.Split(new[] { '|', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0];
                MessageBox.Show(mensajeMostrar);

                // Eliminar o comentar estas líneas:
                // if (respuesta.StartsWith("Partida iniciada"))
                // {
                //     formPartida = new FormPartida(server);
                //     formPartida.Show();
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar partida: {ex.Message}");
            }
        }
        private void MostrarGanadorFinal(string nombre, string puntos, bool soyYo)
        {
            SiguienteRonda.Visible = false;
            GanadorPartida.Visible = true;
            GanadorPartida.Text = $"EL GANADOR DE LA PARTIDA HA SIDO {nombre} CON {puntos} PUNTOS.";
        }

        private void MostrarPerdedorFinal(string nombre, string puntos)
        {
            SiguienteRonda.Enabled = false;
            SiguienteRonda.Visible = false;
            GanadorPartida.Visible = true;
            GanadorPartida.Text = $"EL GANADOR DE LA PARTIDA HA SIDO {nombre} CON {puntos} PUNTOS.";
        }
        private void IniciarRonda(int ronda)
        {
            Debug.WriteLine($"[{MiUsuario.Text}] Iniciando ronda {ronda}");

            rondaActual = ronda;
            rondaLabel.Text = $"RONDA {ronda}";
            rondaLabel.Visible = true;

            dadoComboBox.SelectedIndex = -1;
            dadoComboBox.Visible = true;
            dadoComboBox.Enabled = true;

            Enviar.Enabled = true;
            Enviar.Visible = true;

            SiguienteRonda.Enabled = false;
            SiguienteRonda.Visible = false;

            dadoPictureBox.Visible = false;
            GanadorRonda.Visible = false;

            GanadorPartida.Visible = false;
            finalizarPulsado = false;
        }
        private void MostrarResultado(int numeroDado, int puntos1, int puntos2, int puntos3, string ganadorRonda, string jugador1, string jugador2, string jugador3)
        {
            puntosLabel.Text = $"Puntos: {GetMisPuntos(jugador1, jugador2, jugador3, puntos1, puntos2, puntos3)}";
            GanadorRonda.Text = $"Ganador ronda: {ganadorRonda}";
            GanadorRonda.Visible = true;
            dadoComboBox.Enabled = false;
            Enviar.Enabled = false;

            // Solo mostrar el botón si no es la última ronda
            if (rondaActual < 5)
            {
                SiguienteRonda.Text = "Siguiente Ronda";
            }
            else
            {
                SiguienteRonda.Text = "Finalizar";
            }
            SiguienteRonda.Enabled = true;
            SiguienteRonda.Visible = true;
            MostrarDado(numeroDado);
        }

        private int GetMisPuntos(string j1, string j2, string j3, int p1, int p2, int p3)
        {
            if (MiUsuario.Text.Equals(j1, StringComparison.OrdinalIgnoreCase)) return p1;
            if (MiUsuario.Text.Equals(j2, StringComparison.OrdinalIgnoreCase)) return p2;
            if (MiUsuario.Text.Equals(j3, StringComparison.OrdinalIgnoreCase)) return p3;
            return 0;
        }

        private void Enviar_Click(object sender, EventArgs e)
        {
            if (dadoComboBox.SelectedItem == null) return;
            string apuesta = dadoComboBox.SelectedItem.ToString();
            string mensaje = $"13|{MiUsuario.Text}|{apuesta}";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            Enviar.Enabled = false;
        }

        private void SiguienteRonda_Click(object sender, EventArgs e)
        {
            string mensaje = $"14|{MiUsuario.Text}";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            SiguienteRonda.Enabled = false;
        }
        private void CargarImagenesDado()
        {
            dadoImages = new Image[6];
            for (int i = 0; i < 6; i++)
            {
                string ruta = Path.Combine(Application.StartupPath, "Images", $"dado{i + 1}.jpg");
                dadoImages[i] = Image.FromFile(ruta);

            }
        }

        // Para mostrar la imagen del dado (por ejemplo, cara 4):
        private void MostrarDado(int numero)
        {
            if (numero >= 1 && numero <= 6)
                dadoPictureBox.Image = dadoImages[numero - 1];
                dadoPictureBox.Visible = true;
        }

        private void Enviar_Click_1(object sender, EventArgs e)
        {
            if (dadoComboBox.SelectedItem == null) return;
            string apuesta = dadoComboBox.SelectedItem.ToString();
            Enviar.Enabled = false;
            dadoComboBox.Enabled = false; // Deshabilita el ComboBox justo al enviar
            string mensaje = $"13|{MiUsuario.Text}|{apuesta}";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void SiguienteRonda_Click_1(object sender, EventArgs e)
        {
            try
            {
                string mensaje = $"14|{MiUsuario.Text}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                Debug.WriteLine($"[{MiUsuario.Text}] Enviada confirmación SIGUIENTE_RONDA");

                if (rondaActual == 5)
                {
                    finalizarPulsado = true;
                    SiguienteRonda.Enabled = false;
                    // Si ya tenemos los datos del ganador, mostramos la label
                    if (ganadorPartidaNombre != null && ganadorPartidaPuntos != null)
                    {
                        GanadorPartida.Visible = true;
                        GanadorPartida.Text = $"EL GANADOR DE LA PARTIDA HA SIDO {ganadorPartidaNombre}!!!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar confirmación de siguiente ronda: " + ex.Message);
            }
        }
        private void GanadorPartida_Click(object sender, EventArgs e)
        {

        }

        private void dadoPictureBox_Click(object sender, EventArgs e)
        {

        }

        private void SALIR_Click_1(object sender, EventArgs e)
        {
            // Oculta controles de partida
            GanadorPartida.Visible = false;
            GanadorRonda.Visible = false;
            rondaLabel.Visible = false;
            puntosLabel.Visible = false;
            dadoComboBox.Visible = false;
            Enviar.Visible = false;
            dadoPictureBox.Visible = false;
            SiguienteRonda.Visible = false;
            SALIR.Visible = false;

            // Notifica al servidor que este jugador ha salido de la partida
            string mensaje = $"15|{MiUsuario.Text}";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void DarseBaja_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor");
                return;
            }

            string mensaje = $"16|{MiUsuario.Text}";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void JugadoresConocidos_Click(object sender, EventArgs e)
        {
            try
            {
                string mensaje = "17|";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                // El hilo receptor se encargará del resto
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private void MostrarJugadoresConocidos(string datos)
        {
            try
            {
                DataTable tabla = new DataTable();
                tabla.Columns.Add("Jugadores conocidos");

                if (string.IsNullOrWhiteSpace(datos) || datos.StartsWith("Error"))
                {
                    tabla.Rows.Add("No hay jugadores conocidos");
                }
                else
                {
                    string[] jugadores = datos.Split('|');
                    bool hayJugadores = false;

                    foreach (string j in jugadores)
                    {
                        if (!string.IsNullOrWhiteSpace(j))
                        {
                            tabla.Rows.Add(j);
                            hayJugadores = true;
                        }
                    }

                    if (!hayJugadores)
                        tabla.Rows.Add("No hay jugadores conocidos");
                }

                Form form = new Form();
                form.Text = "Jugadores Conocidos";
                form.Width = 400;
                form.Height = 300;

                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.DataSource = tabla;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.ReadOnly = true;

                form.Controls.Add(dgv);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar jugadores: {ex.Message}");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estÃ¡s conectado al servidor");
                return;
            }

            string otroJugador = textBox1.Text.Trim();
            string miJugador = MiUsuario.Text.Trim();

            if (string.IsNullOrWhiteSpace(otroJugador))
            {
                MessageBox.Show("Introduce el nombre del jugador en el TextBox");
                return;
            }

            try
            {
                // Protocolo 18 para historial entre dos jugadores
                string mensaje = $"18|{miJugador}|{otroJugador}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Recibe la respuesta del servidor
                byte[] buffer = new byte[4096];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                if (string.IsNullOrWhiteSpace(respuesta) || respuesta == "NO_HAY_PARTIDAS")
                {
                    MessageBox.Show("No hay partidas entre estos jugadores.");
                    return;
                }

                // Muestra el historial en un DataGridView en una ventana nueva
                DataTable tabla = new DataTable();
                tabla.Columns.Add("ID Partida");
                tabla.Columns.Add("Ganador");
                tabla.Columns.Add("Perdedores");

                string[] partidas = respuesta.Split('|');
                foreach (string partida in partidas)
                {
                    if (!string.IsNullOrEmpty(partida))
                    {
                        string[] campos = partida.Split(':');
                        if (campos.Length == 3)
                        {
                            tabla.Rows.Add(campos[0], campos[1], campos[2]);
                        }
                    }
                }

                Form form = new Form();
                form.Text = $"Historial entre {miJugador} y {otroJugador}";
                form.Width = 600;
                form.Height = 400;

                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.DataSource = tabla;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.ReadOnly = true;

                form.Controls.Add(dgv);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener historial: {ex.Message}");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor");
                return;
            }

            string fecha = DiaMesAño.Text.Trim();
            if (string.IsNullOrWhiteSpace(fecha))
            {
                MessageBox.Show("Introduce una fecha en formato dd/mm/aaaa");
                return;
            }

            // Validar formato dd/mm/aa
            if (!System.Text.RegularExpressions.Regex.IsMatch(fecha, @"^\d{2}/\d{2}/\d{4}$"))
            {
                MessageBox.Show("Introduce la fecha en formato dd/mm/aa (por ejemplo, 01/01/2025)");
                return;
            }

            try
            {
                string mensaje = $"19|{fecha}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[4096];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                if (respuesta == "NO_HAY_PARTIDAS")
                {
                    MessageBox.Show("No hay partidas jugadas en esa fecha.");
                    return;
                }
                if (respuesta.StartsWith("Error"))
                {
                    MessageBox.Show(respuesta);
                    return;
                }

                // Mostrar en un DataGridView
                DataTable tabla = new DataTable();
                tabla.Columns.Add("ID Partida");
                tabla.Columns.Add("Fecha");
                tabla.Columns.Add("Ganador");
                tabla.Columns.Add("Perdedores");

                string[] partidas = respuesta.Split('|');
                foreach (string partida in partidas)
                {
                    if (!string.IsNullOrEmpty(partida))
                    {
                        string[] campos = partida.Split(':');
                        if (campos.Length == 4)
                        {
                            tabla.Rows.Add(campos[0], campos[1], campos[2], campos[3]);
                        }
                    }
                }

                Form form = new Form();
                form.Text = $"Partidas jugadas el {fecha}";
                form.Width = 700;
                form.Height = 400;

                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.DataSource = tabla;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.ReadOnly = true;

                form.Controls.Add(dgv);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}