
<%@ Page Title="Личный кабинет - Загрузить файлы" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Cabinet_UploadFiles.aspx.cs" Inherits="IngaProject.Account.Cabinet_UploadFiles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <h2><%: Title %>.</h2>
    <div>
        <h4>Добавить документ:</h4>
        <asp:FileUpload ID="FileUpload1" runat="server" accept ="image/*, text/*" />
        <p><br>
            <asp:Button ID="Button1" runat="server" OnClick="UploadInDB" Text="Загрузить"  class="btn btn-default" />
            <asp:Label ID="UploadStatusLabel" runat="server"></asp:Label>
        </p>
        <br>
        <h4>Название нового документа:</h4>
        <asp:TextBox ID="fileName" runat="server" placeholder="Название" class="form-control"></asp:TextBox><asp:Label ID="NameStatusLabel" runat="server" ForeColor="#CC0000"></asp:Label>
    </div>
            
    <br>
    <div>
        <h4>Загруженные документы:</h4>
        <%
            foreach (IngaProject.Models.SourceFile file in UserFiles())
            {
                Response.Write(String.Format(@"
                        <div class='thumbnail'>
                            <h5>{0}</h5>
                            
                        </div>",
                    file.Name));
            }
            %>
            <br>
        <asp:Button ID="Button2" runat="server" OnClick="UploadInProg" Text="Отправить на сборку" data-loading-text=«Загрузка…» class="btn btn-primary"/> &nbsp; <asp:Button ID="Button3" runat="server" OnClick="DelAllFileInDB" Text="Очистить"  class="btn btn-default" />
        <asp:Label ID="InProgramUploadStatusLabel" runat="server"></asp:Label>
        <br/>
        <asp:Label ID="Label1" runat="server"></asp:Label>
    </div>
</asp:Content>
