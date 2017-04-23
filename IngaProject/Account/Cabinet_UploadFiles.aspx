﻿<%@ Page Title="Личный кабинет - Загрузить файлы" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cabinet_UploadFiles.aspx.cs" Inherits="IngaProject.Account.Cabinet_UploadFiles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <h2><%: Title %>.</h2>
    <div>
        <h4>Добавить документ:</h4>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <p><br>
            <asp:Button ID="Button1" runat="server" OnClick="UploadInDB" Text="Загрузить"  class="btn btn-default" />
            <asp:Label ID="UploadStatusLabel" runat="server"></asp:Label>
        </p>
    </div>
    <br>
    <div>
        <h4>Загруженные документы:</h4>
        <%
            foreach (IngaProject.Models.SourceFile file in UserFiles())
            {
                Response.Write(String.Format(@"
                        <div class='item'>
                            <h5>{0}</h5>
                            <asp:Button ID='But2' runat='server' OnClick='DelFile(file)' Text='Отправить на сборку' Width='165px' />
                        </div>",
                    file.Name));
            }
            %>
            
        <asp:Button ID="Button2" runat="server" OnClick="UploadInProg" Text="Отправить на сборку" data-loading-text=«Загрузка…» class="btn btn-primary"/>
        <asp:Label ID="InProgramUploadStatusLabel" runat="server"></asp:Label>
    </div>
</asp:Content>