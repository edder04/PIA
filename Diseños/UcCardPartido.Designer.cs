namespace PIA
{
    partial class UcCardPartido
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

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            button6 = new Button();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            label17 = new Label();
            SuspendLayout();
            // 
            // button6
            // 
            button6.FlatStyle = FlatStyle.System;
            button6.Font = new Font("Avenir LT Pro 65 Medium", 9.857142F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button6.Location = new Point(163, 265);
            button6.Name = "button6";
            button6.Size = new Size(231, 50);
            button6.TabIndex = 24;
            button6.Text = "Ver Disponibilidad";
            button6.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Avenir LT Pro 65 Medium", 11.1428576F, FontStyle.Bold);
            label5.ForeColor = Color.White;
            label5.Location = new Point(19, 208);
            label5.Name = "label5";
            label5.Size = new Size(196, 33);
            label5.TabIndex = 23;
            label5.Text = "Disponibilidad";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Avenir LT Pro 65 Medium", 11.1428576F, FontStyle.Bold);
            label4.ForeColor = Color.White;
            label4.Location = new Point(19, 160);
            label4.Name = "label4";
            label4.Size = new Size(110, 33);
            label4.TabIndex = 22;
            label4.Text = "Estadio";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Avenir LT Pro 65 Medium", 11.1428576F, FontStyle.Bold);
            label3.ForeColor = Color.White;
            label3.Location = new Point(19, 110);
            label3.Name = "label3";
            label3.Size = new Size(175, 33);
            label3.TabIndex = 21;
            label3.Text = "Fecha y hora";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Avenir LT Pro 65 Medium", 14.1428576F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(262, 18);
            label2.Name = "label2";
            label2.Size = new Size(50, 40);
            label2.TabIndex = 20;
            label2.Text = "vs";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Avenir LT Pro 65 Medium", 14.1428576F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(393, 18);
            label1.Name = "label1";
            label1.Size = new Size(148, 40);
            label1.TabIndex = 19;
            label1.Text = "Equipo2";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Avenir LT Pro 65 Medium", 14.1428576F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label17.ForeColor = Color.White;
            label17.Location = new Point(19, 18);
            label17.Name = "label17";
            label17.Size = new Size(148, 40);
            label17.TabIndex = 18;
            label17.Text = "Equipo1";
            // 
            // UcCardPartido
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            Controls.Add(button6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(label17);
            Margin = new Padding(10);
            Name = "UcCardPartido";
            Size = new Size(567, 340);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label17;
    }
}
