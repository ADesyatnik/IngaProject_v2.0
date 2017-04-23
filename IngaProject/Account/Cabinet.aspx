﻿<%@ Page Title="Личный кабинет" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cabinet.aspx.cs" Inherits="IngaProject.Account.Cabinet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
       <h2><%: Title %>.</h2>
        <div class ="container">
                    <h4>Ваши последние документы:</h4>
               
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
                    <a href="Cabinet_AllFiles">Все файлы</a>
            </div>
    <a href ="Cabinet_UploadFiles">Загрузить файлы на компиляцию</a>        

</asp:Content>
