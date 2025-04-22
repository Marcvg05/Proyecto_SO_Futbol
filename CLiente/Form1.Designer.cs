namespace Cliente
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.MiUsuario = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MiPassword = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.connectedPlayersDataGridView = new System.Windows.Forms.DataGridView();
            this.statusLabel = new System.Windows.Forms.Label();
            this.btnCrearGrupo = new System.Windows.Forms.Button();
            this.txtInvitado = new System.Windows.Forms.TextBox();
            this.btnInvitar = new System.Windows.Forms.Button();
            this.txtMensajeGrupo = new System.Windows.Forms.TextBox();
            this.btnEnviarMensajeGrupo = new System.Windows.Forms.Button();
            this.btnSalirGrupo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.connectedPlayersDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 10);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 19);
            this.button1.TabIndex = 0;
            this.button1.Text = "Conectar con server";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(9, 100);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 20);
            this.button2.TabIndex = 1;
            this.button2.Text = "Registrarse";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(80, 101);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 19);
            this.button3.TabIndex = 2;
            this.button3.Text = "Iniciar sesion";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MiUsuario
            // 
            this.MiUsuario.Location = new System.Drawing.Point(70, 44);
            this.MiUsuario.Margin = new System.Windows.Forms.Padding(2);
            this.MiUsuario.Name = "MiUsuario";
            this.MiUsuario.Size = new System.Drawing.Size(76, 20);
            this.MiUsuario.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(20, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "usuario";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(9, 70);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Contraseña";
            // 
            // MiPassword
            // 
            this.MiPassword.Location = new System.Drawing.Point(70, 70);
            this.MiPassword.Margin = new System.Windows.Forms.Padding(2);
            this.MiPassword.Name = "MiPassword";
            this.MiPassword.Size = new System.Drawing.Size(76, 20);
            this.MiPassword.TabIndex = 6;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(9, 193);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(115, 19);
            this.button4.TabIndex = 7;
            this.button4.Text = "Consultar Jugadores";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(9, 225);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(115, 34);
            this.button5.TabIndex = 8;
            this.button5.Text = "Mostrar historial partidas";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(9, 492);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(56, 19);
            this.button6.TabIndex = 9;
            this.button6.Text = "Salir";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // connectedPlayersDataGridView
            // 
            this.connectedPlayersDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.connectedPlayersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.connectedPlayersDataGridView.Location = new System.Drawing.Point(758, 10);
            this.connectedPlayersDataGridView.Name = "connectedPlayersDataGridView";
            this.connectedPlayersDataGridView.Size = new System.Drawing.Size(240, 150);
            this.connectedPlayersDataGridView.TabIndex = 11;
            this.connectedPlayersDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.connectedPlayersDataGridView_CellContentClick);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.BackColor = System.Drawing.Color.White;
            this.statusLabel.Location = new System.Drawing.Point(894, 460);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(35, 13);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "label3";
            this.statusLabel.Click += new System.EventHandler(this.statusLabel_Click);
            // 
            // btnCrearGrupo
            // 
            this.btnCrearGrupo.Location = new System.Drawing.Point(9, 280);
            this.btnCrearGrupo.Name = "btnCrearGrupo";
            this.btnCrearGrupo.Size = new System.Drawing.Size(115, 24);
            this.btnCrearGrupo.TabIndex = 12;
            this.btnCrearGrupo.Text = "Crear Grupo";
            this.btnCrearGrupo.UseVisualStyleBackColor = true;
            this.btnCrearGrupo.Click += new System.EventHandler(this.btnCrearGrupo_Click);
            // 
            // txtInvitado
            // 
            this.txtInvitado.Location = new System.Drawing.Point(9, 310);
            this.txtInvitado.Name = "txtInvitado";
            this.txtInvitado.Size = new System.Drawing.Size(115, 20);
            this.txtInvitado.TabIndex = 13;
            // 
            // btnInvitar
            // 
            this.btnInvitar.Location = new System.Drawing.Point(10, 336);
            this.btnInvitar.Name = "btnInvitar";
            this.btnInvitar.Size = new System.Drawing.Size(57, 20);
            this.btnInvitar.TabIndex = 14;
            this.btnInvitar.Text = "Invitar";
            this.btnInvitar.UseVisualStyleBackColor = true;
            this.btnInvitar.Click += new System.EventHandler(this.btnInvitar_Click);
            // 
            // txtMensajeGrupo
            // 
            this.txtMensajeGrupo.Location = new System.Drawing.Point(12, 390);
            this.txtMensajeGrupo.Multiline = true;
            this.txtMensajeGrupo.Name = "txtMensajeGrupo";
            this.txtMensajeGrupo.Size = new System.Drawing.Size(112, 50);
            this.txtMensajeGrupo.TabIndex = 15;
            this.txtMensajeGrupo.TextChanged += new System.EventHandler(this.txtMensajeGrupo_TextChanged);
            // 
            // btnEnviarMensajeGrupo
            // 
            this.btnEnviarMensajeGrupo.Location = new System.Drawing.Point(49, 446);
            this.btnEnviarMensajeGrupo.Name = "btnEnviarMensajeGrupo";
            this.btnEnviarMensajeGrupo.Size = new System.Drawing.Size(75, 23);
            this.btnEnviarMensajeGrupo.TabIndex = 16;
            this.btnEnviarMensajeGrupo.Text = "Enviar Mensaje Grupo";
            this.btnEnviarMensajeGrupo.UseVisualStyleBackColor = true;
            this.btnEnviarMensajeGrupo.Click += new System.EventHandler(this.btnEnviarMensajeGrupo_Click);
            // 
            // btnSalirGrupo
            // 
            this.btnSalirGrupo.Location = new System.Drawing.Point(70, 336);
            this.btnSalirGrupo.Name = "btnSalirGrupo";
            this.btnSalirGrupo.Size = new System.Drawing.Size(57, 20);
            this.btnSalirGrupo.TabIndex = 17;
            this.btnSalirGrupo.Text = "Salir ";
            this.btnSalirGrupo.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label4.ForeColor = System.Drawing.Color.Snow;
            this.label4.Location = new System.Drawing.Point(10, 370);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 17);
            this.label4.TabIndex = 19;
            this.label4.Text = "Chat";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1010, 531);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSalirGrupo);
            this.Controls.Add(this.btnEnviarMensajeGrupo);
            this.Controls.Add(this.txtMensajeGrupo);
            this.Controls.Add(this.btnInvitar);
            this.Controls.Add(this.txtInvitado);
            this.Controls.Add(this.btnCrearGrupo);
            this.Controls.Add(this.connectedPlayersDataGridView);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.MiPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MiUsuario);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.connectedPlayersDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox MiUsuario;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MiPassword;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.DataGridView connectedPlayersDataGridView;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button btnCrearGrupo;
        private System.Windows.Forms.TextBox txtInvitado;
        private System.Windows.Forms.Button btnInvitar;
        private System.Windows.Forms.TextBox txtMensajeGrupo;
        private System.Windows.Forms.Button btnEnviarMensajeGrupo;
        private System.Windows.Forms.Button btnSalirGrupo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

