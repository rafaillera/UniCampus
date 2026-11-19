using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Seguridad;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>
    /// Vista (MVP) de las inscripciones del estudiante. Solo para estudiantes.
    /// </summary>
    public partial class MisInscripciones : PaginaBase, IMisInscripcionesView
    {
        private MisInscripcionesPresenter _presenter;

        protected override string[] RolesPermitidos
        {
            get { return new[] { Roles.Estudiante }; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new MisInscripcionesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Cargar();
            }
        }

        protected void gvInscripciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Cancelar") return;

            int actividadId;
            if (int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out actividadId))
            {
                _presenter.Cancelar(actividadId);
            }
        }

        public void MostrarInscripciones(IList<InscripcionDto> inscripciones)
        {
            gvInscripciones.DataSource = inscripciones;
            gvInscripciones.DataBind();
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
