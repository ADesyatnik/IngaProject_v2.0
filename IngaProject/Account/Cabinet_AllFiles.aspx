<%@ Page Title="Личный кабинет - Все файлы" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cabinet_AllFiles.aspx.cs" Inherits="IngaProject.Account.Cabinet" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h4>Ваши документы:</h4>     
        <%
            var i = 0;
            foreach (IngaProject.Models.ReadyFile file in DBFile())
            {
                Response.Write(String.Format(@"
                            <div class='thumbnail'>
                                <h5>&nbsp;<a href='{2}'>{0}</a> &nbsp; <small>{1}</small></h5>
                            </div>",
                    file.Name, file.Date, file.UrlFile));
                i++;
            }
            if (i == 0)
                Response.Write(String.Format(@"Здесь пока ничего нет! Но Вы можете работу с сервисом прямо сейчас: <a href='TemplatesDirectory'>скачайте шаблон</a> или <a href ='Cabinet_UploadFiles'>загрузите Вашу работу</a>  (´• ω •`)ﾉ"));
            %> 
</asp:Content>
