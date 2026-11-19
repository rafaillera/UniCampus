<%@ Page Title="UniCampus - Detalle de la actividad" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DetalleActividad.aspx.cs" Inherits="UniCampus.Web.DetalleActividad" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <p><a href="Catalogo.aspx">&larr; Volver al catálogo</a></p>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" />

    <asp:Panel ID="pnlNoEncontrada" runat="server" CssClass="tarjeta" Visible="false">
        <h2>Actividad no encontrada</h2>
        <p class="nota">La actividad que busca no existe o no está disponible.</p>
    </asp:Panel>

    <asp:Panel ID="pnlActividad" runat="server" Visible="false">
        <div class="columnas">
            <section class="tarjeta">
                <h2><asp:Literal ID="litTitulo" runat="server" /></h2>
                <p>
                    <span class="badge badge-tipo"><asp:Literal ID="litTipo" runat="server" /></span>
                    <asp:Label ID="lblEstado" runat="server" CssClass="badge" />
                </p>

                <dl class="detalle">
                    <dt>Fechas</dt>
                    <dd><asp:Literal ID="litFechas" runat="server" /></dd>
                    <dt>Horario</dt>
                    <dd><asp:Literal ID="litHorario" runat="server" /></dd>
                    <dt>Modalidad</dt>
                    <dd><asp:Literal ID="litModalidad" runat="server" /></dd>
                    <dt>Lugar</dt>
                    <dd><asp:Literal ID="litLugar" runat="server" /></dd>
                    <dt>Cupos</dt>
                    <dd><asp:Label ID="lblCupos" runat="server" CssClass="cupos" /></dd>
                </dl>

                <p><asp:Literal ID="litDescripcion" runat="server" /></p>

                <asp:Label ID="lblEstadoInscripcion" runat="server" CssClass="alerta alerta-info" Visible="false" />

                <div class="acciones">
                    <asp:Button ID="btnInscribirme" runat="server" Text="Inscribirme" CssClass="boton" OnClick="btnInscribirme_Click" Visible="false" />
                    <asp:Button ID="btnCancelarInscripcion" runat="server" Text="Cancelar mi inscripción" CssClass="boton boton-peligro" OnClick="btnCancelarInscripcion_Click" Visible="false"
                        OnClientClick="return confirm('¿Seguro que deseas cancelar tu inscripción? Liberarás tu cupo.');" />
                    <asp:HyperLink ID="lnkResenas" runat="server" CssClass="boton boton-secundario" Text="Ver reseñas" />
                </div>
            </section>

            <asp:Panel ID="pnlInscritos" runat="server" CssClass="tarjeta" Visible="false">
                <h2>Estudiantes inscritos (<asp:Literal ID="litTotalInscritos" runat="server" />)</h2>
                <div class="tabla-contenedor">
                    <asp:GridView ID="gvInscritos" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                        EmptyDataText="Aún no hay estudiantes inscritos." EmptyDataRowStyle-CssClass="vacio">
                        <Columns>
                            <asp:BoundField DataField="NombreEstudiante" HeaderText="Nombre" />
                            <asp:BoundField DataField="Programa" HeaderText="Programa" NullDisplayText="—" />
                            <asp:BoundField DataField="FechaInscripcion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate><span class="badge badge-<%#: Eval("Estado") %>"><%#: Eval("Estado") %></span></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>
</asp:Content>
