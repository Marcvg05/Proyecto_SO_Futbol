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
            this.FormClosing += Form1_FormClosing;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            AddNotification($"Jugador conectado: {MiUsuario.Text}");
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

        private void button5_Click(object sender, EventArgs e) // Mostrar Jugadores Conectados
        {
            if (connectedPlayersTable.Rows.Count == 0)
            {
                MessageBox.Show("No hay jugadores conectados o no se ha recibido información actualizada");
                return;
            }

            connectedPlayersDataGridView.DataSource = connectedPlayersTable;
            connectedPlayersDataGridView.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e) // Conectar server
        {
            if (server != null && server.Connected)
            {
                MessageBox.Show("Ya estás conectado al servidor");
                return;
            }

            try
            {
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc, 50076); //50076-50080

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private void button4_Click(object sender, EventArgs e) // Mostrar historial partidas
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor");
                return;
            }

            try
            {
                string mensaje = "5";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] buffer = new byte[4096];
                int bytesReceived = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                MostrarHistorialPartidas(respuesta);
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void MostrarHistorialPartidas(string datosPartidas)
        {
            DataTable tablaPartidas = new DataTable();
            tablaPartidas.Columns.Add("ID Partida");
            tablaPartidas.Columns.Add("Fecha");
            tablaPartidas.Columns.Add("Duración (min)");
            tablaPartidas.Columns.Add("Ganador");

            string[] partidas = datosPartidas.Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string partida in partidas)
            {
                if (!string.IsNullOrWhiteSpace(partida))
                {
                    string[] partes = partida.Split(new[] { ", " }, StringSplitOptions.None);

                    if (partes.Length >= 4)
                    {
                        string id = partes[0].Replace("ID: ", "");
                        string fecha = partes[1].Replace("Fecha: ", "");
                        string duracion = partes[2].Replace("Duracion: ", "").Replace(" min", "");
                        string ganador = partes[3].Replace("Ganador: ", "");

                        tablaPartidas.Rows.Add(id, fecha, duracion, ganador);
                    }
                }
            }

            Form formHistorial = new Form();
            formHistorial.Text = "Historial de Partidas";
            formHistorial.Width = 600;
            formHistorial.Height = 400;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.DataSource = tablaPartidas;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;

            Button btnCerrar = new Button();
            btnCerrar.Text = "Cerrar";
            btnCerrar.Dock = DockStyle.Bottom;
            btnCerrar.Click += (s, ev) => formHistorial.Close();

            formHistorial.Controls.Add(dgv);
            formHistorial.Controls.Add(btnCerrar);
            formHistorial.ShowDialog();
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
    }
}