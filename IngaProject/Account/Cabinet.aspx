<%@ Page Title="Личный кабинет" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cabinet.aspx.cs" Inherits="IngaProject.Account.Cabinet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
       <h2><%: Title %>.</h2>
    <div>
        <h4>Добавить документ:</h4>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <p><br>
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Загрузить" Width="165px" />
            <asp:Label ID="UploadStatusLabel" runat="server"></asp:Label>
        </p>
    </div>
    <div>
        <h4>Загруженные документы:</h4>
        <%
                foreach (IngaProject.Models.SourceFile file in UserFiles())
                {
                    Response.Write(String.Format(@"
                        <div class='item'>
                            <h5>{0}</h5>
                            <a href='{1}'>Скачать</a>
                        </div>", 
                        file.Name, file.UrlFile));
                }
            %>
    </div>

</asp:Content>
