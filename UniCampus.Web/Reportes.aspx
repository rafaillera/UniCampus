<%@ Page Title="UniCampus - Reportes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="UniCampus.Web.Reportes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="encabezado-pagina">
        <h1>Reportes</h1>
        <p>Resumen de actividades, inscripciones y valoraciones.</p>
    </div>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alerta alerta-error" />

    <div class="resumen">
        <div class="resumen-item">
            <strong><asp:Literal ID="litTotal" runat="server" Text="0" /></strong>
            <span>Total de actividades</span>
        </div>
        <div class="resumen-item resumen-verde">
            <strong><asp:Literal ID="litInscripciones" runat="server" Text="0" /></strong>
            <span>Inscripciones vigentes</span>
        </div>
        <div class="resumen-item resumen-ambar">
            <strong><asp:Literal ID="litProximas" runat="server" Text="0" /></strong>
            <span>Actividades próximas</span>
        </div>
        <div class="resumen-item">
            <strong><asp:Literal ID="litRealizadas" runat="server" Text="0" /></strong>
            <span>Actividades realizadas</span>
        </div>
        <div class="resumen-item resumen-rojo">
            <strong><asp:Literal ID="litInactivas" runat="server" Text="0" /></strong>
            <span>Inactivas o canceladas</span>
        </div>
        <div class="resumen-item">
            <strong><asp:Literal ID="litPromedio" runat="server" Text="—" /></strong>
            <span>Calificación promedio (1 a 5)</span>
        </div>
    </div>

    <section class="tarjeta">
        <h2>Actividades por tipo</h2>
        <div class="tabla-contenedor">
            <asp:GridView ID="gvPorTipo" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                EmptyDataText="No hay tipos de actividad registrados." EmptyDataRowStyle-CssClass="vacio">
                <Columns>
                    <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                    <asp:BoundField DataField="Actividades" HeaderText="Actividades" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                    <asp:BoundField DataField="Cupos" HeaderText="Cupos totales" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                    <asp:BoundField DataField="Inscritos" HeaderText="Inscritos" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                    <asp:BoundField DataField="PorcentajeOcupacion" HeaderText="Ocupación" DataFormatString="{0:0.#} %" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <section class="tarjeta">
        <h2>Ocupación y valoración por actividad</h2>
        <div class="tabla-contenedor">
            <asp:GridView ID="gvPorActividad" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                EmptyDataText="No hay actividades registradas." EmptyDataRowStyle-CssClass="vacio">
                <Columns>
                    <asp:BoundField DataField="Titulo" HeaderText="Actividad" />
                    <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                    <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate><span class="badge badge-<%#: Eval("Estado") %>"><%#: Eval("Estado") %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Inscritos" HeaderText="Inscritos" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                    <asp:BoundField DataField="Cupos" HeaderText="Cupos" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                    <asp:BoundField DataField="PorcentajeOcupacion" HeaderText="Ocupación" DataFormatString="{0:0.#} %" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                    <asp:BoundField DataField="TotalResenas" HeaderText="Reseñas" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                    <asp:BoundField DataField="PromedioCalificacion" HeaderText="Promedio" DataFormatString="{0:0.0}" NullDisplayText="—" ItemStyle-CssClass="numero" HeaderStyle-CssClass="numero" />
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <div class="acciones">
        <button type="button" class="boton boton-secundario" onclick="window.print();">Imprimir reporte</button>
    </div>
</asp:Content>
