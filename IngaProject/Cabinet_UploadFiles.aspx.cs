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

        protected void UploadInProg(object sender, EventArgs e)
        {
            if (fileName.Text.Count() == 0)
            {
                NameStatusLabel.Text = "Это необходимое поле!";
                fileName.BorderColor = System.Drawing.Color.DarkRed;
            }
            else
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


                    // Create the ProcessInfo object
                    var psi = new ProcessStartInfo("cmd.exe")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = Request.PhysicalApplicationPath + "Uploads\\" + User.Identity.Name.ToString() + "\\"
                    };

                    // Start the process
                    var proc = Process.Start(psi);
                    // Open the batch file for reading
                    var strm = File.OpenText(path);
                    // Attach the output for reading
                    if (proc != null)
                    {
                        var sOut = proc.StandardOutput;
                        // Attach the in for writing
                        var sIn = proc.StandardInput;
                        // Write each line of the batch file to standard input
                        while (strm.Peek() != -1)
                        {
                            sIn.WriteLine(strm.ReadLine());
                        }
                        strm.Close();

                        // Exit CMD.EXE
                        var stEchoFmt = "# {0} run successfully. Exiting";
                        sIn.WriteLine(stEchoFmt, path);
                        sIn.WriteLine("EXIT");
                        // Close the process
                        proc.Close();
                        // Read the sOut to a string.
                        var trim = sOut.ReadToEnd().Trim();
                        // Close the io Streams;
                        sIn.Close();
                        sOut.Close();
                    }

                    string pdfPath = appFolder + "\\main.pdf";
                    string file = fileName.Text + ".pdf";
                    string readyFileTrack = Request.PhysicalApplicationPath + "CompleteFile\\" + User.Identity.Name + "\\";
                    string url = "CompleteFile/" + User.Identity.Name + "/" + fileName.Text + ".pdf";

                    if (File.Exists(pdfPath))
                    {
                        if (!Directory.Exists(readyFileTrack))
                            Directory.CreateDirectory(readyFileTrack);

                        //перенос файла в папку CompleteFile
                        File.Copy(appFolder + "\\main.pdf", readyFileTrack + file);

                        //формирование строки в бд
                        ReadyFile newFile = new ReadyFile() { Name = file, UrlFile = url, User = Context.User.Identity.Name, Date = DateTime.Now };
                        using (var DB = new ApplicationDbContext())
                        {
                            DB.ReadyFiles.Add(newFile);
                            DB.SaveChanges();
                        }

                        //Удаление директории пользователя после загрузки
                        DelAllFileInDB(sender, e);

                        InProgramUploadStatusLabel.Text = @"Документ готов, <a href = '" + newFile.UrlFile + "'>скачать</a>";
                    }

                    else
                    {
                        InProgramUploadStatusLabel.Text = "Не получилось создать документ, проверьте файлы на ошибки :(";
                    }
                }
                else
                {
                    InProgramUploadStatusLabel.Text = "Для продолжения необходим документ с названием main.tex!";
                }
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