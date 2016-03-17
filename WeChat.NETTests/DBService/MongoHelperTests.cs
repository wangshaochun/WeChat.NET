using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChat.NET.DBService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace WeChat.NET.DBService.Tests
{
    [TestClass()]
    public class MongoHelperTests
    {
        [TestMethod()]
        public void GetUserTest()
        {
            var mongo = new MongoHelper();
            var t=mongo.GetUser("dingxiaoyue");
            Assert.Fail();
        }

        [TestMethod()]
        public void AddTopicTest()
        {
            var mongo = new MongoHelper();
            var t = mongo.AddTopic("1409876543","1234","dingxiaoyue","订小阅号","");
            Assert.Fail();
        }
    }
}
