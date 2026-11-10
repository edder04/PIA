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
			btnVenta = new Button();
			lblDisponibilidadInCard = new Label();
			lblEstadioInCard = new Label();
			lblFechaHoraInCard = new Label();
			label2 = new Label();
			lblEquipoVisitanteInCard = new Label();
			lblEquipoLocalInCard = new Label();
			SuspendLayout();
			// 
			// btnVenta
			// 
			btnVenta.FlatStyle = FlatStyle.System;
			btnVenta.Font = new Font("Avenir LT Pro 65 Medium", 9.857142F, FontStyle.Bold, GraphicsUnit.Point, 0);
			btnVenta.Location = new Point(95, 132);
			btnVenta.Margin = new Padding(2);
			btnVenta.Name = "btnVenta";
			btnVenta.Size = new Size(135, 25);
			btnVenta.TabIndex = 24;
			btnVenta.Text = "Hacer Venta";
			btnVenta.UseVisualStyleBackColor = true;
			btnVenta.Click += btnVenta_Click;
			// 
			// lblDisponibilidadInCard
			// 
			lblDisponibilidadInCard.AutoSize = true;
			lblDisponibilidadInCard.Font = new Font("Avenir LT Pro 65 Medium", 11.1428576F, FontStyle.Bold);
			lblDisponibilidadInCard.ForeColor = Color.White;
			lblDisponibilidadInCard.Location = new Point(11, 104);
			lblDisponibilidadInCard.Margin = new Padding(2, 0, 2, 0);
			lblDisponibilidadInCard.Name = "lblDisponibilidadInCard";
			lblDisponibilidadInCard.Size = new Size(108, 18);
			lblDisponibilidadInCard.TabIndex = 23;
			lblDisponibilidadInCard.Text = "Disponibilidad";
			// 
			// lblEstadioInCard
			// 
			lblEstadioInCard.AutoSize = true;
			lblEstadioInCard.Font = new Font("Avenir LT Pro 65 Medium", 11.1428576F, FontStyle.Bold);
			lblEstadioInCard.ForeColor = Color.White;
			lblEstadioInCard.Location = new Point(11, 80);
			lblEstadioInCard.Margin = new Padding(2, 0, 2, 0);
			lblEstadioInCard.Name = "lblEstadioInCard";
			lblEstadioInCard.Size = new Size(60, 18);
			lblEstadioInCard.TabIndex = 22;
			lblEstadioInCard.Text = "Estadio";
			// 
			// lblFechaHoraInCard
			// 
			lblFechaHoraInCard.AutoSize = true;
			lblFechaHoraInCard.Font = new Font("Avenir LT Pro 65 Medium", 11.1428576F, FontStyle.Bold);
			lblFechaHoraInCard.ForeColor = Color.White;
			lblFechaHoraInCard.Location = new Point(11, 55);
			lblFechaHoraInCard.Margin = new Padding(2, 0, 2, 0);
			lblFechaHoraInCard.Name = "lblFechaHoraInCard";
			lblFechaHoraInCard.Size = new Size(98, 18);
			lblFechaHoraInCard.TabIndex = 21;
			lblFechaHoraInCard.Text = "Fecha y hora";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new Font("Avenir LT Pro 65 Medium", 14.1428576F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label2.ForeColor = Color.White;
			label2.Location = new Point(153, 9);
			label2.Margin = new Padding(2, 0, 2, 0);
			label2.Name = "label2";
			label2.Size = new Size(30, 23);
			label2.TabIndex = 20;
			label2.Text = "vs";
			// 
			// lblEquipoVisitanteInCard
			// 
			lblEquipoVisitanteInCard.AutoSize = true;
			lblEquipoVisitanteInCard.Font = new Font("Avenir LT Pro 65 Medium", 14.1428576F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblEquipoVisitanteInCard.ForeColor = Color.White;
			lblEquipoVisitanteInCard.Location = new Point(229, 9);
			lblEquipoVisitanteInCard.Margin = new Padding(2, 0, 2, 0);
			lblEquipoVisitanteInCard.Name = "lblEquipoVisitanteInCard";
			lblEquipoVisitanteInCard.Size = new Size(85, 23);
			lblEquipoVisitanteInCard.TabIndex = 19;
			lblEquipoVisitanteInCard.Text = "Equipo2";
			// 
			// lblEquipoLocalInCard
			// 
			lblEquipoLocalInCard.AutoSize = true;
			lblEquipoLocalInCard.Font = new Font("Avenir LT Pro 65 Medium", 14.1428576F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblEquipoLocalInCard.ForeColor = Color.White;
			lblEquipoLocalInCard.Location = new Point(11, 9);
			lblEquipoLocalInCard.Margin = new Padding(2, 0, 2, 0);
			lblEquipoLocalInCard.Name = "lblEquipoLocalInCard";
			lblEquipoLocalInCard.Size = new Size(85, 23);
			lblEquipoLocalInCard.TabIndex = 18;
			lblEquipoLocalInCard.Text = "Equipo1";
			// 
			// UcCardPartido
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.Black;
			Controls.Add(btnVenta);
			Controls.Add(lblDisponibilidadInCard);
			Controls.Add(lblEstadioInCard);
			Controls.Add(lblFechaHoraInCard);
			Controls.Add(label2);
			Controls.Add(lblEquipoVisitanteInCard);
			Controls.Add(lblEquipoLocalInCard);
			Margin = new Padding(6, 5, 6, 5);
			Name = "UcCardPartido";
			Size = new Size(331, 170);
			Load += UcCardPartido_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button btnVenta;
        private Label lblDisponibilidadInCard;
        private Label lblEstadioInCard;
        private Label lblFechaHoraInCard;
        private Label label2;
        private Label lblEquipoVisitanteInCard;
        private Label lblEquipoLocalInCard;
    }
}
