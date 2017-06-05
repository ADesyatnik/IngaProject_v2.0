using IngaProject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        public IEnumerable<SourceFile> UserFiles() //выборка файлов конкретного юзера
        {
            using (var DB = new ApplicationDbContext())
            {
                var query = from SourceFile in DB.SourceFiles
                            where SourceFile.User == User.Identity.Name
                            select SourceFile;

                foreach (var file in query)
                {
                    yield return file;
                }
            }

        }

        protected void UploadInDB(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string FileType = Path.GetExtension(Server.HtmlEncode(FileUpload1.FileName));

                if (FileUpload1.PostedFile.ContentLength < 3100000)
                {
                    string appPath = Request.PhysicalApplicationPath + "Uploads\\" + User.Identity.Name.ToString();
                    Directory.CreateDirectory(appPath);

                    string savePath = appPath + "\\" + Server.HtmlEncode(FileUpload1.FileName);
                    FileUpload1.SaveAs(savePath); //сохранение по адресу savePath

                    SourceFile NewFile = new SourceFile() { Name = FileUpload1.FileName, Repository = appPath, User = Context.User.Identity.Name }; //формирование строки в бд
                    using (var DB = new ApplicationDbContext())
                    {
                        DB.SourceFiles.Add(NewFile);
                        DB.SaveChanges(); //добавление объекта и сохранение изменений в бд
                    }
                    UploadStatusLabel.CssClass = "label label-success";
                    UploadStatusLabel.Text = "Файл успешно загружен";
                }
                else
                {
                    UploadStatusLabel.CssClass = "label label-danger";
                    UploadStatusLabel.Text = "Файл не может быть загружен: превышен допустимый размер файла.";
                }
            }
        }

        //protected string InLatexAsync(object sender, EventArgs e)
        //{
            
        //}


        protected void UploadInProg(object sender, EventArgs e)
        {
            string appFolder = Request.PhysicalApplicationPath + "Uploads\\" + User.Identity.Name.ToString();
            string appPath = appFolder + "\\main.tex";

            if (File.Exists(appPath)) //существует ли указанный файл
            {
                InProgramUploadStatusLabel.Text = "Происходит загрузка, подождите";

                //Выход во внешнюю программу
                string path = appFolder + "\\start.bat";

                if (File.Exists(path)) // удалить файл, если он вдруг существует
                    File.Delete(path);

                using (FileStream fs = File.Create(path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("latexmk -pdf main.tex");
                    fs.Write(info, 0, info.Length);
                }

                Process p = new Process();
                p.StartInfo.FileName = appFolder + "\\start.bat";
                p.Start();

                Thread.Sleep(7000);
                //конец

                string pdfPath = appFolder + "\\main.pdf";
                string readyFileTrack = Request.PhysicalApplicationPath + "CompleteFile\\" + User.Identity.Name.ToString() + "_" + DateTime.Now + ".pdf";
                if (File.Exists(pdfPath))
                {
                    //перенос файла в папку CompleteFile
                    File.Move(pdfPath, readyFileTrack);

                    //формирование строки в бд
                    ReadyFile NewFile = new ReadyFile() { Name = FileUpload1.FileName, UrlFile = readyFileTrack, User = Context.User.Identity.Name, Date = DateTime.Now };
                    using (var DB = new ApplicationDbContext())
                    {
                        DB.ReadyFiles.Add(NewFile);
                        DB.SaveChanges();
                    }

                    //Удаление директории пользователя после загрузки
                    DelAllFileInDB(sender, e);

                    InProgramUploadStatusLabel.Text = @"Документ готов, <a href = '" + NewFile.UrlFile +"'>скачать</a>";
                }
            }
            else
            {
                InProgramUploadStatusLabel.Text = "Для продолжения необходим документ с названием main.tex!";
            }
        }


        protected void DelAllFileInDB(object sender, EventArgs e)
        {
            try
            {
                string appPath = Request.PhysicalApplicationPath + "Uploads\\" + User.Identity.Name.ToString();
                Directory.Delete(appPath, true); //удаление папки

                using (var DB = new ApplicationDbContext())//удаление записей из базы
                {
                    var query = from SourceFile in DB.SourceFiles
                                where SourceFile.User == User.Identity.Name
                                select SourceFile;

                    foreach (SourceFile file in query)
                    {
                        DB.SourceFiles.Remove(file);
                    }
                    DB.SaveChanges();
                }
            }
            catch (Exception ex) { Label1.Text = ex.Message; }
        }
    }
}