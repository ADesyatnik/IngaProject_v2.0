<%@ Page Title="Личный кабинет - Все файлы" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cabinet_AllFiles.aspx.cs" Inherits="IngaProject.Account.Cabinet" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h4>Ваши документы:</h4>     
        <%
                foreach (IngaProject.Models.ReadyFile file in DBFile())
                {
                    Response.Write(String.Format(@"
                            <div>
                                <h5>{0}</h5>
                                <p>{1}</p>
                                <a href='{2}'>Скачать</a>
                            </div>", 
                        file.Name, file.Date, file.UrlFile));
                }
            %>
</asp:Content>
