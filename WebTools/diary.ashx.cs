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
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace WebTools
{
    /// <summary>
    /// diary1 的摘要说明
    /// </summary>
    public class diary1 : IHttpHandler,System.Web.SessionState.IRequiresSessionState
    {
        string[] zubie = { "网管组", "数码组", "编程组", "前端组", "设计组" };
        string[][] all_16 ={
            new string[] { "谈正起", "杨利婷", "贺泽芃" },//按顺序，依次是网管组
            new string[] { "唐玮祺" },//数码组
            new string[] { "邓泳锋", "冯泽伦", "陈浩林", "张春祥", "李佳新", "林录奇" },//编程组
            new string[] { "林秘海", "陈焕森" },//前端组
            new string[] { "黄雅诗" } };//设计组
        string[][] all_15 ={
            new string[] { "文海平", "罗丽苹" },
            new string[] { "童礼翎" },
            new string[] { "陈楠雅", "卢向东" },
            new string[] { "雷雨", "郑培钦", "黄睿", "刘浩翔", "宋婧仪" },
            new string[] { "范敬一", "胡慧盈", "吴超美", "钟浩明" } };
        string[][] all_14 ={
            new string[] { "姜永浩" },
            new string[] { "曹颖雯", "招润捷"},
            new string[] { "谢志强", "崔霄", "冯常殷" },
            new string[] { "徐东航" },
            new string[] { "黄金梅", "胡泳琳", "钟雯璇", "戴雨晴" } };
        int maxnianji = 16;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.BufferOutput = true;
            if (context.Request["action"] != null)
            {
                switch (context.Request["action"].ToString())
                {
                    case "search_diary":
                        //判断参数是否存在
                        context.Response.ContentType = "application/x-javascript";
                        if (context.Request["name"] == null || context.Request["time"] == null) {
                            context.Response.Write("参数错误");
                            context.Response.End();
                            return;
                        }
                        Regex rul = new Regex(@"\d{4}-\d{2}-\d{2}\|\d{4}-\d{2}-\d{2}");
                        if (!rul.IsMatch((string)context.Request["time"])) {
                            context.Response.Write("时间参数错误");
                            context.Response.End();
                        }
                        //判断完毕
                        //先进行初始化
                        if (!frist(ref context)) {
                            context.Response.ClearContent();
                            context.Response.Write("fault");
                            context.Response.End();
                        };
                        //初始化完毕
                        //开始查询
                        //获取人名非all为单
                        if (!context.Request["name"].ToString().Contains("all"))
                        {
                            string name = context.Request["name"];
                            po_to_user(name,ref context);
                            context.Response.End();
                        }
                        else {
                            string[][] ppl= { };
                            switch (context.Request["name"].ToString()) {
                                case "all_16":
                                    ppl = all_16;
                                    break;
                                case "all_15":
                                    ppl = all_15;
                                    break;
                                case "all_14":
                                    ppl = all_14;
                                    break;
                                    }
                            foreach (var i in ppl) {
                                foreach (var n in i) {
                                    po_to_user(n,ref context);
                                    if(n!=ppl[ppl.Length-1][ppl[ppl.Length - 1].Length-1])frist(ref context);
                                }
                            }
                        }
                        break;
                    case "search_arc":
                        context.Response.ContentType = "application/x-javascript";
                        //判断参数是否存在
                        if (!frist(ref context,true))
                        {
                            context.Response.ClearContent();
                            context.Response.Write("fault");
                            context.Response.End();
                        };
                        if (context.Request["searchkey"] == null)
                        {
                            context.Response.Write("参数错误");
                            context.Response.End();
                            return;
                        }
                        else {
                            string sk = context.Request["searchkey"];
                            int sw = context.Request["searchway"] != null ? int.Parse(context.Request["searchway"]) : 1;
                            int sn = context.Request["searchnumber"] != null ? int.Parse(context.Request["searchnumber"]) : -1;
                            bool de = context.Request["delcontent"] != null && context.Request["delcontent"] == "1" ? true : false;
                            bool dec = context.Request["detail"] != null && context.Request["detail"] == "1" ? true : false;
                            arc_node[] jjk=search_arc_diy(ref context,sk,sw,sn);
                            //初始化输入
                            bool dc_put= context.Request["dc_put"] != null && context.Request["dc_put"] == "1" ? true : false;
                            if (dc_put) {
                                if (context.Session["excel_app"]!=null) { context.Response.Write("output now");context.Response.End();return; };
                                string[] k = {"姓名","ID","节点","标题" };
                                if (dec) k=(new string[] { "年级", "组别" }).Concat(k).ToArray();
                                if (!de) k=k.Concat(new string[] { "内容" }).ToArray();
                                excel_start(ref context,k);
                            }//初始化表头完毕
                            var ipoi = 0;
                            foreach (var i in jjk) {
                                if (!dc_put)
                                {
                                    string ppo = i.name + ";" + i.id + ";" + i.node_name.Trim() + ";" + LZString.LZString.compressToBase64(i.title);
                                    if (dec) { var iu = get_detail(i.name); ppo = iu[0] + ";" + iu[1] + ";" + ppo; }
                                    if (!de) ppo += ";" + LZString.LZString.compressToBase64(get_content(ref context, i.id, true, "hope_article_content"))+"-";
                                    context.Response.Write("success|" + ppo);
                                    context.Response.FlushAsync();
                                }
                                else {
                                    string[] ppo = { i.name, i.id, i.node_name.Trim(), i.title };
                                    if (dec) { var iu = get_detail(i.name); ppo = (new string[] { iu[0], iu[1] }).Concat(ppo).ToArray(); }
                                    if (!de) ppo=ppo.Concat(new string[] { get_content(ref context, i.id, true, "hope_article_content") }).ToArray();
                                    excel_input(ref context,ppo);
                                    ipoi++;
                                    context.Response.Write("|导出进度:" + ipoi.ToString() + "/" + jjk.Length.ToString()+"|");
                                    context.Response.FlushAsync();

                                }
                            }
                            if (dc_put) { string uri = excel_finish(ref context); context.Response.Write("success|" + uri); };
                            if (jjk.Length == 0) {
                                context.Response.Write( "no_arc");
                            }
                            context.Response.End();
                        }
                        frist(ref context, true);



                        break;
                    default:
                        context.Response.Write("invalid parameter");
                        break;
                }
            }
            else {
                context.Response.Write("null");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public bool frist(ref HttpContext context,bool i=false) {
            if (context.Session["wb"] == null) return false;
            //context.Session["loading"] = true;
            IWebDriver web = (IWebDriver)context.Session["wb"];
            //web.Navigate().GoToUrl("http://ce.sysu.edu.cn/hope/Admin/Content/Article/ArticleList.aspx?NodeID=78");
            var old = web.PageSource;
            if (i)
            {
                web.Url = "http://ce.sysu.edu.cn/hope/Admin/Content/Article/ArticleList.aspx";
            } else {
                web.Url = "http://ce.sysu.edu.cn/hope/Admin/Content/Article/ArticleList.aspx?NodeID=78";
            }
            
            Thread.Sleep(100);
            do { System.Windows.Forms.Application.DoEvents(); } while (old == web.PageSource);
            //Thread.Sleep(100);
            context.Session["wb"] = web;
            return true;
        }

        public diary_node[] search_one(ref HttpContext context, string name, string start_time = "2000-01-01", string end_time = "2050-01-01") {
            string[] result= { };
            diary_node[] node_record = { };
            List<string> all = result.ToList();
            List<diary_node> all2 = node_record.ToList();
            IWebDriver web = (IWebDriver)context.Session["wb"];
            IJavaScriptExecutor js = web as IJavaScriptExecutor;
            var old = web.PageSource;
            js.ExecuteScript("document.getElementById('ddlSearchField').value='Content_Article.Inputer';document.getElementById('txtSearchKey').value='"+name+ "';document.getElementById('btnSearch').click();");
            Thread.Sleep(100);
            do { System.Windows.Forms.Application.DoEvents(); } while (old == web.PageSource);
            string[] ret;
            if(web.FindElement(By.Id("divTip")).Text== "此节点下或当前查询条件无任何记录！") return all2.ToArray();
            do
            {
                string gt = ((string)js.ExecuteScript("var all=0;var ele=document.getElementsByClassName('contTitle');var ide=document.getElementsByClassName('contID');var k='';var s=0;var need='1';for(var i=1;i<ele.length;i++){var time=Date.parse((ele[i].innerText+' 00:00').replace(/-/g,'/'));if(!isNaN(time)){if(time>=Date.parse(('" + start_time + " 00:00').replace(/-/g,'/'))&&time<=Date.parse(('" + end_time + "').replace(/-/g,'/'))){s=i;k+='|'+ide[i].innerText+':'+ele[i].innerText}else{if(time<Date.parse(('2017-07-10 00:00').replace(/-/g,'/'))){need='0';break}}}};need+=k;return need;"));
                ret = gt.Split('|');
                if (ret.Length > 1) {
                    for (int a = 1; a < ret.Length; a++) {
                        diary_node k = new diary_node();
                        k.name = name;
                        k.id = ret[a].Split(':')[0];
                        k.title= ret[a].Split(':')[1];
                        all2.Add(k);
                    }
                }
                if (ret[0] == "1") {
                    var k=(string)js.ExecuteScript("if(document.getElementsByClassName('paginator')[1]==null)return 'no';var o=document.getElementsByClassName('paginator')[1].innerText;__doPostBack('hp_paginator',String(Number(o.substring(o.indexOf('第')+1,o.indexOf('/')))+1));return 'yes';");
                    Thread.Sleep(100);
                    if (k == "no") break;
                };//当前日志时间均大于所选时间段
            } while (ret[0] != "0");
            old = web.PageSource;
            string kkip=(string)js.ExecuteScript("if(document.getElementsByClassName('paginator')[1]==null)return 'no';var o=document.getElementsByClassName('paginator')[1].innerText;__doPostBack('hp_paginator','1');return 'success';");
            if (kkip != "no") {
                Thread.Sleep(100);
                do { System.Windows.Forms.Application.DoEvents(); } while (old == web.PageSource);
            }
            context.Session["wb"] = web;
            return all2.ToArray();
        }

        public struct diary_node {
            public string name;
            public string id;
            public string title;
        }

        public struct arc_node {
            public string name;
            public string id;
            public string node_name;
            public string title;
        }

        public string[] get_detail(string name) {
            int o,z;
            foreach (var k in new string[][][]{all_16,all_15,all_14 })
            {
                o = 0;
                z = 0;
                foreach (var i in k)
                {
                    foreach (var a in i)
                    {
                        if (a == name) { return new string[]{ (maxnianji - z).ToString(),zubie[o]}; }
                    }
                    o++;
                }
                z++;
            }
            return new string[] { "null", "null" };
        }

        public string get_content(ref HttpContext context, string id, bool html = true,string classname= "dairylist_content") {
            IWebDriver to = (IWebDriver)context.Session["wb"];
            var old = to.PageSource;
            to.Navigate().GoToUrl("http://ce.sysu.edu.cn/hope/Item/" + id + ".aspx");
            do { System.Windows.Forms.Application.DoEvents(); } while (old == to.PageSource);
            string output;
            IJavaScriptExecutor js = to as IJavaScriptExecutor;
            if (html) {
                output=(string)js.ExecuteScript("return document.getElementsByClassName('"+classname+"')[0].innerHTML;");
            } else {
                output = (string)js.ExecuteScript("return document.getElementsByClassName('"+ classname +"')[0].innerText;");
            }
            return output;
        }

        public void po_to_user(string name,ref HttpContext context) {
            string start_time = context.Request["time"].Split('|')[0];
            string end_time = context.Request["time"].Split('|')[1];
            diary_node[] reo = search_one(ref context, name, start_time, end_time);
            string output = (reo.Length == 0) ? "false" : "success";
            bool isu = true, icu = true, inc = false;
            if (context.Request["outputtext"] != null && context.Request["outputtext"] == "1") isu = false;
            if (context.Request["onebyone"] != null && context.Request["onebyone"] == "1") icu = false;
            if (context.Request["delcontent"] != null && context.Request["delcontent"] == "1") inc = true;
            if (context.Request["detail"] != null && context.Request["detail"] == "1")
            {
                foreach (var k in reo)
                {
                    string[] ms = get_detail(k.name);
                    string p = "|" + ms[0] + ";" + ms[1] + ";" + k.name + ";" + k.id + ";" + LZString.LZString.compressToBase64(k.title);
                    if (!inc) p += ";" + LZString.LZString.compressToBase64(get_content(ref context, k.id, isu))+"-";
                    output += p;
                    if (!icu)
                    {
                        context.Response.Clear();
                        context.Response.Write("success" + p);
                        context.Response.FlushAsync();
                    }
                };
            }
            else
            {
                foreach (var k in reo)
                {
                    string p = "|" + k.name + ";" + k.id + ";" + LZString.LZString.compressToBase64(k.title);
                    if (!inc) p += ";" + LZString.LZString.compressToBase64(get_content(ref context, k.id, isu))+"-";
                    output += p;
                    if (!icu)
                    {
                        context.Response.Clear();
                        context.Response.Write("success" + p);
                        context.Response.FlushAsync();
                    }
                };
            }
            if (icu)
            {
                context.Response.Write(output);
                context.Response.FlushAsync();
            }
        }

        public void search_arc_one(string searchkey, ref HttpContext context, bool delcontent = true) {
            IWebDriver web = (IWebDriver)context.Session["wb"];
            IJavaScriptExecutor js = web as IJavaScriptExecutor;
            var old = web.PageSource;
            js.ExecuteScript("document.getElementById('ddlSearchField').value='Content_Article.Inputer';document.getElementById('txtSearchKey').value='" + searchkey + "';document.getElementById('btnSearch').click();");
            Thread.Sleep(100);
            do { System.Windows.Forms.Application.DoEvents(); } while (old == web.PageSource);
            if (web.FindElement(By.Id("divTip")).Text == "此节点下或当前查询条件无任何记录！") {
                context.Session["wb"] = web;
                context.Response.Write("null");
                context.Response.End();
                return;
            };
            string output = (string)js.ExecuteScript("if(document.getElementsByClassName('contID').length>1){return document.getElementsByClassName('contID')[1].innerText+'|!|'+document.getElementById('repList_lnkNodeName_0').innerText+'|!|'+document.getElementById('repList_lnkTitle_0').innerText+'|!|'+document.getElementById('repList_lnkInputer_0').innerText;}else{return 'null'};");
            if (!delcontent) output += "|!|"+LZString.LZString.compressToBase64( get_content(ref context, output.Split(new string[] { "|!|" }, StringSplitOptions.RemoveEmptyEntries)[0],true ,"hope_article_content"))+"-";
            context.Session["wb"] = web;
            context.Response.Write(output);
            context.Response.End();
        }

        public arc_node[] search_arc_diy(ref HttpContext context, string searchkey, int searchway = 1, int searchnumber = -1) {
            IWebDriver web = (IWebDriver)context.Session["wb"];
            IJavaScriptExecutor js = web as IJavaScriptExecutor;
            string[] io = { "Content_Article.GeneralID", "Content_Article.Title", "Content_Article.Inputer" };
            if (searchnumber < 0) { searchnumber = 9999; }
            var old = web.PageSource;
            js.ExecuteScript("document.getElementById('ddlSearchField').value='"+io[searchway]+"';document.getElementById('txtSearchKey').value='" + searchkey + "';document.getElementById('btnSearch').click();");
            Thread.Sleep(100);
            do { System.Windows.Forms.Application.DoEvents(); } while (old == web.PageSource|| web.FindElement(By.Id("divTip"))==null);
            if (web.FindElement(By.Id("divTip")).Text == "此节点下或当前查询条件无任何记录！")
            {
                context.Session["wb"] = web;
                return new arc_node[]{ };
            };
            string output = "";
            arc_node[] pio = { };
            List<arc_node> cio = pio.ToList();
            do
            {
                test.log(searchnumber.ToString());
                output = (string)js.ExecuteScript("var output='1';var num="+searchnumber.ToString()+ ";if(num==0){return '0';};var icp=document.getElementsByClassName('contID');if(icp.length<21 || document.getElementsByClassName('paginator').length==0){output='0';}else if(document.getElementsByClassName('paginator')[2].getElementsByTagName('a')[document.getElementsByClassName('paginator')[2].getElementsByTagName('a').length-1].href==''){output='0';};for(var i=1;i<icp.length&&i<num+1;i++){output+='|!!|'+icp[i].innerText+'|!|'+document.getElementById('repList_lnkNodeName_'+String(i-1)).innerText+'|!|'+document.getElementById('repList_lnkTitle_'+String(i-1)).innerText+'|!|'+document.getElementById('repList_lnkInputer_'+String(i-1)).innerText;}return output;");
                test.log(output);
                string[] arc_all = output.Split(new string[] { "|!!|" }, StringSplitOptions.RemoveEmptyEntries);
                searchnumber -= arc_all.Length - 1;
                if (arc_all.Length == 0) break;
                for (var i=1; i<arc_all.Length; i++) {
                    arc_node cciop;
                    string[] llop = arc_all[i].Split(new string[] { "|!|" }, StringSplitOptions.RemoveEmptyEntries);
                    cciop.id = llop[0];
                    cciop.node_name = llop[1];
                    cciop.title = llop[2];
                    cciop.name = llop[3];
                    cio.Add(cciop);
                }
                if (output.Substring(0, 1) == "1") {
                    old = web.PageSource;
                    js.ExecuteScript("var o=document.getElementsByClassName('paginator')[1].innerText;__doPostBack('hp_paginator',String(Number(o.substring(o.indexOf('第')+1,o.indexOf('/')))+1));");
                    Thread.Sleep(100);
                    do { System.Windows.Forms.Application.DoEvents(); } while (old == web.PageSource || web.FindElement(By.Id("divTip")) == null);
                }
            } while (output.Substring(0, 1) == "1");
            pio = cio.ToArray();
            context.Session["wb"] = web;
            return pio;
        }

        public void isFinish(string url,ref HttpContext context) {
            IWebDriver web = (IWebDriver)context.Session["wb"];
            do {
                Thread.Sleep(100);
            }while(web.Url != url);
            IJavaScriptExecutor js = web as IJavaScriptExecutor;
            js.ExecuteScript("while(document.readyState!='complete'){};");
        }

        public void excel_start(ref HttpContext context,string[] frist) {
            Microsoft.Office.Interop.Excel.Application appexcel = new Microsoft.Office.Interop.Excel.Application();
            Workbook datasum = appexcel.Workbooks.Add(true);
            datasum.Activate();
            Worksheet sheet = datasum.Worksheets["Sheet1"];
            for(var i = 1; i < frist.Length+1; i++)
            {
                sheet.Cells[1, i] = frist[i-1];
            }
            context.Session["excel_app"] = appexcel;
            context.Session["excel_wb"] = datasum;
            context.Session["excel_ws"] = sheet;
            context.Session["excel_num"] = 1;
        }
        public void excel_input(ref HttpContext context, string[] input) {
            Worksheet sheet = (Worksheet)context.Session["excel_ws"];
            int a = (int)context.Session["excel_num"]+1;//下移
            for (var i = 1; i < input.Length+1; i++) {
                sheet.Cells[a, i] = input[i-1];
            }
            context.Session["excel_num"] = a;
            context.Session["excel_ws"] = sheet;
        }
        public string excel_finish(ref HttpContext context) {
            Workbook datasum = (Workbook)context.Session["excel_wb"];
            string a = "统计表" + test.GetRandomString(6, true, true, true, false, "") + ".xlsx";
            datasum.SaveCopyAs(@"C:\Users\qq854\Documents\visual studio 2017\Projects\WebTools\WebTools\" + a);
            datasum.Close(false, null, null);
            ((Application)context.Session["excel_app"]).Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject((Worksheet)context.Session["excel_ws"]);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(datasum);
            //抛弃所有保存
            context.Session["excel_app"] = null;
            context.Session["excel_wb"] = null;
            context.Session["excel_ws"] = null;
            context.Session["excel_num"] = null;
            return a;
        }
    }
}