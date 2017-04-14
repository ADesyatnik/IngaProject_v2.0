using IngaProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IngaProject.Account
{
    public partial class Cabinet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Context.User.Identity.IsAuthenticated)
                Response.Redirect("~/Default.aspx");
        }

        public IEnumerable<SourceFile> DBFile()  
        {
            var DB = new ApplicationDbContext();
            return DB.SourceFiles;
        }

        public IEnumerable<SourceFile> UserFiles() //выборка файлов конкретного юзера
        {
            IEnumerable<SourceFile> Files = DBFile();
            foreach (var file in Files)
            {
                if (file.User == Context.User.Identity.Name)
                    yield return file;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
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
                        UploadStatusLabel.Text = "Файл успешно загружен";
                    }
                    else
                        UploadStatusLabel.Text = "Файл не может быть загружен: превышен допустимый размер файла!";
                }
                else
                {
                    UploadStatusLabel.Text = "Выбраный тип файла не может быть загружен!";
                }
            }
        }
    }
}