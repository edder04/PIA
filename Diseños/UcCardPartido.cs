using PIA.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PIA
{
	public partial class UcCardPartido : UserControl
	{
		private int _idPartido;
		private vw_Partidos? _partido;
		public event Action<vw_Partidos>? PartidoElegido;

		public UcCardPartido()
		{
			InitializeComponent();
		}

		public void CargarPartido(vw_Partidos partido)
		{
			_idPartido = partido.IdPartido;

			lblEquipoLocalInCard.Text = partido.EquipoLocal;
			lblEquipoVisitanteInCard.Text = partido.EquipoVisitante;
			lblFechaHoraInCard.Text = partido.FechaHora.ToString("dd/MM/yyyy HH:mm");
			lblEstadioInCard.Text = partido.Estadio;
			lblDisponibilidadInCard.Text =
				$"{partido.BoletosDisponibles} boletos disponibles";
		}

		private void UcCardPartido_Load(object sender, EventArgs e)
		{

		}

		private void btnVenta_Click(object sender, EventArgs e)
		{
			if (_partido is not null){
				PartidoElegido?.Invoke(_partido);
			}
		}
	}
}
