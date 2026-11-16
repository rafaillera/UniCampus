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
    /// Vista (MVP) del catálogo. Cualquier usuario autenticado puede abrirla.
    /// </summary>
    public partial class Catalogo : PaginaBase, ICatalogoView
    {
        private const int LargoResumen = 120;

        private CatalogoPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new CatalogoPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Cargar();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            _presenter.Buscar();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            _presenter.Limpiar();
        }

        // ---------------------------------------------------------------------
        // Implementación de ICatalogoView
        // ---------------------------------------------------------------------

        public FiltroActividadDto Filtro
        {
            get
            {
                int tipoId;
                return new FiltroActividadDto
                {
                    Texto = txtBuscar.Text,
                    TipoId = int.TryParse(ddlTipo.SelectedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out tipoId) && tipoId > 0
                        ? tipoId
                        : (int?)null,
                    Modalidad = ddlModalidad.SelectedValue,
                    FechaDesde = LeerFecha(txtDesde.Text),
                    FechaHasta = LeerFecha(txtHasta.Text),
                    SoloConCupos = chkConCupos.Checked
                };
            }
        }

        public void MostrarTipos(IList<TipoActividadDto> tipos)
        {
            ddlTipo.Items.Clear();
            ddlTipo.Items.Add(new ListItem("Todos", "0"));

            foreach (TipoActividadDto tipo in tipos)
            {
                ddlTipo.Items.Add(new ListItem(tipo.Nombre, tipo.TipoId.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public void MostrarActividades(IList<ActividadDto> actividades)
        {
            rptActividades.DataSource = actividades;
            rptActividades.DataBind();

            pnlSinResultados.Visible = actividades.Count == 0;
            litResultado.Text = actividades.Count == 1
                ? "1 actividad encontrada."
                : actividades.Count + " actividades encontradas.";
        }

        public void MostrarError(string mensaje)
        {
            lblMensaje.Text = Server.HtmlEncode(mensaje);
            lblMensaje.Visible = true;
        }

        public void LimpiarFiltros()
        {
            txtBuscar.Text = string.Empty;
            txtDesde.Text = string.Empty;
            txtHasta.Text = string.Empty;
            ddlTipo.SelectedIndex = 0;
            ddlModalidad.SelectedIndex = 0;
            chkConCupos.Checked = false;
        }

        // ---------------------------------------------------------------------
        // Auxiliares de formato usados en el Repeater (solo presentación)
        // ---------------------------------------------------------------------

        protected string Resumir(object descripcion)
        {
            string texto = Convert.ToString(descripcion);
            return texto.Length <= LargoResumen ? texto : texto.Substring(0, LargoResumen) + "…";
        }

        protected string Horario(object horaInicio, object horaFin)
        {
            if (!(horaInicio is TimeSpan)) return "Horario por confirmar";

            string inicio = ((TimeSpan)horaInicio).ToString(@"hh\:mm");
            return horaFin is TimeSpan ? inicio + " - " + ((TimeSpan)horaFin).ToString(@"hh\:mm") : inicio;
        }

        protected string ClaseCupos(object cuposDisponibles)
        {
            return Convert.ToInt32(cuposDisponibles, CultureInfo.InvariantCulture) == 0 ? "cupos-agotados" : string.Empty;
        }

        /// <summary>Un TextBox con TextMode="Date" envía la fecha en formato yyyy-MM-dd.</summary>
        private static DateTime? LeerFecha(string texto)
        {
            DateTime fecha;
            return DateTime.TryParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha)
                ? fecha
                : (DateTime?)null;
        }
    }
}
