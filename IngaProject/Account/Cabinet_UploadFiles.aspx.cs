﻿using IngaProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IngaProject.Account
{
    public partial class Cabinet_UploadFiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Context.User.Identity.IsAuthenticated)
                Response.Redirect("~/Default.aspx");
        }

        protected void UploadInDB(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string FileType = System.IO.Path.GetExtension(Server.HtmlEncode(FileUpload1.FileName));
                if ((FileType == ".doc") || (FileType == ".docx") || (FileType == ".txt"))
                {
                    if (FileUpload1.PostedFile.ContentLength < 3100000)
                    {
                        string appPath = Request.PhysicalApplicationPath;
                        string savePath = appPath + "Uploads\\" + Server.HtmlEncode(FileUpload1.FileName);
                        FileUpload1.SaveAs(savePath); //сохранение по адресу savePath
                        SourceFile NewFile = new SourceFile() { Name = FileUpload1.FileName, UrlFile = savePath, User = Context.User.Identity.Name }; //формирование строки в бд
                        var DB = new ApplicationDbContext();
                        DB.SourceFiles.Add(NewFile);
                        DB.SaveChanges(); //добавление объекта и сохранение изменений в бд
                        UploadStatusLabel.CssClass = "label label-success";
                        UploadStatusLabel.Text = "Файл успешно загружен";
                    }
                    else
                    {
                        UploadStatusLabel.CssClass = "label label-danger";
                        UploadStatusLabel.Text = "Файл не может быть загружен: превышен допустимый размер файла.";
                    }
                }
                else
                {
                    UploadStatusLabel.CssClass = "label label-danger";
                    UploadStatusLabel.Text = "Выбраный тип файла не может быть загружен.";
                }
            }
        }

        protected void UploadInProg(object sender, EventArgs e)
        {
            using (var DB = new ApplicationDbContext())
            {
                IEnumerable<SourceFile> Files = DB.SourceFiles;
                if (Files.Count() != 0)
                {
                    InProgramUploadStatusLabel.Text = "Происходит загрузка, подождите";
                    //Выход во внешнюю программу
                }
            }
        }

        public IEnumerable<SourceFile> UserFiles() //выборка файлов конкретного юзера
        {
            using (var DB = new ApplicationDbContext())
            {
                IEnumerable<SourceFile> Files = DB.SourceFiles;
                foreach (var file in Files)
                {
                    if (file.User == Context.User.Identity.Name)
                    {
                        yield return file;
                    }
                }
            }
            
        }

        public void DelFile(SourceFile file)
        {
            using (var DB = new ApplicationDbContext())
            {
                IEnumerable<SourceFile> Files = DB.SourceFiles;
                foreach (var f in Files)
                {
                    if (f.Id == file.Id)
                    {
                        DB.SourceFiles.Remove(file);
                        DB.SaveChanges();
                    }
                }
            }
        }
    }
}