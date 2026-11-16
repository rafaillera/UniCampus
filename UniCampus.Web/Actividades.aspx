<%@ Page Title="UniCampus - Gestión de actividades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Actividades.aspx.cs" Inherits="UniCampus.Web.Actividades" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="encabezado-pagina">
        <h1>Registro de Actividades Académicas</h1>
        <p>Administra, registra y consulta las actividades académicas de la institución.</p>
    </div>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" />

    <section class="tarjeta">
        <h2><asp:Literal ID="litTituloFormulario" runat="server" Text="Nueva actividad académica" /></h2>

        <%-- Id de la actividad en edición (vacío = registro nuevo) --%>
        <asp:HiddenField ID="hfActividadId" runat="server" />

        <div class="rejilla">
            <div class="campo campo-ancho">
                <asp:Label runat="server" AssociatedControlID="txtTitulo" Text="Título *" />
                <asp:TextBox ID="txtTitulo" runat="server" MaxLength="150" placeholder="Ej. Taller de Programación Web" />
            </div>

            <div class="campo campo-ancho">
                <asp:Label runat="server" AssociatedControlID="txtDescripcion" Text="Descripción" />
                <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" MaxLength="1000" />
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="ddlTipo" Text="Tipo de actividad *" />
                <asp:DropDownList ID="ddlTipo" runat="server" />
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="ddlModalidad" Text="Modalidad *" />
                <asp:DropDownList ID="ddlModalidad" runat="server">
                    <asp:ListItem Text="Presencial" Value="Presencial" />
                    <asp:ListItem Text="Virtual" Value="Virtual" />
                    <asp:ListItem Text="Híbrida" Value="Híbrida" />
                </asp:DropDownList>
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="txtFechaInicio" Text="Fecha de inicio *" />
                <asp:TextBox ID="txtFechaInicio" runat="server" TextMode="Date" />
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="txtFechaFin" Text="Fecha de finalización *" />
                <asp:TextBox ID="txtFechaFin" runat="server" TextMode="Date" />
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="txtHoraInicio" Text="Hora de inicio" />
                <asp:TextBox ID="txtHoraInicio" runat="server" TextMode="Time" />
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="txtHoraFin" Text="Hora de finalización" />
                <asp:TextBox ID="txtHoraFin" runat="server" TextMode="Time" />
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="txtLugar" Text="Lugar o enlace *" />
                <asp:TextBox ID="txtLugar" runat="server" MaxLength="150" placeholder="Ej. Laboratorio 3" />
            </div>

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="txtCupos" Text="Cupos *" />
                <asp:TextBox ID="txtCupos" runat="server" TextMode="Number" placeholder="Ej. 30" />
            </div>

            <%-- Al editar, el estado no se cambia desde el formulario sino con los botones de la tabla --%>
            <asp:Panel ID="pnlEstado" runat="server" CssClass="campo">
                <asp:Label runat="server" AssociatedControlID="ddlEstado" Text="Estado" />
                <asp:DropDownList ID="ddlEstado" runat="server">
                    <asp:ListItem Text="Publicada" Value="Publicada" />
                    <asp:ListItem Text="Borrador" Value="Borrador" />
                </asp:DropDownList>
            </asp:Panel>
        </div>

        <p class="nota">Los campos marcados con * son obligatorios. Todas las validaciones se realizan en el servidor.</p>

        <div class="acciones">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar actividad" CssClass="boton" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar edición" CssClass="boton boton-secundario" OnClick="btnCancelar_Click" Visible="false" />
        </div>
    </section>

    <asp:Panel ID="pnlDetalle" runat="server" CssClass="tarjeta" Visible="false">
        <h2>Detalles de la actividad</h2>

        <asp:DetailsView ID="dvDetalle" runat="server" AutoGenerateRows="false" CssClass="tabla" GridLines="None">
            <Fields>
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" NullDisplayText="—" />
                <asp:BoundField DataField="FechaInicio" HeaderText="Fecha de inicio" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="FechaFin" HeaderText="Fecha de fin" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="HoraInicio" HeaderText="Hora de inicio" DataFormatString="{0:hh\:mm}" NullDisplayText="—" />
                <asp:BoundField DataField="HoraFin" HeaderText="Hora de fin" DataFormatString="{0:hh\:mm}" NullDisplayText="—" />
                <asp:BoundField DataField="Modalidad" HeaderText="Modalidad" />
                <asp:BoundField DataField="Lugar" HeaderText="Lugar" />
                <asp:BoundField DataField="Cupos" HeaderText="Cupos totales" />
                <asp:BoundField DataField="Inscritos" HeaderText="Inscritos" />
                <asp:BoundField DataField="CuposDisponibles" HeaderText="Cupos disponibles" />
                <asp:BoundField DataField="Estado" HeaderText="Estado" />
            </Fields>
        </asp:DetailsView>

        <div class="acciones">
            <asp:HyperLink ID="lnkVerInscritos" runat="server" CssClass="boton boton-secundario" Text="Ver página de la actividad e inscritos" />
            <asp:Button ID="btnCerrarDetalle" runat="server" Text="Cerrar" CssClass="boton boton-secundario" OnClick="btnCerrarDetalle_Click" />
        </div>
    </asp:Panel>

    <section class="tarjeta">
        <h2>Listado de actividades académicas</h2>

        <div class="tabla-contenedor">
            <asp:GridView ID="gvActividades" runat="server"
                AutoGenerateColumns="false"
                CssClass="tabla"
                GridLines="None"
                DataKeyNames="ActividadId"
                OnRowCommand="gvActividades_RowCommand"
                EmptyDataText="No hay actividades registradas."
                EmptyDataRowStyle-CssClass="vacio">
                <Columns>
                    <asp:BoundField DataField="Titulo" HeaderText="Título" />
                    <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                    <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Modalidad" HeaderText="Modalidad" />
                    <asp:TemplateField HeaderText="Inscritos / Cupos" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero">
                        <ItemTemplate><%#: Eval("Inscritos") %> / <%#: Eval("Cupos") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <span class="badge badge-<%#: Eval("Estado") %>"><%#: Eval("Estado") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="acciones-fila">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Ver" CommandArgument='<%# Eval("ActividadId") %>' Text="Ver" />
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("ActividadId") %>' Text="Editar" />
                            <asp:LinkButton runat="server" CommandName="Publicar" CommandArgument='<%# Eval("ActividadId") %>' Text="Publicar"
                                Visible='<%# PuedePublicar(Eval("Estado")) %>' />
                            <asp:LinkButton runat="server" CommandName="Desactivar" CommandArgument='<%# Eval("ActividadId") %>' Text="Desactivar"
                                CssClass="peligro" Visible='<%# PuedeDesactivar(Eval("Estado")) %>'
                                OnClientClick="return confirm('¿Desactivar esta actividad? Dejará de aparecer en el catálogo.');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>
</asp:Content>
