using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Seguridad;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>
    /// Vista (MVP) del detalle de una actividad. La abre cualquier usuario autenticado:
    /// el estudiante ve los botones de inscripción y el organizador ve la lista de inscritos.
    /// </summary>
    public partial class DetalleActividad : PaginaBase, IDetalleActividadView
    {
        private DetalleActividadPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new DetalleActividadPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Cargar();
            }
        }

        protected void btnInscribirme_Click(object sender, EventArgs e)
        {
            _presenter.Inscribirse();
        }

        protected void btnCancelarInscripcion_Click(object sender, EventArgs e)
        {
            _presenter.CancelarInscripcion();
        }

        // ---------------------------------------------------------------------
        // Implementación de IDetalleActividadView
        // ---------------------------------------------------------------------

        public int ActividadId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], NumberStyles.Integer, CultureInfo.InvariantCulture, out id) && id > 0
                    ? id
                    : 0;
            }
        }

        public void MostrarActividad(ActividadDto actividad)
        {
            // Literal no codifica HTML: todo texto que venga de la BD pasa por HtmlEncode.
            litTitulo.Text = Server.HtmlEncode(actividad.Titulo);
            litTipo.Text = Server.HtmlEncode(actividad.TipoNombre);
            lblEstado.Text = Server.HtmlEncode(actividad.Estado);
            lblEstado.CssClass = "badge badge-" + actividad.Estado;
            litFechas.Text = actividad.FechaInicio.Date == actividad.FechaFin.Date
                ? actividad.FechaInicio.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                : actividad.FechaInicio.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + actividad.FechaFin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            litHorario.Text = actividad.HoraInicio.HasValue
                ? actividad.HoraInicio.Value.ToString(@"hh\:mm") + (actividad.HoraFin.HasValue ? " - " + actividad.HoraFin.Value.ToString(@"hh\:mm") : string.Empty)
                : "Por confirmar";
            litModalidad.Text = Server.HtmlEncode(actividad.Modalidad);
            litLugar.Text = Server.HtmlEncode(actividad.Lugar);
            lblCupos.Text = "Cupos disponibles: " + actividad.CuposDisponibles + " de " + actividad.Cupos;
            lblCupos.CssClass = actividad.CuposDisponibles == 0 ? "cupos cupos-agotados" : "cupos";
            litDescripcion.Text = Server.HtmlEncode(actividad.Descripcion ?? string.Empty);
            lnkResenas.NavigateUrl = "~/Resenas.aspx?id=" + actividad.ActividadId.ToString(CultureInfo.InvariantCulture);

            pnlActividad.Visible = true;
            pnlNoEncontrada.Visible = false;
        }

        public void MostrarNoEncontrada()
        {
            pnlActividad.Visible = false;
            pnlNoEncontrada.Visible = true;
        }

        public void MostrarOpcionesInscripcion(OpcionesInscripcionDto opciones)
        {
            btnInscribirme.Visible = opciones.PuedeInscribirse;
            btnCancelarInscripcion.Visible = opciones.PuedeCancelar;
            lblEstadoInscripcion.Visible = !string.IsNullOrEmpty(opciones.Mensaje);
            lblEstadoInscripcion.Text = Server.HtmlEncode(opciones.Mensaje ?? string.Empty);
        }

        public void MostrarInscritos(IList<InscripcionDto> inscritos)
        {
            pnlInscritos.Visible = inscritos != null;
            if (inscritos == null) return;

            litTotalInscritos.Text = inscritos.Count(i => i.Estado != EstadosInscripcion.Cancelada).ToString(CultureInfo.InvariantCulture);
            gvInscritos.DataSource = inscritos;
            gvInscritos.DataBind();
        }

        public void MostrarMensaje(string mensaje)
        {
            MostrarAlerta(mensaje, "alerta alerta-exito");
        }

        public void MostrarError(string mensaje)
        {
            MostrarAlerta(mensaje, "alerta alerta-error");
        }

        private void MostrarAlerta(string mensaje, string cssClass)
        {
            lblMensaje.Text = Server.HtmlEncode(mensaje);
            lblMensaje.CssClass = cssClass;
            lblMensaje.Visible = true;
        }
    }
}
