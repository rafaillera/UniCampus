<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UniCampus.Web.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>UniCampus - Iniciar sesión</title>
    <link href="Content/site.css" rel="stylesheet" />
</head>
<body class="pagina-login">
    <form id="form1" runat="server">
        <section class="tarjeta login" aria-labelledby="tituloLogin">
            <div class="login-marca">
                <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 3 1 9l11 6 9-4.91V17h2V9L12 3zm-6.18 9.5L12 16l6.18-3.5V16L12 19.5 5.82 16v-3.5z" /></svg>
                <div>
                    <h1 id="tituloLogin">UniCampus</h1>
                    <p>Sistema de Gestión Académica</p>
                </div>
            </div>

            <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alerta alerta-error" />

            <asp:Panel runat="server" DefaultButton="btnIngresar">
                <div class="campo">
                    <asp:Label runat="server" AssociatedControlID="txtCorreo" Text="Correo institucional" />
                    <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" MaxLength="150" placeholder="usuario@unicampus.edu.co" />
                </div>

                <div class="campo">
                    <asp:Label runat="server" AssociatedControlID="txtClave" Text="Contraseña" />
                    <asp:TextBox ID="txtClave" runat="server" TextMode="Password" MaxLength="100" />
                </div>

                <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="boton boton-bloque" OnClick="btnIngresar_Click" />
            </asp:Panel>

            <p class="nota">Los usuarios de prueba están en el script <code>BaseDatos/03_BaseDatos.sql</code> y en el README.</p>
        </section>
    </form>
</body>
</html>
