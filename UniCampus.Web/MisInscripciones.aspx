<%@ Page Title="UniCampus - Mis inscripciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MisInscripciones.aspx.cs" Inherits="UniCampus.Web.MisInscripciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="encabezado-pagina">
        <h1>Mis inscripciones</h1>
        <p>Actividades en las que estás inscrito. Puedes cancelar antes del día de inicio.</p>
    </div>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" />

    <section class="tarjeta">
        <div class="tabla-contenedor">
            <asp:GridView ID="gvInscripciones" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                OnRowCommand="gvInscripciones_RowCommand"
                EmptyDataText="Todavía no te has inscrito en ninguna actividad. Revisa el catálogo."
                EmptyDataRowStyle-CssClass="vacio">
                <Columns>
                    <asp:TemplateField HeaderText="Actividad">
                        <ItemTemplate>
                            <a href="<%#: ResolveUrl("~/DetalleActividad.aspx?id=" + Eval("ActividadId")) %>"><%#: Eval("ActividadTitulo") %></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                    <asp:BoundField DataField="FechaInicio" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="HoraInicio" HeaderText="Hora" DataFormatString="{0:hh\:mm}" NullDisplayText="—" />
                    <asp:BoundField DataField="Lugar" HeaderText="Lugar" />
                    <asp:TemplateField HeaderText="Inscripción">
                        <ItemTemplate><span class="badge badge-<%#: Eval("Estado") %>"><%#: Eval("Estado") %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="acciones-fila">
                        <ItemTemplate>
                            <a href="<%#: ResolveUrl("~/Resenas.aspx?id=" + Eval("ActividadId")) %>">Reseñas</a>
                            <asp:LinkButton runat="server" CommandName="Cancelar" CommandArgument='<%# Eval("ActividadId") %>'
                                Text="Cancelar" CssClass="peligro" Visible='<%# Eval("PuedeCancelar") %>'
                                OnClientClick="return confirm('¿Cancelar tu inscripción? Liberarás tu cupo.');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>
</asp:Content>
