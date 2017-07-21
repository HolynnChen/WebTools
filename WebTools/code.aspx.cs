using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using mshtml;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Chrome;
using System.Drawing.Imaging;
using System.Drawing;

namespace WebTools
{
    public partial class code : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCodeImage();
        }
        private void GetCodeImage( )
        {
            IWebDriver web = new PhantomJSDriver();
            web.Navigate().GoToUrl("http://ce.sysu.edu.cn/hope/admin/login.aspx");
            Screenshot screenShotFile = ((ITakesScreenshot)web).GetScreenshot();
            screenShotFile.SaveAsFile(@"C:\Users\qq854\Documents\visual studio 2017\Projects\WebTools\code.jpg", ScreenshotImageFormat.Bmp);
            test.log("here");
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
            Image1.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(arr);
            Session["wb"] = web;
            /*
            WebBrowser WebControlName = new WebBrowser();
            WebControlName.Url = new Uri("http://ce.sysu.edu.cn/hope/admin/login.aspx");
            WebControlName.ScriptErrorsSuppressed = true;
            WebControlName.Tag = true;
            WebControlName.DocumentCompleted += (object sender, WebBrowserDocumentCompletedEventArgs e)=> {
                if (((WebBrowser)sender).ReadyState != WebBrowserReadyState.Complete) return;
                WebControlName.Tag = false;
            };
            do
            {
                System.Windows.Forms.Application.DoEvents();
            } while ((bool)WebControlName.Tag);
            HtmlElement ImgeTag = WebControlName.Document.GetElementById("img1");
            Clipboard.Clear();
            //获取网页所有内容  
            HTMLDocument hdoc = (HTMLDocument)WebControlName.Document.DomDocument;
            //获取网页body标签中的内容  
            HTMLBody hbody = (HTMLBody)hdoc.body;
            //创建一个接口  
            IHTMLControlRange hcr = (IHTMLControlRange)hbody.createControlRange();
            //获取图片地址  
            IHTMLControlElement hImg = (IHTMLControlElement)ImgeTag.DomElement;
            //将图片添加到接口中
            hcr.add(hImg);
            //将图片复制到内存  
            hcr.execCommand("Copy", false, null);
            //从粘贴板得到图片
            //返回得到的验证码  
            Session["code_cookie"] = WebControlName.Document.Cookie;*/
            return ;

        }
    }
}