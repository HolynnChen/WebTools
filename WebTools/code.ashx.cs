using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading;

namespace WebTools
{
    /// <summary>
    /// code1 的摘要说明
    /// </summary>
    public class code1 : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["action"] != null)
            {
                switch (context.Request["action"].ToString())
                {
                    case "code":
                        context.Response.ContentType = "text/plain";
                        IWebDriver web = new PhantomJSDriver();
                        web.Navigate().GoToUrl("http://ce.sysu.edu.cn/hope/admin/login.aspx");
                        Screenshot screenShotFile = ((ITakesScreenshot)web).GetScreenshot();
                        screenShotFile.SaveAsFile(@"C:\Users\qq854\Documents\visual studio 2017\Projects\WebTools\code.jpg", ScreenshotImageFormat.Bmp);
                        string baseFilePath = @"C:\Users\qq854\Documents\visual studio 2017\Projects\WebTools\code.jpg";
                        Bitmap bmpBase = new Bitmap(baseFilePath);
                        var a = web.FindElement(By.Id("img1")).Location;
                        var b = web.FindElement(By.Id("img1")).Size;
                        Rectangle rect = new Rectangle(a.X, a.Y, b.Width, b.Height);
                        Bitmap bmpNew = bmpBase.Clone(rect, bmpBase.PixelFormat);
                        bmpBase.Dispose();
                        bmpNew.Save(baseFilePath, ImageFormat.Bmp);
                        MemoryStream ms = new MemoryStream();
                        bmpNew.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        bmpNew.Dispose();
                        byte[] arr = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(arr, 0, (int)ms.Length);
                        ms.Close();
                        context.Response.Write("data:image/jpeg;base64," + Convert.ToBase64String(arr));
                        context.Session["wb"] = web;
                        context.Response.End();
                        break;
                    case "login":
                        if (context.Request["yzm"] != null && context.Session["wb"] != null)
                        {
                            IWebDriver req = (IWebDriver)context.Session["wb"];
                            req.FindElement(By.Id("txtAccount")).SendKeys("陈浩林");
                            req.FindElement(By.Id("txtPassword")).SendKeys("zxc854560673");
                            req.FindElement(By.Id("txtValidatorCode")).SendKeys(context.Request["yzm"].ToString());
                            var old = req.Url;
                            req.FindElement(By.Id("btnLogin")).Click();
                            Thread.Sleep(300);
                            int pi = 0;
                            for (int i = 0; i < 30 && old == req.Url; i++) {
                                pi = i;
                                Thread.Sleep(100);
                            }
                            if (pi == 29) {
                                context.Session["wb"] = null;
                                context.Response.Write("outTime");
                                context.Response.End();
                            } else {
                                context.Session["wb"] = req;
                                context.Response.Write("success");
                                context.Response.End();
                            }
                            
                        }
                        else
                        {
                            context.Response.Write("null");
                        }
                        break;
                }
            }
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}