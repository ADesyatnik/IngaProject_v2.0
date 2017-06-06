<%@ Page Title="Личный кабинет" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cabinet.aspx.cs" Inherits="IngaProject.Account.Cabinet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
       <h2><%: Title %>.</h2>
        
        <div class ="container">
            <p>Здравствуйте, <% Response.Write(String.Format(User.Identity.Name.ToString())); %>!</p>
            <h4>Ваши последние документы:</h4>
               
        <%
            var i = 0;
            foreach (IngaProject.Models.ReadyFile file in DBFile().Reverse())
            {
                Response.Write(String.Format(@"
                            <div class='thumbnail'>
                                <h5>&nbsp;<a href='{2}'>{0}</a> &nbsp; <small>{1}</small></h5>
                            </div>",
                        file.Name, file.Date, file.UrlFile));
                i++;
                if (i > 2)
                    break;
            }
            if (i == 0)
                Response.Write(String.Format(@"Здесь пока ничего нет! Но Вы можете работу с сервисом прямо сейчас: <a href='TemplatesDirectory'>скачайте шаблон</a> или <a href ='Cabinet_UploadFiles'>загрузите Вашу работу</a>  (´• ω •`)ﾉ"));
            else
                Response.Write(String.Format(@"<a href='Cabinet_AllFiles' class='btn btn-default'>Все файлы</a>"));
            %>
                   
        </div>
        <br>
        <a href ="Cabinet_UploadFiles" class="btn btn-primary">Создать новый »</a>   
    

</asp:Content>
