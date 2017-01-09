using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using WaweUploadFileChallenge.Models;
using System.Web.Mvc;

namespace WaweUploadFileChallenge.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {          
            return View();
        }

        // GET: UploadFile
        public ActionResult Upload(HttpPostedFileBase file)
        {
            DataTable tbl = new DataTable();
            ViewBag.Success = "";
            ViewBag.Error = "";
            try
            {
                string fileLocation =Server.MapPath("~/App_Data/uploads/"+file.FileName);
                file.SaveAs(fileLocation);
                ViewBag.Success = "File Uploaded successfully to the following location: "+ fileLocation;
                tbl = UploadFile.ProcessUpload(fileLocation);
            }
            catch (Exception ex)
            {
                ViewBag.Success = "";
                ViewBag.Error ="Error: " +ex.Message;
            }                                  
            
            return View(tbl);
        }
    }
}