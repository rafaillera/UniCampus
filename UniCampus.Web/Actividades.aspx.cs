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
    /// Vista (MVP). Implementa IActividadesView: solo lee y escribe controles
    /// y delega toda acción al Presenter. Aquí no hay SQL ni reglas de negocio.
    /// Solo pueden abrirla organizadores y administradores.
    /// </summary>
    public partial class Actividades : PaginaBase, IActividadesView
    {
        private ActividadesPresenter _presenter;

        protected override string[] RolesPermitidos
        {
            get { return Roles.Gestores; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ActividadesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Cargar();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            ActividadDto dto = ConstruirDto();
            _presenter.GuardarActividad(dto);
        }

        // ---------------------------------------------------------------------
        // Implementación de IActividadesView
        // ---------------------------------------------------------------------

        public void MostrarActividades(IList<ActividadDto> actividades)
        {
            gvActividades.DataSource = actividades;
            gvActividades.DataBind();
        }

        public void MostrarMensaje(string mensaje)
        {
            MostrarAlerta(mensaje, "alerta alerta-exito");
        }

        public void MostrarError(string mensaje)
        {
            MostrarAlerta(mensaje, "alerta alerta-error");
        }

        public void MostrarTipos(IList<TipoActividadDto> tipos)
        {
            ddlTipo.Items.Clear();
            ddlTipo.Items.Add(new ListItem("-- Seleccione --", "0"));

            foreach (TipoActividadDto tipo in tipos)
            {
                ddlTipo.Items.Add(new ListItem(tipo.Nombre, tipo.TipoId.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public void LimpiarFormulario()
        {
            txtTitulo.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            txtFechaInicio.Text = string.Empty;
            txtFechaFin.Text = string.Empty;
            txtHoraInicio.Text = string.Empty;
            txtHoraFin.Text = string.Empty;
            txtLugar.Text = string.Empty;
            txtCupos.Text = string.Empty;
            ddlTipo.SelectedIndex = 0;
            ddlModalidad.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
        }

        // ---------------------------------------------------------------------
        // Auxiliares de la Vista: solo convierten el texto de los controles
        // al DTO. Las validaciones de negocio se hacen en la capa BLL.
        // ---------------------------------------------------------------------

        private ActividadDto ConstruirDto()
        {
            return new ActividadDto
            {
                Titulo = txtTitulo.Text,
                Descripcion = txtDescripcion.Text,
                TipoId = LeerEntero(ddlTipo.SelectedValue),
                FechaInicio = LeerFecha(txtFechaInicio.Text),
                FechaFin = LeerFecha(txtFechaFin.Text),
                HoraInicio = LeerHora(txtHoraInicio.Text),
                HoraFin = LeerHora(txtHoraFin.Text),
                Modalidad = ddlModalidad.SelectedValue,
                Lugar = txtLugar.Text,
                Cupos = LeerEntero(txtCupos.Text),
                Estado = ddlEstado.SelectedValue
            };
        }

        private void MostrarAlerta(string mensaje, string cssClass)
        {
            // Server.HtmlEncode evita que un texto ingresado por el usuario
            // (por ejemplo, el título) se interprete como HTML/JavaScript (XSS).
            lblMensaje.Text = Server.HtmlEncode(mensaje);
            lblMensaje.CssClass = cssClass;
            lblMensaje.Visible = true;
        }

        private static int LeerEntero(string texto)
        {
            int valor;
            return int.TryParse(texto, NumberStyles.Integer, CultureInfo.InvariantCulture, out valor) ? valor : 0;
        }

        /// <summary>Un TextBox con TextMode="Date" envía la fecha en formato yyyy-MM-dd.</summary>
        private static DateTime LeerFecha(string texto)
        {
            DateTime fecha;
            return DateTime.TryParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha)
                ? fecha
                : DateTime.MinValue;
        }

        /// <summary>Un TextBox con TextMode="Time" envía la hora en formato HH:mm.</summary>
        private static TimeSpan? LeerHora(string texto)
        {
            TimeSpan hora;
            return TimeSpan.TryParse(texto, CultureInfo.InvariantCulture, out hora) ? hora : (TimeSpan?)null;
        }
    }
}
