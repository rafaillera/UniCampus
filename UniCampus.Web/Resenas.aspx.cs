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
    /// Vista (MVP) de reseñas y valoraciones de una actividad.
    /// </summary>
    public partial class Resenas : PaginaBase, IResenasView
    {
        private ResenasPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ResenasPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Cargar();
            }
        }

        protected void btnPublicar_Click(object sender, EventArgs e)
        {
            _presenter.Publicar();
        }

        // ---------------------------------------------------------------------
        // Implementación de IResenasView
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

        public int Calificacion
        {
            get
            {
                int valor;
                return int.TryParse(rblCalificacion.SelectedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out valor) ? valor : 0;
            }
        }

        public string Comentario
        {
            get { return txtComentario.Text; }
        }

        public void MostrarActividad(ActividadDto actividad)
        {
            litTitulo.Text = Server.HtmlEncode(actividad.Titulo);
            lnkVolver.NavigateUrl = "~/DetalleActividad.aspx?id=" + actividad.ActividadId.ToString(CultureInfo.InvariantCulture);
            pnlContenido.Visible = true;
            pnlNoEncontrada.Visible = false;
        }

        public void MostrarNoEncontrada()
        {
            pnlContenido.Visible = false;
            pnlNoEncontrada.Visible = true;
        }

        public void MostrarResenas(IList<ResenaDto> resenas, decimal? promedio)
        {
            rptResenas.DataSource = resenas;
            rptResenas.DataBind();
            pnlSinResenas.Visible = resenas.Count == 0;

            if (promedio.HasValue)
            {
                int redondeado = (int)Math.Round(promedio.Value, MidpointRounding.AwayFromZero);
                litEstrellasPromedio.Text = new string('★', redondeado) + new string('☆', 5 - redondeado);
                litPromedio.Text = promedio.Value.ToString("0.0", CultureInfo.CurrentCulture)
                    + " de 5 (" + resenas.Count + (resenas.Count == 1 ? " reseña)" : " reseñas)");
            }
            else
            {
                litEstrellasPromedio.Text = string.Empty;
                litPromedio.Text = "Sin calificaciones todavía.";
            }
        }

        public void MostrarFormulario(PermisoResenaDto permiso)
        {
            pnlFormulario.Visible = permiso.Permitido;
            lblMotivo.Visible = !permiso.Permitido;
            lblMotivo.Text = Server.HtmlEncode(permiso.Motivo ?? string.Empty);
        }

        public void LimpiarFormulario()
        {
            rblCalificacion.ClearSelection();
            txtComentario.Text = string.Empty;
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
