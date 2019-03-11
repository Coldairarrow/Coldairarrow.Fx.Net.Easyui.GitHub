﻿using Coldairarrow.Util;
using System.IO;
using System.Web.Mvc;

namespace Coldairarrow.Web.Controllers
{
    public class DemoController : BaseController
    {
        #region 视图

        public ActionResult UMEditor()
        {
            return View();
        }

        public ActionResult UploadFileIndex()
        {
            return View();
        }
        public ActionResult UploadFileForm()
        {
            return View();
        }
        public ActionResult UploadImgView()
        {
            return View();
        }

        #endregion

        #region 接口

        public ActionResult UploadFile(string fileBase64, string fileName,string data)
        {
            byte[] bytes = fileBase64.ToBytes_FromBase64Str();
            string fileDir = System.Web.HttpContext.Current.Server.MapPath("~/Upload/File");
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);
            string filePath = Path.Combine(fileDir, fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (MemoryStream m = new MemoryStream(bytes))
                {
                    m.WriteTo(fileStream);
                }
            }

            return Success();
        }

        public ActionResult UploadImg(string fileName, string data)
        {
            var url = ImgHelper.GetImgUrl(data);            

            var res = new
            {
                success=true,
                src=url
            };
            return JsonContent(res.ToJson());
        }

        #endregion
    }
}