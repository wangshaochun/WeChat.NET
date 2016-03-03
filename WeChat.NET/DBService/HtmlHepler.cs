using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeChat.NET.DBService
{
    public class HtmlHepler
    {
        public string GetPage(string url, Encoding encoding)
        {
            //初始化新的request对象
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";

            webRequest.CookieContainer = new CookieContainer();
            //返回Internet资源的相应
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            //获取流
            var stream = webResponse.GetResponseStream();
            //读取编码
            var reader = new StreamReader(stream, encoding);
            //整个页面内容
            var content = reader.ReadToEnd();
            reader.Close();
            webResponse.Close();

            return content;
        }
        public string GetPage(string url, Encoding encoding, string strProxyIP)
        {

            //初始化新的request对象
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            //webRequest.KeepAlive = false;
            //webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.CookieContainer = new CookieContainer();
            //设置代理
            WebProxy proxy = new WebProxy(strProxyIP, true);
            proxy.BypassProxyOnLocal = true;
            webRequest.Proxy = proxy;
            //返回Internet资源的相应
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            //获取流
            var stream = webResponse.GetResponseStream();
            //读取编码
            var reader = new StreamReader(stream, encoding);
            //整个页面内容
            var content = reader.ReadToEnd();
            reader.Close();
            webResponse.Close();

            return content;
        }
        /// <summary>
        /// 获取超链接的文本
        /// </summary>
        /// <param name="taghtml"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetItemByContent(string content)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
           
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(content);
            int i = 0;
            foreach(System.Xml.XmlElement  item in  doc.ChildNodes[0].ChildNodes[0].ChildNodes[11].ChildNodes[0].ChildNodes)
            {
                i++;
                if(i<=2)
                {
                    continue;
                }
                var nodes = item.ChildNodes;
                var title = nodes[1].InnerText;
                var url = nodes[2].InnerText;
                result.Add(title, url);
            }
            
            return result;
        }
        public static List<string> GetTextByLink(string taghtml)
        {

            List<string> result = new List<string>();
            Regex re = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=;]*)?", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchList = re.Matches(taghtml);
            foreach (Match item in matchList)
            {
                if (item.ToString().IndexOf("mp.weixin.qq.com") > 0 && !result.Contains(item.ToString()))
                    result.Add(item.ToString());
            }
            return result;
        }
        public static bool CheckProxy(string Url, string ProxyAddr)
        {
            try
            {
                WebProxy CurrentWebProxy = new WebProxy(ProxyAddr, true);
                CurrentWebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                WebRequest sr = WebRequest.Create(Url);
                sr.Proxy = CurrentWebProxy;
                sr.GetResponse();
                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        internal string GetTitle(string taghtml)
        {  
            Regex re = new Regex(@"<title>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchList = re.Matches(taghtml);
            return matchList.Count >= 1 ? matchList[0].Value.Replace("<title>", "").Replace("</title>", "").Replace("&nbsp;","-") : "";
             
        }

        internal string GetBody(string html)
        {
             
            if(html.IndexOf("rich_media_thumb_wrp")>0)
            {

                Regex re = new Regex("<div class=\"rich_media_thumb_wrp[\\s\\S]*?<script type=\"text/javascript\">");               
                var matchList = re.Matches(html);
                foreach (Match item in matchList)
                {
                    return item.ToString().Replace("<script type=\"text/javascript\">","");
                }
            }
            else
            {

                Regex re = new Regex("<div class=\"rich_media_content[\\s\\S]*?<script type=\"text/javascript\">");
                var matchList = re.Matches(html);
                foreach (Match item in matchList)
                {
                    return item.ToString().Replace("<script type=\"text/javascript\">", "");
                }
            }
            return "";
        }

        internal string GetYuanwenUrl(string html)
        {
            if (html.IndexOf("var msg_source_url = ''") > 0)
                return "";
            Regex re = new Regex(@"var msg_source_url = '(.*?)rd", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchList = re.Matches(html);
            return matchList.Count >= 1 ? matchList[0].Value.Replace("var msg_source_url = '", ""): "";
        }
    }
}
