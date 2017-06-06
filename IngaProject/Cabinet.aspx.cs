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

        public IEnumerable<ReadyFile> DBFile()
        {
            using (var DB = new ApplicationDbContext())
            {
                IEnumerable<ReadyFile> Files = DB.ReadyFiles;
                foreach (var file in Files)
                {
                    if (file.User == Context.User.Identity.Name)
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}