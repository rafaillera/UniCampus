using System;
using System.Collections.Generic;
using System.Globalization;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Seguridad;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>
    /// Vista (MVP) de reportes. Solo organizadores y administradores.
    /// </summary>
    public partial class Reportes : PaginaBase, IReportesView
    {
        private ReportesPresenter _presenter;

        protected override string[] RolesPermitidos
        {
            get { return Roles.Gestores; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ReportesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Cargar();
            }
        }

        public void MostrarResumen(ResumenActividadesDto resumen)
        {
            CultureInfo cultura = CultureInfo.CurrentCulture;

            litTotal.Text = resumen.TotalActividades.ToString(cultura);
            litInscripciones.Text = resumen.Inscripciones.ToString(cultura);
            litProximas.Text = resumen.Proximas.ToString(cultura);
            litRealizadas.Text = resumen.Realizadas.ToString(cultura);
            litInactivas.Text = resumen.Inactivas.ToString(cultura);
            litPromedio.Text = resumen.PromedioCalificacion.HasValue
                ? resumen.PromedioCalificacion.Value.ToString("0.0", cultura)
                : "—";
        }

        public void MostrarReportePorTipo(IList<ReporteTipoDto> filas)
        {
            gvPorTipo.DataSource = filas;
            gvPorTipo.DataBind();
        }

        public void MostrarReportePorActividad(IList<ReporteActividadDto> filas)
        {
            gvPorActividad.DataSource = filas;
            gvPorActividad.DataBind();
        }

        public void MostrarError(string mensaje)
        {
            lblMensaje.Text = Server.HtmlEncode(mensaje);
            lblMensaje.Visible = true;
        }
    }
}
