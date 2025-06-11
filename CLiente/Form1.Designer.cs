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
            this.components = new System.ComponentModel.Container();
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
            this.statusLabel = new System.Windows.Forms.Label();
            this.TITULO = new System.Windows.Forms.Label();
            this.timerBienvenida = new System.Windows.Forms.Timer(this.components);
            this.ChatBox = new System.Windows.Forms.TextBox();
            this.Enviar_Mensaje = new System.Windows.Forms.Button();
            this.CrearPartidaButton = new System.Windows.Forms.Button();
            this.InvitarJugadorButton = new System.Windows.Forms.Button();
            this.InvitarJugadorTextBox = new System.Windows.Forms.TextBox();
            this.MostrarJugadoresButton = new System.Windows.Forms.Button();
            this.lblBienvenida = new System.Windows.Forms.Label();
            this.IniciarPartidaButton = new System.Windows.Forms.Button();
            this.chatDataGridView = new System.Windows.Forms.DataGridView();
            this.connectedPlayersDataGridView = new System.Windows.Forms.DataGridView();
            this.rondaLabel = new System.Windows.Forms.Label();
            this.puntosLabel = new System.Windows.Forms.Label();
            this.dadoComboBox = new System.Windows.Forms.ComboBox();
            this.Enviar = new System.Windows.Forms.Button();
            this.dadoPictureBox = new System.Windows.Forms.PictureBox();
            this.SiguienteRonda = new System.Windows.Forms.Button();
            this.GanadorRonda = new System.Windows.Forms.Label();
            this.SALIR = new System.Windows.Forms.Button();
            this.GanadorPartida = new System.Windows.Forms.Label();
            this.DarseBaja = new System.Windows.Forms.Button();
            this.JugadoresConocidos = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.DiaMesAño = new System.Windows.Forms.TextBox();
            this.button8 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chatDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectedPlayersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dadoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button1.FlatAppearance.BorderSize = 4;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.Location = new System.Drawing.Point(681, 399);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(543, 168);
            this.button1.TabIndex = 0;
            this.button1.Text = "CONECTAR CON EL SERVIDOR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button2.FlatAppearance.BorderSize = 4;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(742, 571);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(187, 60);
            this.button2.TabIndex = 1;
            this.button2.Text = "Registrarse";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button3.FlatAppearance.BorderSize = 4;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(961, 571);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(215, 60);
            this.button3.TabIndex = 1;
            this.button3.Text = "Iniciar sesion";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MiUsuario
            // 
            this.MiUsuario.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MiUsuario.Location = new System.Drawing.Point(980, 352);
            this.MiUsuario.Margin = new System.Windows.Forms.Padding(2);
            this.MiUsuario.Name = "MiUsuario";
            this.MiUsuario.Size = new System.Drawing.Size(151, 51);
            this.MiUsuario.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(787, 363);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 34);
            this.label1.TabIndex = 4;
            this.label1.Text = "USUARIO";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(720, 475);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 34);
            this.label2.TabIndex = 5;
            this.label2.Text = "CONTRASEÑA";
            // 
            // MiPassword
            // 
            this.MiPassword.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MiPassword.Location = new System.Drawing.Point(980, 467);
            this.MiPassword.Margin = new System.Windows.Forms.Padding(2);
            this.MiPassword.Name = "MiPassword";
            this.MiPassword.Size = new System.Drawing.Size(151, 51);
            this.MiPassword.TabIndex = 6;
            // 
            // button4
            // 
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button4.FlatAppearance.BorderSize = 4;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(11, 179);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(187, 84);
            this.button4.TabIndex = 7;
            this.button4.Text = "Mostrar historial partidas";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button5.BackgroundImage")));
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button5.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button5.FlatAppearance.BorderSize = 4;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(11, 308);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(187, 84);
            this.button5.TabIndex = 8;
            this.button5.Text = "Consultar jugadores online ";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button6.BackgroundImage")));
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button6.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button6.FlatAppearance.BorderSize = 4;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Location = new System.Drawing.Point(11, 913);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(187, 84);
            this.button6.TabIndex = 9;
            this.button6.Text = "Salir";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.BackColor = System.Drawing.Color.White;
            this.statusLabel.Location = new System.Drawing.Point(1857, 1006);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(35, 13);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "label3";
            this.statusLabel.Click += new System.EventHandler(this.statusLabel_Click);
            // 
            // TITULO
            // 
            this.TITULO.AutoSize = true;
            this.TITULO.BackColor = System.Drawing.Color.Transparent;
            this.TITULO.Font = new System.Drawing.Font("MV Boli", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TITULO.ForeColor = System.Drawing.Color.White;
            this.TITULO.Location = new System.Drawing.Point(608, 9);
            this.TITULO.Name = "TITULO";
            this.TITULO.Size = new System.Drawing.Size(709, 375);
            this.TITULO.TabIndex = 12;
            this.TITULO.Text = "    CASINO \r\n      DE \r\nMONTEPINAR";
            this.TITULO.Visible = false;
            this.TITULO.Click += new System.EventHandler(this.TITULO_Click);
            // 
            // timerBienvenida
            // 
            this.timerBienvenida.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ChatBox
            // 
            this.ChatBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.ChatBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChatBox.ForeColor = System.Drawing.Color.White;
            this.ChatBox.Location = new System.Drawing.Point(1694, 308);
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.Size = new System.Drawing.Size(207, 13);
            this.ChatBox.TabIndex = 14;
            // 
            // Enviar_Mensaje
            // 
            this.Enviar_Mensaje.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Enviar_Mensaje.BackgroundImage")));
            this.Enviar_Mensaje.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Enviar_Mensaje.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.Enviar_Mensaje.FlatAppearance.BorderSize = 4;
            this.Enviar_Mensaje.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Enviar_Mensaje.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.Enviar_Mensaje.ForeColor = System.Drawing.Color.White;
            this.Enviar_Mensaje.Location = new System.Drawing.Point(1764, 412);
            this.Enviar_Mensaje.Name = "Enviar_Mensaje";
            this.Enviar_Mensaje.Size = new System.Drawing.Size(128, 61);
            this.Enviar_Mensaje.TabIndex = 15;
            this.Enviar_Mensaje.Text = "Enviar";
            this.Enviar_Mensaje.UseVisualStyleBackColor = true;
            this.Enviar_Mensaje.Click += new System.EventHandler(this.Enviar_Mensaje_Click);
            // 
            // CrearPartidaButton
            // 
            this.CrearPartidaButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CrearPartidaButton.BackgroundImage")));
            this.CrearPartidaButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CrearPartidaButton.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.CrearPartidaButton.FlatAppearance.BorderSize = 4;
            this.CrearPartidaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CrearPartidaButton.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.CrearPartidaButton.ForeColor = System.Drawing.Color.White;
            this.CrearPartidaButton.Location = new System.Drawing.Point(11, 435);
            this.CrearPartidaButton.Margin = new System.Windows.Forms.Padding(2);
            this.CrearPartidaButton.Name = "CrearPartidaButton";
            this.CrearPartidaButton.Size = new System.Drawing.Size(187, 84);
            this.CrearPartidaButton.TabIndex = 17;
            this.CrearPartidaButton.Text = "Crear Partida";
            this.CrearPartidaButton.UseVisualStyleBackColor = true;
            this.CrearPartidaButton.Visible = false;
            this.CrearPartidaButton.Click += new System.EventHandler(this.CrearPartidaButton_Click);
            // 
            // InvitarJugadorButton
            // 
            this.InvitarJugadorButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("InvitarJugadorButton.BackgroundImage")));
            this.InvitarJugadorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.InvitarJugadorButton.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.InvitarJugadorButton.FlatAppearance.BorderSize = 4;
            this.InvitarJugadorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InvitarJugadorButton.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.InvitarJugadorButton.ForeColor = System.Drawing.Color.White;
            this.InvitarJugadorButton.Location = new System.Drawing.Point(11, 608);
            this.InvitarJugadorButton.Name = "InvitarJugadorButton";
            this.InvitarJugadorButton.Size = new System.Drawing.Size(187, 84);
            this.InvitarJugadorButton.TabIndex = 18;
            this.InvitarJugadorButton.Text = "Invitar Jugador";
            this.InvitarJugadorButton.UseVisualStyleBackColor = true;
            this.InvitarJugadorButton.Visible = false;
            this.InvitarJugadorButton.Click += new System.EventHandler(this.InvitarJugadorButton_Click);
            // 
            // InvitarJugadorTextBox
            // 
            this.InvitarJugadorTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.InvitarJugadorTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.InvitarJugadorTextBox.ForeColor = System.Drawing.Color.White;
            this.InvitarJugadorTextBox.Location = new System.Drawing.Point(28, 554);
            this.InvitarJugadorTextBox.Name = "InvitarJugadorTextBox";
            this.InvitarJugadorTextBox.Size = new System.Drawing.Size(150, 13);
            this.InvitarJugadorTextBox.TabIndex = 19;
            this.InvitarJugadorTextBox.Visible = false;
            // 
            // MostrarJugadoresButton
            // 
            this.MostrarJugadoresButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("MostrarJugadoresButton.BackgroundImage")));
            this.MostrarJugadoresButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MostrarJugadoresButton.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.MostrarJugadoresButton.FlatAppearance.BorderSize = 4;
            this.MostrarJugadoresButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MostrarJugadoresButton.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.MostrarJugadoresButton.ForeColor = System.Drawing.Color.White;
            this.MostrarJugadoresButton.Location = new System.Drawing.Point(11, 726);
            this.MostrarJugadoresButton.Name = "MostrarJugadoresButton";
            this.MostrarJugadoresButton.Size = new System.Drawing.Size(187, 84);
            this.MostrarJugadoresButton.TabIndex = 20;
            this.MostrarJugadoresButton.Text = "Jugadores en Partida";
            this.MostrarJugadoresButton.UseVisualStyleBackColor = true;
            this.MostrarJugadoresButton.Visible = false;
            this.MostrarJugadoresButton.Click += new System.EventHandler(this.MostrarJugadoresButton_Click);
            // 
            // lblBienvenida
            // 
            this.lblBienvenida.BackColor = System.Drawing.Color.Transparent;
            this.lblBienvenida.Font = new System.Drawing.Font("MV Boli", 95.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBienvenida.ForeColor = System.Drawing.Color.White;
            this.lblBienvenida.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBienvenida.Location = new System.Drawing.Point(-603, 394);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new System.Drawing.Size(1259, 550);
            this.lblBienvenida.TabIndex = 13;
            this.lblBienvenida.Text = "BIENVENIDO AL\r\nCASINO DE\r\nMONTEPINAR\r\n";
            this.lblBienvenida.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBienvenida.Visible = false;
            this.lblBienvenida.Click += new System.EventHandler(this.lblBienvenida_Click);
            // 
            // IniciarPartidaButton
            // 
            this.IniciarPartidaButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("IniciarPartidaButton.BackgroundImage")));
            this.IniciarPartidaButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.IniciarPartidaButton.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.IniciarPartidaButton.FlatAppearance.BorderSize = 4;
            this.IniciarPartidaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IniciarPartidaButton.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.IniciarPartidaButton.ForeColor = System.Drawing.Color.White;
            this.IniciarPartidaButton.Location = new System.Drawing.Point(11, 825);
            this.IniciarPartidaButton.Name = "IniciarPartidaButton";
            this.IniciarPartidaButton.Size = new System.Drawing.Size(187, 84);
            this.IniciarPartidaButton.TabIndex = 21;
            this.IniciarPartidaButton.Text = "Iniciar Partida";
            this.IniciarPartidaButton.UseVisualStyleBackColor = true;
            this.IniciarPartidaButton.Visible = false;
            this.IniciarPartidaButton.Click += new System.EventHandler(this.IniciarPartidaButton_Click_1);
            // 
            // chatDataGridView
            // 
            this.chatDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.chatDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.chatDataGridView.Location = new System.Drawing.Point(1694, 6);
            this.chatDataGridView.Name = "chatDataGridView";
            this.chatDataGridView.Size = new System.Drawing.Size(207, 296);
            this.chatDataGridView.TabIndex = 23;
            // 
            // connectedPlayersDataGridView
            // 
            this.connectedPlayersDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.connectedPlayersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.connectedPlayersDataGridView.Location = new System.Drawing.Point(1694, 707);
            this.connectedPlayersDataGridView.Name = "connectedPlayersDataGridView";
            this.connectedPlayersDataGridView.Size = new System.Drawing.Size(207, 296);
            this.connectedPlayersDataGridView.TabIndex = 24;
            this.connectedPlayersDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.connectedPlayersDataGridView_CellContentClick_1);
            // 
            // rondaLabel
            // 
            this.rondaLabel.AutoSize = true;
            this.rondaLabel.BackColor = System.Drawing.Color.Transparent;
            this.rondaLabel.Font = new System.Drawing.Font("MV Boli", 32.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rondaLabel.ForeColor = System.Drawing.Color.White;
            this.rondaLabel.Location = new System.Drawing.Point(858, 374);
            this.rondaLabel.Name = "rondaLabel";
            this.rondaLabel.Size = new System.Drawing.Size(220, 55);
            this.rondaLabel.TabIndex = 25;
            this.rondaLabel.Text = "Ronda: 0";
            this.rondaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // puntosLabel
            // 
            this.puntosLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.puntosLabel.AutoSize = true;
            this.puntosLabel.BackColor = System.Drawing.Color.Transparent;
            this.puntosLabel.Font = new System.Drawing.Font("MV Boli", 32.25F, System.Drawing.FontStyle.Bold);
            this.puntosLabel.ForeColor = System.Drawing.Color.White;
            this.puntosLabel.Location = new System.Drawing.Point(838, 463);
            this.puntosLabel.Name = "puntosLabel";
            this.puntosLabel.Size = new System.Drawing.Size(240, 55);
            this.puntosLabel.TabIndex = 26;
            this.puntosLabel.Text = "Puntos: 0";
            this.puntosLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dadoComboBox
            // 
            this.dadoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dadoComboBox.FormattingEnabled = true;
            this.dadoComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.dadoComboBox.Location = new System.Drawing.Point(783, 615);
            this.dadoComboBox.Name = "dadoComboBox";
            this.dadoComboBox.Size = new System.Drawing.Size(146, 21);
            this.dadoComboBox.TabIndex = 27;
            // 
            // Enviar
            // 
            this.Enviar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Enviar.BackgroundImage")));
            this.Enviar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Enviar.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.Enviar.FlatAppearance.BorderSize = 4;
            this.Enviar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Enviar.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.Enviar.ForeColor = System.Drawing.Color.White;
            this.Enviar.Location = new System.Drawing.Point(995, 572);
            this.Enviar.Name = "Enviar";
            this.Enviar.Size = new System.Drawing.Size(211, 97);
            this.Enviar.TabIndex = 28;
            this.Enviar.Text = "Enviar";
            this.Enviar.UseVisualStyleBackColor = true;
            this.Enviar.Click += new System.EventHandler(this.Enviar_Click_1);
            // 
            // dadoPictureBox
            // 
            this.dadoPictureBox.Location = new System.Drawing.Point(877, 707);
            this.dadoPictureBox.Name = "dadoPictureBox";
            this.dadoPictureBox.Size = new System.Drawing.Size(180, 168);
            this.dadoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.dadoPictureBox.TabIndex = 29;
            this.dadoPictureBox.TabStop = false;
            this.dadoPictureBox.Click += new System.EventHandler(this.dadoPictureBox_Click);
            // 
            // SiguienteRonda
            // 
            this.SiguienteRonda.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SiguienteRonda.BackgroundImage")));
            this.SiguienteRonda.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SiguienteRonda.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.SiguienteRonda.FlatAppearance.BorderSize = 4;
            this.SiguienteRonda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SiguienteRonda.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.SiguienteRonda.ForeColor = System.Drawing.Color.White;
            this.SiguienteRonda.Location = new System.Drawing.Point(1099, 707);
            this.SiguienteRonda.Name = "SiguienteRonda";
            this.SiguienteRonda.Size = new System.Drawing.Size(260, 68);
            this.SiguienteRonda.TabIndex = 30;
            this.SiguienteRonda.Text = "Siguiente Ronda";
            this.SiguienteRonda.UseVisualStyleBackColor = true;
            this.SiguienteRonda.Click += new System.EventHandler(this.SiguienteRonda_Click_1);
            // 
            // GanadorRonda
            // 
            this.GanadorRonda.AutoSize = true;
            this.GanadorRonda.BackColor = System.Drawing.Color.Transparent;
            this.GanadorRonda.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.GanadorRonda.ForeColor = System.Drawing.Color.White;
            this.GanadorRonda.Location = new System.Drawing.Point(803, 901);
            this.GanadorRonda.Name = "GanadorRonda";
            this.GanadorRonda.Size = new System.Drawing.Size(93, 34);
            this.GanadorRonda.TabIndex = 31;
            this.GanadorRonda.Text = "label3";
            // 
            // SALIR
            // 
            this.SALIR.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SALIR.BackgroundImage")));
            this.SALIR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SALIR.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.SALIR.FlatAppearance.BorderSize = 4;
            this.SALIR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SALIR.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold);
            this.SALIR.ForeColor = System.Drawing.Color.White;
            this.SALIR.Location = new System.Drawing.Point(1114, 800);
            this.SALIR.Name = "SALIR";
            this.SALIR.Size = new System.Drawing.Size(228, 92);
            this.SALIR.TabIndex = 32;
            this.SALIR.Text = "Salir de la partida";
            this.SALIR.UseVisualStyleBackColor = true;
            this.SALIR.Click += new System.EventHandler(this.SALIR_Click_1);
            // 
            // GanadorPartida
            // 
            this.GanadorPartida.AutoSize = true;
            this.GanadorPartida.BackColor = System.Drawing.Color.Transparent;
            this.GanadorPartida.Font = new System.Drawing.Font("MV Boli", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GanadorPartida.ForeColor = System.Drawing.Color.White;
            this.GanadorPartida.Location = new System.Drawing.Point(454, 31);
            this.GanadorPartida.Name = "GanadorPartida";
            this.GanadorPartida.Size = new System.Drawing.Size(149, 52);
            this.GanadorPartida.TabIndex = 33;
            this.GanadorPartida.Text = "Label3";
            this.GanadorPartida.Click += new System.EventHandler(this.GanadorPartida_Click);
            // 
            // DarseBaja
            // 
            this.DarseBaja.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("DarseBaja.BackgroundImage")));
            this.DarseBaja.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.DarseBaja.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.DarseBaja.FlatAppearance.BorderSize = 4;
            this.DarseBaja.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DarseBaja.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DarseBaja.ForeColor = System.Drawing.Color.White;
            this.DarseBaja.Location = new System.Drawing.Point(11, 55);
            this.DarseBaja.Margin = new System.Windows.Forms.Padding(2);
            this.DarseBaja.Name = "DarseBaja";
            this.DarseBaja.Size = new System.Drawing.Size(187, 84);
            this.DarseBaja.TabIndex = 34;
            this.DarseBaja.Text = "Darse de baja";
            this.DarseBaja.UseVisualStyleBackColor = true;
            this.DarseBaja.Click += new System.EventHandler(this.DarseBaja_Click);
            // 
            // JugadoresConocidos
            // 
            this.JugadoresConocidos.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("JugadoresConocidos.BackgroundImage")));
            this.JugadoresConocidos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.JugadoresConocidos.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.JugadoresConocidos.FlatAppearance.BorderSize = 4;
            this.JugadoresConocidos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.JugadoresConocidos.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JugadoresConocidos.ForeColor = System.Drawing.Color.White;
            this.JugadoresConocidos.Location = new System.Drawing.Point(222, 55);
            this.JugadoresConocidos.Margin = new System.Windows.Forms.Padding(2);
            this.JugadoresConocidos.Name = "JugadoresConocidos";
            this.JugadoresConocidos.Size = new System.Drawing.Size(187, 84);
            this.JugadoresConocidos.TabIndex = 35;
            this.JugadoresConocidos.Text = "Jugadores Conocidos";
            this.JugadoresConocidos.UseVisualStyleBackColor = true;
            this.JugadoresConocidos.Click += new System.EventHandler(this.JugadoresConocidos_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(238, 220);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(150, 13);
            this.textBox1.TabIndex = 36;
            this.textBox1.Visible = false;
            // 
            // button7
            // 
            this.button7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button7.BackgroundImage")));
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button7.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button7.FlatAppearance.BorderSize = 4;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Location = new System.Drawing.Point(407, 179);
            this.button7.Margin = new System.Windows.Forms.Padding(2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(269, 84);
            this.button7.TabIndex = 37;
            this.button7.Text = "Resultado de partidas contra:";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.ForeColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(877, 514);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(150, 13);
            this.textBox2.TabIndex = 38;
            this.textBox2.Visible = false;
            // 
            // DiaMesAño
            // 
            this.DiaMesAño.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.DiaMesAño.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DiaMesAño.ForeColor = System.Drawing.Color.White;
            this.DiaMesAño.Location = new System.Drawing.Point(222, 349);
            this.DiaMesAño.Name = "DiaMesAño";
            this.DiaMesAño.Size = new System.Drawing.Size(150, 13);
            this.DiaMesAño.TabIndex = 39;
            this.DiaMesAño.Visible = false;
            // 
            // button8
            // 
            this.button8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button8.BackgroundImage")));
            this.button8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button8.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.button8.FlatAppearance.BorderSize = 4;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("MV Boli", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.ForeColor = System.Drawing.Color.White;
            this.button8.Location = new System.Drawing.Point(387, 300);
            this.button8.Margin = new System.Windows.Forms.Padding(2);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(269, 84);
            this.button8.TabIndex = 40;
            this.button8.Text = "Lista de partidas";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.DiaMesAño);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.JugadoresConocidos);
            this.Controls.Add(this.DarseBaja);
            this.Controls.Add(this.GanadorPartida);
            this.Controls.Add(this.SALIR);
            this.Controls.Add(this.GanadorRonda);
            this.Controls.Add(this.SiguienteRonda);
            this.Controls.Add(this.dadoPictureBox);
            this.Controls.Add(this.Enviar);
            this.Controls.Add(this.dadoComboBox);
            this.Controls.Add(this.puntosLabel);
            this.Controls.Add(this.rondaLabel);
            this.Controls.Add(this.connectedPlayersDataGridView);
            this.Controls.Add(this.chatDataGridView);
            this.Controls.Add(this.IniciarPartidaButton);
            this.Controls.Add(this.Enviar_Mensaje);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.lblBienvenida);
            this.Controls.Add(this.TITULO);
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
            this.Controls.Add(this.CrearPartidaButton);
            this.Controls.Add(this.InvitarJugadorButton);
            this.Controls.Add(this.InvitarJugadorTextBox);
            this.Controls.Add(this.MostrarJugadoresButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chatDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectedPlayersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dadoPictureBox)).EndInit();
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
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label TITULO;
        private System.Windows.Forms.Timer timerBienvenida;
        private System.Windows.Forms.TextBox ChatBox;
        private System.Windows.Forms.Button Enviar_Mensaje;
        private System.Windows.Forms.Button CrearPartidaButton;
        private System.Windows.Forms.Button InvitarJugadorButton;
        private System.Windows.Forms.TextBox InvitarJugadorTextBox;
        private System.Windows.Forms.Button MostrarJugadoresButton;
        private System.Windows.Forms.Label lblBienvenida;
        private System.Windows.Forms.Button IniciarPartidaButton;
        private System.Windows.Forms.DataGridView chatDataGridView;
        private System.Windows.Forms.DataGridView connectedPlayersDataGridView;
        private System.Windows.Forms.Label rondaLabel;
        private System.Windows.Forms.Label puntosLabel;
        private System.Windows.Forms.ComboBox dadoComboBox;
        private System.Windows.Forms.Button Enviar;
        private System.Windows.Forms.PictureBox dadoPictureBox;
        private System.Windows.Forms.Button SiguienteRonda;
        private System.Windows.Forms.Label GanadorRonda;
        private System.Windows.Forms.Button SALIR;
        private System.Windows.Forms.Label GanadorPartida;
        private System.Windows.Forms.Button DarseBaja;
        private System.Windows.Forms.Button JugadoresConocidos;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox DiaMesAño;
        private System.Windows.Forms.Button button8;
    }
}

