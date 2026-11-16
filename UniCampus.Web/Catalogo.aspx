<%@ Page Title="UniCampus - Catálogo de actividades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Catalogo.aspx.cs" Inherits="UniCampus.Web.Catalogo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="encabezado-pagina">
        <h1>Catálogo de actividades</h1>
        <p>Busca talleres, conferencias, seminarios y cursos, y consulta los cupos disponibles.</p>
    </div>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alerta alerta-error" />

    <section class="tarjeta">
        <asp:Panel runat="server" DefaultButton="btnBuscar">
            <div class="filtros">
                <div class="campo">
                    <asp:Label runat="server" AssociatedControlID="txtBuscar" Text="Buscar" />
                    <asp:TextBox ID="txtBuscar" runat="server" MaxLength="100" placeholder="Título, descripción o lugar" />
                </div>
                <div class="campo">
                    <asp:Label runat="server" AssociatedControlID="ddlTipo" Text="Tipo" />
                    <asp:DropDownList ID="ddlTipo" runat="server" />
                </div>
                <div class="campo">
                    <asp:Label runat="server" AssociatedControlID="ddlModalidad" Text="Modalidad" />
                    <asp:DropDownList ID="ddlModalidad" runat="server">
                        <asp:ListItem Text="Todas" Value="" />
                        <asp:ListItem Text="Presencial" Value="Presencial" />
                        <asp:ListItem Text="Virtual" Value="Virtual" />
                        <asp:ListItem Text="Híbrida" Value="Híbrida" />
                    </asp:DropDownList>
                </div>
                <div class="campo">
                    <asp:Label runat="server" AssociatedControlID="txtDesde" Text="Desde" />
                    <asp:TextBox ID="txtDesde" runat="server" TextMode="Date" />
                </div>
                <div class="campo">
                    <asp:Label runat="server" AssociatedControlID="txtHasta" Text="Hasta" />
                    <asp:TextBox ID="txtHasta" runat="server" TextMode="Date" />
                </div>
            </div>

            <div class="acciones">
                <asp:CheckBox ID="chkConCupos" runat="server" Text=" Solo con cupos disponibles" />
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="boton" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar filtros" CssClass="boton boton-secundario" OnClick="btnLimpiar_Click" />
            </div>
        </asp:Panel>
    </section>

    <p class="resultado-busqueda"><asp:Literal ID="litResultado" runat="server" /></p>

    <asp:Repeater ID="rptActividades" runat="server">
        <HeaderTemplate><div class="catalogo"></HeaderTemplate>
        <ItemTemplate>
            <article class="actividad-card">
                <div class="etiquetas">
                    <span class="badge badge-tipo"><%#: Eval("TipoNombre") %></span>
                    <span class="badge"><%#: Eval("Modalidad") %></span>
                </div>
                <h3><%#: Eval("Titulo") %></h3>
                <p><%#: Resumir(Eval("Descripcion")) %></p>
                <p><%#: Eval("FechaInicio", "{0:dd/MM/yyyy}") %> - <%#: Eval("FechaFin", "{0:dd/MM/yyyy}") %> · <%#: Horario(Eval("HoraInicio"), Eval("HoraFin")) %></p>
                <p><%#: Eval("Lugar") %></p>
                <p class="cupos <%# ClaseCupos(Eval("CuposDisponibles")) %>">
                    Cupos disponibles: <%#: Eval("CuposDisponibles") %> de <%#: Eval("Cupos") %>
                </p>
                <a class="boton" href="<%#: ResolveUrl("~/DetalleActividad.aspx?id=" + Eval("ActividadId")) %>">Ver detalle</a>
            </article>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>

    <asp:Panel ID="pnlSinResultados" runat="server" CssClass="tarjeta" Visible="false">
        <p class="nota">No se encontraron actividades publicadas con esos criterios.</p>
    </asp:Panel>
</asp:Content>
