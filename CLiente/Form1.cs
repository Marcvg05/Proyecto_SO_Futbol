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

namespace Cliente
{
    public partial class Form1 : Form
    {
        Socket server;
        private Thread receiveThread;
        private bool isReceiving = true;
        private DataTable connectedPlayersTable = new DataTable();
        private DataTable notificationsTable = new DataTable();


        public Form1()
        {
            InitializeComponent();
            InitializePlayersTable();
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            lblBienvenida.Visible = false;
            TITULO.Visible = false;
            button1.Visible = false;
            MostrarBienvenida();

        }

        private void MostrarBienvenida()
        {
            float scaleX = (float)this.Width / 1920;
            float scaleY = (float)this.Height / 1080;

            lblBienvenida.Location = new Point(
                (int)(377 * scaleX),
                (int)(308 * scaleY));

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
                IPEndPoint ipep = new IPEndPoint(direc, 50076);

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

            if (string.IsNullOrWhiteSpace(MiUsuario.Text) || string.IsNullOrWhiteSpace(MiPassword.Text))
            {
                MessageBox.Show("Usuario y contraseña son obligatorios");
                return;
            }

            try
            {
                string mensaje = $"1|{MiUsuario.Text.Trim()}|{MiPassword.Text.Trim()}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                if (respuesta.Contains("Registro exitoso"))
                {
                    MessageBox.Show("¡Registro completado con éxito!");
                    statusLabel.Text = "Usuario registrado - " + MiUsuario.Text;
                }
                else
                {
                    MessageBox.Show(respuesta.Split(',')[0]);
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
                AddNotification($"Jugador conectado: {MiUsuario.Text}");
                server.Send(msg);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private void button4_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
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
                tabla.Columns.Add("Jugador 1 (Ganador)");
                tabla.Columns.Add("Jugador 2 (Perdedor)");
                tabla.Columns.Add("Ganador Confirmado");

                string[] partidas = datosPartidas.Split('|');

                foreach (string partida in partidas)
                {
                    if (!string.IsNullOrEmpty(partida))
                    {
                        string[] campos = partida.Split(':');
                        if (campos.Length == 4)
                        {
                            tabla.Rows.Add(
                                campos[0],
                                campos[1],
                                campos[2],
                                campos[3]
                            );
                        }
                    }
                }

                Form form = new Form();
                form.Text = "Historial de Partidas (por nombres)";
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
                        byte[] buffer = new byte[1024];
                        int bytesRec = server.Receive(buffer);
                        if (bytesRec == 0) // Conexión cerrada por el servidor
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("El servidor cerró la conexión");
                            });
                            break;
                        }

                        string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();
                        respuesta = respuesta.Split('\0')[0];

                        this.Invoke((MethodInvoker)delegate
                        {
                            ProcessServerMessage(respuesta);
                        });
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

            try
            {
                if (message.StartsWith("UPDATE|"))
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
                else if (message.StartsWith("LOGIN:"))
                {
                    string playerName = message.Substring(6); // Elimina "LOGIN:"
                    AddNotification($"LOGIN:{playerName}");
                    if (playerName.Equals(MiUsuario.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        statusLabel.Text = "Sesión iniciada - " + MiUsuario.Text;
                    }
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
                else
                {
                    // Otros mensajes no reconocidos
                    Debug.WriteLine($"Mensaje no reconocido del servidor: {message}");
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
                // Mostrar el mensaje localmente primero
                //AddChatMessage($"Tú: {ChatBox.Text}");

                // Enviar al servidor (formato: "7|username|message")
                //string mensaje = $"7|{username}|{ChatBox.Text}";
                //byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                //server.Send(msg);

                ChatBox.Text = "";
                ChatBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar mensaje: {ex.Message}");
            }
        }
    }
}