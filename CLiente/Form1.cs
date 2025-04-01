using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace Cliente
{
    public partial class Form1 : Form
    {
        Socket server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e) //Jugadores Conectados
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor");
                return;
            }

            try
            {
                // Enviar solicitud (código 4 para listar conectados)
                string mensaje = "4";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Recibir respuesta con buffer más grande
                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim();

                // Eliminar cualquier carácter adicional después del último jugador
                respuesta = respuesta.Split('\0')[0];

                // Procesar respuesta
                string[] partes = respuesta.Split('/');

                if (partes.Length < 1 || !int.TryParse(partes[0], out int totalJugadores))
                {
                    MessageBox.Show($"Formato de respuesta inválido. Recibido: {respuesta}");
                    return;
                }

                // Crear tabla
                DataTable tabla = new DataTable();
                tabla.Columns.Add("#", typeof(int));
                tabla.Columns.Add("Jugador", typeof(string));
                tabla.Columns.Add("Estado", typeof(string));

                for (int i = 1; i < partes.Length && i <= totalJugadores; i++)
                {
                    if (!string.IsNullOrEmpty(partes[i]))
                    {
                        tabla.Rows.Add(i, partes[i], "Conectado");
                    }
                }

                // Mostrar en ventana
                if (tabla.Rows.Count > 0)
                {
                    Form form = new Form();
                    DataGridView dgv = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        DataSource = tabla,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    };

                    form.Text = $"Jugadores Conectados ({totalJugadores})";
                    form.Size = new Size(400, 300);
                    form.Controls.Add(dgv);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No hay jugadores conectados actualmente");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e) //Conectar server
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 9050);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                MessageBox.Show("Conectado");

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

        }

        private void button6_Click(object sender, EventArgs e) //Salir
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e) //Registrarse
        {
            string mensaje = "1" + "|" + MiUsuario.Text + "|" + MiPassword.Text;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            mensaje = Encoding.ASCII.GetString(msg2).Split(',')[0];
            MessageBox.Show(mensaje);
        }
        private void button3_Click(object sender, EventArgs e) // Iniciar Sesión
        {
            try
            {
                // 1. Verificar conexión existente
                if (server == null || !server.Connected)
                {
                    // 2. Reconexión automática
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.56.102"), 9050);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // 3. Configurar timeout
                    server.SendTimeout = 5000; // 5 segundos
                    server.ReceiveTimeout = 5000;

                    IAsyncResult result = server.BeginConnect(ipep, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                    if (!success)
                    {
                        MessageBox.Show("Timeout de conexión");
                        server.Close();
                        return;
                    }
                }

                // 4. Enviar credenciales
                string mensaje = $"2|{MiUsuario.Text}|{MiPassword.Text}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);

                // 5. Envío seguro
                int bytesSent = server.Send(msg);
                if (bytesSent != msg.Length)
                {
                    MessageBox.Show("Error al enviar datos completos");
                    return;
                }

                // 6. Recibir respuesta con buffer dinámico
                byte[] buffer = new byte[1024];
                int bytesRec = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec);

                MessageBox.Show(respuesta.Split(',')[0]);
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Error de socket: {ex.SocketErrorCode}\n{ex.Message}");

                // 7. Limpieza de conexión fallida
                if (server != null)
                {
                    server.Close();
                    server = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}");
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
                // 1. Enviar código de solicitud (5 = historial de partidas)
                string mensaje = "5";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // 2. Recibir datos (buffer más grande para múltiples partidas)
                byte[] buffer = new byte[4096];
                int bytesReceived = server.Receive(buffer);
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                // 3. Procesar respuesta y mostrar en DataGridView
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
            // Crear tabla temporal
            DataTable tablaPartidas = new DataTable();
            tablaPartidas.Columns.Add("ID Partida");
            tablaPartidas.Columns.Add("Fecha");
            tablaPartidas.Columns.Add("Duración (min)");
            tablaPartidas.Columns.Add("Ganador");

            // Dividir la respuesta del servidor
            string[] partidas = datosPartidas.Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string partida in partidas)
            {
                if (!string.IsNullOrWhiteSpace(partida))
                {
                    // Extraer datos de cada partida (formato: "ID: 1, Fecha: 2023-10-01, Duracion: 30 min, Ganador: player1")
                    string[] partes = partida.Split(new[] { ", " }, StringSplitOptions.None);

                    if (partes.Length >= 4)
                    {
                        string id = partes[0].Replace("ID: ", "");
                        string fecha = partes[1].Replace("Fecha: ", "");
                        string duracion = partes[2].Replace("Duracion: ", "").Replace(" min", "");
                        string ganador = partes[3].Replace("Ganador: ", "");

                        // Añadir fila a la tabla
                        tablaPartidas.Rows.Add(id, fecha, duracion, ganador);
                    }
                }
            }

            // Crear formulario para mostrar los datos
            Form formHistorial = new Form();
            formHistorial.Text = "Historial de Partidas";
            formHistorial.Width = 600;
            formHistorial.Height = 400;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.DataSource = tablaPartidas;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;

            // Añadir botón de cerrar
            Button btnCerrar = new Button();
            btnCerrar.Text = "Cerrar";
            btnCerrar.Dock = DockStyle.Bottom;
            btnCerrar.Click += (s, ev) => formHistorial.Close();

            formHistorial.Controls.Add(dgv);
            formHistorial.Controls.Add(btnCerrar);
            formHistorial.ShowDialog();
        }
    }
}
