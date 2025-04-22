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

        public Form1()
        {
            InitializeComponent();
            InitializePlayersTable();
            this.FormClosing += Form1_FormClosing;
        }

        private void InitializePlayersTable()
        {
            connectedPlayersTable = new DataTable();
            connectedPlayersTable.Columns.Add("Historial", typeof(string));
            connectedPlayersTable.Columns.Add("Tipo", typeof(string));

            connectedPlayersDataGridView.DataSource = connectedPlayersTable;

            connectedPlayersDataGridView.ColumnHeadersVisible = false;
            connectedPlayersDataGridView.RowHeadersVisible = false;
            connectedPlayersDataGridView.BackgroundColor = SystemColors.Control;
            connectedPlayersDataGridView.BorderStyle = BorderStyle.None;
            connectedPlayersDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            connectedPlayersDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            connectedPlayersDataGridView.Columns["Tipo"].Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AddNotification($"Bienvenido al cliente de juego", "Sistema");
        }

        public void AddNotification(string message, string tipo)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { AddNotification(message, tipo); });
                return;
            }

            DataRow newRow = connectedPlayersTable.NewRow();
            newRow["Historial"] = message;
            newRow["Tipo"] = tipo;
            connectedPlayersTable.Rows.InsertAt(newRow, 0);

            while (connectedPlayersTable.Rows.Count > 50)
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

                StartReceivingThread();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
                server?.Close();
                server = null;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
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
                AddNotification($"Jugador conectado: {MiUsuario.Text}", "Sistema");
                server.Send(msg);
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
                        if (bytesRec == 0)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("El servidor cerró la conexión");
                                statusLabel.Text = "Desconectado";
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
                                statusLabel.Text = "Desconectado";
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
                    string[] parts = message.Split('|');
                    if (parts.Length >= 2)
                    {
                        string[] playersInfo = parts[1].Split('/');
                        if (playersInfo.Length > 0 && int.TryParse(playersInfo[0], out int playerCount))
                        {
                            UpdateConnectedPlayersList(playersInfo.Skip(1).ToArray());
                        }
                    }
                }
                else if (message.StartsWith("INVITACION_FORM|"))
                {
                    string[] partes = message.Split('|');
                    if (partes.Length >= 2)
                    {
                        int grupoIdx;
                        if (int.TryParse(partes[1], out grupoIdx))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                // Mostrar notificación genérica ya que no tenemos el nombre del invitador
                                AddNotification("Has recibido una invitación a un grupo", "Invitación");

                                using (var invitationForm = new InvitationForm(grupoIdx, server))
                                {
                                    invitationForm.ShowDialog(this);
                                }
                            });
                        }
                    }
                }
                else if (message.StartsWith("INVITACION_RECHAZADA|"))
                {
                    string[] parts = message.Split('|');
                    if (parts.Length >= 2)
                    {
                        int grupoIdx;
                        if (int.TryParse(parts[1], out grupoIdx))
                        {
                            AddNotification("Un jugador ha rechazado tu invitación al grupo", "Notificación");
                        }
                    }
                }
                else if (message.StartsWith("GRUPO|"))
                {
                    string[] partes = message.Split('|');
                    if (partes.Length >= 3)
                    {
                        AddNotification($"{partes[1]}: {partes[2]}", "Mensaje de grupo");
                    }
                }
                else if (message.StartsWith("GRUPO_CREADO:"))
                {
                    string creador = message.Substring(13);
                    AddNotification($"{creador} ha creado un grupo", "Notificación");
                }
                else if (message.StartsWith("LOGIN:"))
                {
                    string playerName = message.Substring(6);
                    AddNotification($"{playerName} se ha conectado", "Sistema");
                    if (playerName.Equals(MiUsuario.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        statusLabel.Text = "Sesión iniciada - " + MiUsuario.Text;
                    }
                }
                else if (message.StartsWith("LOGOUT:"))
                {
                    string playerName = message.Substring(7);
                    AddNotification($"{playerName} se ha desconectado", "Sistema");
                }
                else if (message.Contains("exitoso"))
                {
                    string cleanMessage = message.Split(',')[0];
                    MessageBox.Show(cleanMessage);
                }
                else if (message.StartsWith("INVITACION_RESULTADO|"))
                {
                    string[] parts = message.Split('|');
                    if (parts.Length >= 3)
                    {
                        string status = parts[1];
                        string content = parts[2];

                        if (status == "OK")
                        {
                            AddNotification(content, "Notificación");
                        }
                        else
                        {
                            MessageBox.Show($"Error al invitar: {content}");
                        }
                    }
                }
                else if (message.StartsWith("INVITACION_ACEPTADA|"))
                {
                    string invitado = message.Substring("INVITACION_ACEPTADA|".Length);
                    AddNotification($"{invitado} ha aceptado tu invitación", "Notificación");
                }
                else if (message.StartsWith("INVITACION_RECHAZADA|"))
                {
                    string invitado = message.Substring("INVITACION_RECHAZADA|".Length);
                    AddNotification($"{invitado} ha rechazado tu invitación", "Notificación");
                }
                else
                {
                    Debug.WriteLine($"Mensaje no reconocido del servidor: {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error procesando mensaje: {ex.Message}");
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

            connectedPlayersTable.Rows.Clear();
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



        private void btnSalirGrupo_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor");
                return;
            }

            try
            {
                string mensaje = "12|";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Esperar respuesta del servidor
                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                if (respuesta.Contains("Has salido del grupo"))
                {
                    AddNotification("Has salido del grupo", "Notificación");
                }
                else
                {
                    MessageBox.Show(respuesta.Split(',')[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al salir del grupo: {ex.Message}");
            }
        }

        private void btnCrearGrupo_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor");
                return;
            }

            try
            {
                string mensaje = "7|";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                AddNotification("Has creado un nuevo grupo", "Notificación");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear grupo: {ex.Message}");
            }
        }

        private void btnInvitar_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtInvitado.Text))
            {
                MessageBox.Show("Debes introducir un nombre de usuario");
                return;
            }

            try
            {
                string mensaje = $"8|{txtInvitado.Text.Trim()}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Ya no se hace Receive aquí. Esperamos la respuesta asincrónica.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al invitar: {ex.Message}");
            }
        }

        private void btnEnviarMensajeGrupo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMensajeGrupo.Text))
            {
                MessageBox.Show("El mensaje no puede estar vacío");
                return;
            }

            try
            {
                string mensaje = $"9|{txtMensajeGrupo.Text}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                AddNotification($"{MiUsuario.Text}: {txtMensajeGrupo.Text}", "Mensaje de grupo");
                txtMensajeGrupo.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar mensaje: {ex.Message}");
            }
        }

        private void statusLabel_Click(object sender, EventArgs e) { }

        private void connectedPlayersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void txtMensajeGrupo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}