using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Diagnostics;
using Emgu.CV.GPU;

namespace DetectImage.Controllers
{
    public class HomeController : Controller
    {
        public static long threadHolding = 50;
        
        public ActionResult Index()
        {
            ViewBag.siteUrl = Request.Url.AbsoluteUri;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CheckImage(HttpPostedFileBase file)
        {
            //if (file.ContentLength <= 0) return;

            var fileName = Path.GetFileName(file.FileName);
                        //Console Log
            System.Diagnostics.Debug.WriteLine(file.FileName);
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            var processPath = Path.Combine(Server.MapPath("~/Images"), fileName+"_process.jpg");
            file.SaveAs(path);
            
            Image img = Image.FromFile(path);
            HomeController.RemoveBackground((Bitmap)img, processPath);
            bool check = HomeController.CheckBackground((Bitmap) img);

            ((IDisposable)img).Dispose();
            return Json(new
            {
                status = check,
                url = fileName + "_process.jpg"
            }, JsonRequestBehavior.AllowGet);
        }
        public static float GetGray(Color C)
        {
            //return (C.R + C.G + C.B) / 3;
            return (float)(0.2126 * C.R + 0.7152 * C.G + 0.0722 * C.B);
        }
        public static void RemoveBackground(System.Drawing.Bitmap picture, string path)
        {
            float ThreadHolding =5;
            Color backColor = Color.Transparent;
            Color preColor;
            float Temp;
            for (int h = 0; h < picture.Size.Height; h++)
            {
                preColor = picture.GetPixel(0, h);
                int w1 =0 ;
                for (int w = 0; w < picture.Size.Width; w++)
                {
                    Color c = picture.GetPixel(w,h);
                    Temp = Math.Abs(GetGray(preColor) - GetGray(c));
                    if (Temp > ThreadHolding)
                    {
                        w1 = w;
                        break;
                    }
                    picture.SetPixel(w, h, backColor);
                    preColor = c;
                }
                preColor = picture.GetPixel(picture.Size.Width - 1, h);
                for (int w = picture.Size.Width-1; w >= w1; w--)
                {
                    Color c = picture.GetPixel(w, h);
                    Temp = Math.Abs(GetGray(preColor) - GetGray(c));
                    if (Temp > ThreadHolding) break;

                    picture.SetPixel(w, h, backColor);
                    preColor = c;
                }
            }

            picture.Save(path);
        }
        public static bool CheckBackground(System.Drawing.Bitmap picture)
        {
            int threadHolding = (int)(picture.Size.Width * 0.1);
            float flagColor1 = 0, flagColor2 = 0;
            for (int i = 0; i < threadHolding; i++)
            {
                for (int j = 0; j < picture.Size.Height; j++)
                {
                    float Temp1 = 0, Temp2 = 0;
                    System.Drawing.Color c = picture.GetPixel(i, j);                    
                    Temp1 += c.R;
                    Temp1 += c.G;
                    Temp1 += c.B;
                    Temp1 = (int)Temp1 / 3;
                    if (j == 0) flagColor1 = Temp1;
                    else
                    {
                        if( Math.Abs(flagColor1-Temp1)>3)
                        {
                            return false;
                        }
                    }

                    System.Drawing.Color d = picture.GetPixel(picture.Size.Width-1-i, j);
                    Temp2 += d.R;
                    Temp2 += d.G;
                    Temp2 += d.B;
                    Temp2 = (int)Temp2 / 3;
                    if (j == 0) flagColor2 = Temp2;
                    else
                    {
                        if (Math.Abs(flagColor2 - Temp2) > 3)
                        {
                            return false;
                        }
                    }

                }
            }            
            return true;
        }
    }
}