<%@ Page Title="UniCampus - Gestión de actividades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Actividades.aspx.cs" Inherits="UniCampus.Web.Actividades" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="encabezado-pagina">
        <h1>Registro de Actividades Académicas</h1>
        <p>Administra, registra y consulta las actividades académicas de la institución.</p>
    </div>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" />

    <section class="tarjeta">
        <h2>Nueva actividad académica</h2>

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

            <div class="campo">
                <asp:Label runat="server" AssociatedControlID="ddlEstado" Text="Estado" />
                <asp:DropDownList ID="ddlEstado" runat="server">
                    <asp:ListItem Text="Publicada" Value="Publicada" />
                    <asp:ListItem Text="Borrador" Value="Borrador" />
                </asp:DropDownList>
            </div>
        </div>

        <p class="nota">Los campos marcados con * son obligatorios. Todas las validaciones se realizan en el servidor.</p>

        <div class="acciones">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar actividad" CssClass="boton" OnClick="btnGuardar_Click" />
        </div>
    </section>

    <section class="tarjeta">
        <h2>Actividades publicadas</h2>

        <asp:GridView ID="gvActividades" runat="server"
            AutoGenerateColumns="false"
            CssClass="tabla"
            GridLines="None"
            DataKeyNames="ActividadId"
            EmptyDataText="No hay actividades publicadas en este momento."
            EmptyDataRowStyle-CssClass="vacio">
            <Columns>
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="FechaFin" HeaderText="Fin" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="HoraInicio" HeaderText="Hora" DataFormatString="{0:hh\:mm}" NullDisplayText="—" />
                <asp:BoundField DataField="Modalidad" HeaderText="Modalidad" />
                <asp:BoundField DataField="Lugar" HeaderText="Lugar" />
                <asp:BoundField DataField="Cupos" HeaderText="Cupos" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                <asp:BoundField DataField="Estado" HeaderText="Estado" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
