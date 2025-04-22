using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Cliente
{
    public partial class InvitationForm : Form
    {
        private string inviterName;
        private int grupoIdx;
        private Socket server;

        public InvitationForm(int grupoIdx, Socket server)
        {
            InitializeComponent();
            this.grupoIdx = grupoIdx;
            this.server = server;

            lblInvitation.Text = $"{inviterName} te ha invitado a un grupo. ¿Quieres unirte?";

            // Configuración adicional
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }
        

        private void btnAccept_Click_1(object sender, EventArgs e)
        {
            string mensaje = $"10|{grupoIdx}";
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            this.Close();
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            try
            {
                string mensaje = $"11|{grupoIdx}";
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                this.Close();

                // Opcional: Mostrar confirmación al usuario
                MessageBox.Show("Has rechazado la invitación al grupo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al rechazar invitación: {ex.Message}");
            }
        }
    }
}
