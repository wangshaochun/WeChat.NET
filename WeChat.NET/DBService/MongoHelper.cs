﻿using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text; 

namespace WeChat.NET.DBService
{
    public class MongoHelper
    {

        static string strconn = ConfigurationManager.AppSettings["dbserver"].ToString(); 
        MongoServer server = MongoDB.Driver.MongoServer.Create(strconn);
        MongoDatabase database = null;
        HtmlHepler htmlhepler = new HtmlHepler();
        
        public MongoHelper()
        {  
           database = server.GetDatabase("dyx");
        }
        public BsonElement[] GetUser(string userEname)
        {          
            var collection = database.GetCollection<BsonDocument>("users");
            var query = MongoDB.Driver.Builders.Query.EQ("loginname", userEname);
            var t = collection.FindOneAs<BsonDocument>(query);
            if(t!=null)
                return t.ToArray();
            return null;

        }
        public BsonElement[] GetTopic(string title)
        {
            var collection = database.GetCollection<BsonDocument>("topics");
            var query = MongoDB.Driver.Builders.Query.EQ("title", title);
            var t = collection.FindOneAs<BsonDocument>(query);
            if (t != null)
                return t.ToArray();
            return null;

        }
        public bool AddUser(string loginname, string username, string Signature)
        {
            var user = database.GetCollection("users");
            BsonDocument doc = new BsonDocument();
            doc["loginname"] = loginname;
            doc["active"] = true;
            doc["name"] = username;
            doc["email"] = DateTime.Now.Ticks + "@dianxiaoyue.com";
            doc["accessToken"] = Guid.NewGuid().ToString();
            doc["avatar"] = "/public/img/" + loginname + ".jpg";
            doc["pass"] = "$2a$10$6.XBCgvurt2QHJsBy9poMeBvF0VDJXMpJ9a6w935Ufz0eYk8tojTO";
            doc["collect_tag_count"] = 0;
            doc["collect_topic_count"] = 0;
            doc["create_at"] = DateTime.Now;
            doc["is_block"] = false;
            doc["following_count"] = 0;
            doc["follower_count"] = 0;
            doc["receive_at_mail"] = false;
            doc["receive_reply_mail"] = false;
            doc["reply_count"] = 0;
            doc["score"] = 10;                     
            doc["tab"] = getTab(Signature);
            doc["topic_count"] = 0;
            doc["Signature"] = Signature;
            doc["update_at"] = DateTime.Now;
            user.Insert(doc);
            return true;
        }


        public bool UpdateUser(string loginname, string newName, string Signature)
        {

            var collection = database.GetCollection("users");
            var filter = MongoDB.Driver.Builders.Query.EQ("loginname", loginname);
            var update = MongoDB.Driver.Builders.Update.Set("name", newName)
                .Set("avatar", "/public/img/" + loginname + ".jpg")
                .Set("pass", "$2a$10$6.XBCgvurt2QHJsBy9poMeBvF0VDJXMpJ9a6w935Ufz0eYk8tojTO")
                .Set("active", true)             
                .Set("accessToken", Guid.NewGuid().ToString())        
                .Set("is_block", false)
                .Set("update_at", DateTime.Now)
                .Set("Signature",Signature);
            var result = collection.Update(filter, update);
            return true;
        }
        private string getTab(string Signature)
        {
            var tab = "yc";
            if (Signature.Contains("电影") || Signature.Contains("音乐") || Signature.Contains("大片"))
            {
                tab = "movie";
            }
            else if (Signature.Contains("美食") || Signature.Contains("饮食") || Signature.Contains("健身"))
            {
                tab = "food";
            }
            else if (Signature.Contains("汽车"))
            {
                tab = "auto";
            }
            else if (Signature.Contains("历史") || Signature.Contains("书"))
            {
                tab = "reedbook";
            }
            else if (Signature.Contains("编程"))
            {
                tab = "code";
            }
            else if (Signature.Contains("科技") || Signature.Contains("技术"))
            {
                tab = "tech";
            }           
            else if (Signature.Contains("搞笑") || Signature.Contains("笑话") || Signature.Contains("娱乐"))
            {
                tab = "fun";
            }
            else if (Signature.Contains("时尚") || Signature.Contains("生活"))
            {
                tab = "life";
            }
            else if (Signature.Contains("新闻"))
            {
                tab = "news";
            }
            return tab;
        }
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool AddTopic(string timestamp, string content, string userEname, string userName, string Signature)
        {
            try
            {
                var create_at = GetTime(timestamp);
                content = content.Replace("&lt;", "<").Replace("&gt;", ">");
                var links = HtmlHepler.GetItemByContent(content);
                foreach(var item in links)
                {

                    var title = item.Key;
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        return false;
                    }
                    else if (GetTopic(title) != null)
                    {
                        return false;
                    }
                    string body = item.Value;
                    var user = GetUser(userEname);
                    if (user == null)
                    {
                        AddUser(userEname, userName,Signature);
                    }
                    else if (user.FirstOrDefault(a => a.Name == "name") == null)
                    {
                        UpdateUser(userEname, userName, Signature);
                    }
                    var author = GetUser(userEname);

                    var author_id=author.FirstOrDefault(a => a.Name == "_id").Value;
                    var tab = author.FirstOrDefault(a => a.Name == "tab") == null ? "yc" : author.FirstOrDefault(a => a.Name == "tab").Value;

                    var topic = database.GetCollection("topics");
                    BsonDocument doc = new BsonDocument();
                    doc["title"] = title;
                    doc["content"] = body;
                    doc["tab"] =  tab;
                    doc["good"] = false;
                    doc["issend"] = 0;
                    doc["deleted"] = false;
                    doc["last_reply_at"] = create_at;
                    doc["update_at"] = create_at;
                    doc["create_at"] = create_at;
                    doc["collect_count"] = 0;
                    doc["top"] = false;
                    doc["reply_count"] = 0;
                    doc["visit_count"] = 0;
                    doc["author_id"] = author_id;
                    topic.Insert(doc);
                }
               
            }
            catch (Exception ex)
            {
                 
            }
           
            return true;
        }


    }
}
