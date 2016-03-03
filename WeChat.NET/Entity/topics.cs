using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.NET.Entity
{
    public class topics
    {
        public bool good { get; set; }
        public int issend { get; set; }
        public string title { get; set; }
        public bool deleted { get; set; }
        public DateTime last_reply_at { get; set; }
        public DateTime update_at { get; set; }
        public DateTime create_at { get; set; }
        public int collect_count { get; set; }
        public bool top { get; set; }
        public string content { get; set; }

        public int reply_count { get; set; }
        public int visit_count { get; set; }
        public string tab { get; set; }

        public ObjectId author_id { get; set; }

    }
}
