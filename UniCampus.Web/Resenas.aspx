<%@ Page Title="UniCampus - Reseñas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Resenas.aspx.cs" Inherits="UniCampus.Web.Resenas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <p><asp:HyperLink ID="lnkVolver" runat="server" NavigateUrl="~/Catalogo.aspx" Text="&larr; Volver a la actividad" /></p>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" />

    <asp:Panel ID="pnlNoEncontrada" runat="server" CssClass="tarjeta" Visible="false">
        <h2>Actividad no encontrada</h2>
        <p class="nota">La actividad que busca no existe o no está disponible.</p>
    </asp:Panel>

    <asp:Panel ID="pnlContenido" runat="server" Visible="false">
        <div class="encabezado-pagina">
            <h1>Reseñas: <asp:Literal ID="litTitulo" runat="server" /></h1>
            <p>
                <span class="estrellas"><asp:Literal ID="litEstrellasPromedio" runat="server" /></span>
                <asp:Literal ID="litPromedio" runat="server" />
            </p>
        </div>

        <div class="columnas">
            <section class="tarjeta">
                <h2>Opiniones de los estudiantes</h2>

                <asp:Repeater ID="rptResenas" runat="server">
                    <ItemTemplate>
                        <article class="resena">
                            <span class="estrellas" aria-label='<%#: Eval("Calificacion") + " de 5 estrellas" %>'><%#: Eval("Estrellas") %></span>
                            <small><%#: Eval("Autor") %> · <%#: Eval("FechaCreacion", "{0:dd/MM/yyyy}") %></small>
                            <%-- <%#: %> codifica el HTML: un comentario con <script> se muestra como texto --%>
                            <p><%#: Eval("Comentario") %></p>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlSinResenas" runat="server" Visible="false">
                    <p class="nota">Esta actividad todavía no tiene reseñas.</p>
                </asp:Panel>
            </section>

            <section class="tarjeta">
                <h2>Escribe tu reseña</h2>

                <asp:Label ID="lblMotivo" runat="server" CssClass="alerta alerta-info" Visible="false" />

                <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
                    <div class="campo">
                        <asp:Label runat="server" AssociatedControlID="rblCalificacion" Text="Calificación *" />
                        <asp:RadioButtonList ID="rblCalificacion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="calificacion-opciones">
                            <asp:ListItem Text="★ 1" Value="1" />
                            <asp:ListItem Text="★ 2" Value="2" />
                            <asp:ListItem Text="★ 3" Value="3" />
                            <asp:ListItem Text="★ 4" Value="4" />
                            <asp:ListItem Text="★ 5" Value="5" />
                        </asp:RadioButtonList>
                    </div>

                    <div class="campo">
                        <asp:Label runat="server" AssociatedControlID="txtComentario" Text="Comentario (opcional, máximo 1000 caracteres)" />
                        <%-- ValidateRequestMode="Disabled": ASP.NET no bloquea textos con "<" en este campo.
                             La protección anti-XSS está en la salida: el comentario se muestra con <%#: %> (HtmlEncode). --%>
                        <asp:TextBox ID="txtComentario" runat="server" TextMode="MultiLine" Rows="5" MaxLength="1000" ValidateRequestMode="Disabled" />
                    </div>

                    <asp:Button ID="btnPublicar" runat="server" Text="Publicar reseña" CssClass="boton" OnClick="btnPublicar_Click" />
                </asp:Panel>
            </section>
        </div>
    </asp:Panel>
</asp:Content>
