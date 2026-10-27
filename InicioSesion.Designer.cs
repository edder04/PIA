using PIA.Diseños;

namespace PIA
{
    partial class InicioSesion
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InicioSesion));
            lblIniciar = new Label();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            txtContraseña = new TextBox();
            lblContraseña = new Label();
            lblSesion = new Label();
            trPicUANL = new TransparentPictureBox();
            trPicFCFM = new TransparentPictureBox();
            btnAceptar = new Button();
            ((System.ComponentModel.ISupportInitialize)trPicUANL).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trPicFCFM).BeginInit();
            SuspendLayout();
            // 
            // lblIniciar
            // 
            lblIniciar.AutoSize = true;
            lblIniciar.BackColor = Color.Transparent;
            lblIniciar.Font = new Font("Avenir LT Pro 65 Medium", 20.2499981F, FontStyle.Bold);
            lblIniciar.ForeColor = Color.White;
            lblIniciar.Location = new Point(442, 150);
            lblIniciar.Name = "lblIniciar";
            lblIniciar.RightToLeft = RightToLeft.No;
            lblIniciar.Size = new Size(116, 33);
            lblIniciar.TabIndex = 2;
            lblIniciar.Text = "INICIAR";
            lblIniciar.TextAlign = ContentAlignment.BottomCenter;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Avenir LT Pro 65 Medium", 15.7499981F, FontStyle.Bold);
            lblUsuario.ForeColor = SystemColors.ControlLightLight;
            lblUsuario.Location = new Point(443, 325);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(115, 25);
            lblUsuario.TabIndex = 3;
            lblUsuario.Text = "USUARIO:";
            // 
            // txtUsuario
            // 
            txtUsuario.Font = new Font("Avenir LT Pro 65 Medium", 12F, FontStyle.Bold);
            txtUsuario.Location = new Point(350, 350);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(300, 27);
            txtUsuario.TabIndex = 4;
            txtUsuario.TextAlign = HorizontalAlignment.Center;
            txtUsuario.KeyDown += txtUsuario_KeyDown;
            // 
            // txtContraseña
            // 
            txtContraseña.Font = new Font("Avenir LT Pro 65 Medium", 12F, FontStyle.Bold);
            txtContraseña.Location = new Point(350, 425);
            txtContraseña.Name = "txtContraseña";
            txtContraseña.Size = new Size(300, 27);
            txtContraseña.TabIndex = 6;
            txtContraseña.TextAlign = HorizontalAlignment.Center;
            txtContraseña.KeyDown += txtContraseña_KeyDown;
            // 
            // lblContraseña
            // 
            lblContraseña.AutoSize = true;
            lblContraseña.Font = new Font("Avenir LT Pro 65 Medium", 15.7499981F, FontStyle.Bold);
            lblContraseña.ForeColor = SystemColors.ControlLightLight;
            lblContraseña.Location = new Point(417, 400);
            lblContraseña.Name = "lblContraseña";
            lblContraseña.Size = new Size(166, 25);
            lblContraseña.TabIndex = 5;
            lblContraseña.Text = "CONTRASEÑA:";
            // 
            // lblSesion
            // 
            lblSesion.AutoSize = true;
            lblSesion.Font = new Font("Avenir LT Pro 65 Medium", 20.2499981F, FontStyle.Bold);
            lblSesion.ForeColor = SystemColors.ControlLightLight;
            lblSesion.Location = new Point(444, 180);
            lblSesion.Name = "lblSesion";
            lblSesion.RightToLeft = RightToLeft.No;
            lblSesion.Size = new Size(114, 33);
            lblSesion.TabIndex = 7;
            lblSesion.Text = "SESION";
            lblSesion.TextAlign = ContentAlignment.BottomCenter;
            // 
            // trPicUANL
            // 
            trPicUANL.BackColor = Color.Transparent;
            trPicUANL.BackgroundImageLayout = ImageLayout.None;
            trPicUANL.Image = (Image)resources.GetObject("trPicUANL.Image");
            trPicUANL.Location = new Point(263, 125);
            trPicUANL.Name = "trPicUANL";
            trPicUANL.Size = new Size(110, 110);
            trPicUANL.TabIndex = 8;
            trPicUANL.TabStop = false;
            // 
            // trPicFCFM
            // 
            trPicFCFM.BackColor = Color.Transparent;
            trPicFCFM.BackgroundImageLayout = ImageLayout.None;
            trPicFCFM.Image = (Image)resources.GetObject("trPicFCFM.Image");
            trPicFCFM.Location = new Point(636, 135);
            trPicFCFM.Name = "trPicFCFM";
            trPicFCFM.Size = new Size(100, 100);
            trPicFCFM.TabIndex = 9;
            trPicFCFM.TabStop = false;
            // 
            // btnAceptar
            // 
            btnAceptar.FlatStyle = FlatStyle.System;
            btnAceptar.Font = new Font("Avenir LT Pro 65 Medium", 14.1428576F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAceptar.Location = new Point(433, 500);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(134, 44);
            btnAceptar.TabIndex = 10;
            btnAceptar.Text = "ACEPTAR";
            btnAceptar.UseVisualStyleBackColor = true;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // InicioSesion
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(984, 761);
            Controls.Add(btnAceptar);
            Controls.Add(trPicFCFM);
            Controls.Add(trPicUANL);
            Controls.Add(lblSesion);
            Controls.Add(txtContraseña);
            Controls.Add(lblContraseña);
            Controls.Add(txtUsuario);
            Controls.Add(lblUsuario);
            Controls.Add(lblIniciar);
            Font = new Font("Avenir LT Pro 65 Medium", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InicioSesion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Inicio de Sesion";
            FormClosing += InicioSesion_FormClosing;
            ((System.ComponentModel.ISupportInitialize)trPicUANL).EndInit();
            ((System.ComponentModel.ISupportInitialize)trPicFCFM).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblIniciar;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private TextBox txtContraseña;
        private Label lblContraseña;
        private Label lblSesion;
        private TransparentPictureBox trPicUANL;
        private TransparentPictureBox trPicFCFM;
        private Button btnAceptar;
    }
}
