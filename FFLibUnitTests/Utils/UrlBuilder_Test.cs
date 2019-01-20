using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FFLibUnitTests.Utils
{
    [TestFixture]
    public class UrlBuilder_Test
    {
        [Test]
        public void Parse()
        {
            var urlBuilder = new FFLib.UrlBuilder();
            urlBuilder.Parse("http://www.google.com/analytics?id=123&view=summary#dashboard");

            Assert.AreEqual("http", urlBuilder.Scheme);
            Assert.AreEqual("www.google.com", urlBuilder.Host);
            Assert.AreEqual("/analytics", urlBuilder.Path);
            Assert.AreEqual("id=123&view=summary", urlBuilder.Query);
            Assert.AreEqual("dashboard", urlBuilder.Fragment);

            urlBuilder = new FFLib.UrlBuilder();
            urlBuilder.Parse("http://google.com/analytics.aspx#dashboard");

            Assert.AreEqual("http", urlBuilder.Scheme);
            Assert.AreEqual("google.com", urlBuilder.Host);
            Assert.AreEqual("/analytics.aspx", urlBuilder.Path);
            Assert.IsEmpty(urlBuilder.Query);
            Assert.AreEqual("dashboard", urlBuilder.Fragment);

            urlBuilder = new FFLib.UrlBuilder();
            urlBuilder.Parse("/analytics?id=123&view=summary");

            Assert.IsEmpty(urlBuilder.Scheme);
            Assert.IsEmpty(urlBuilder.Host);
            Assert.AreEqual("/analytics", urlBuilder.Path);
            Assert.AreEqual("id=123&view=summary", urlBuilder.Query);
            Assert.IsEmpty(urlBuilder.Fragment);

            urlBuilder = new FFLib.UrlBuilder();
            urlBuilder.Parse("https://www.google.com");

            Assert.AreEqual("https", urlBuilder.Scheme);
            Assert.AreEqual("www.google.com", urlBuilder.Host);
            Assert.IsEmpty(urlBuilder.Path);
            Assert.IsEmpty(urlBuilder.Query);
            Assert.IsEmpty(urlBuilder.Fragment);

            urlBuilder = new FFLib.UrlBuilder();
            urlBuilder.Parse("https://ww2.google.com?x=123");

            Assert.AreEqual("https", urlBuilder.Scheme);
            Assert.AreEqual("ww2.google.com", urlBuilder.Host);
            Assert.IsEmpty(urlBuilder.Path);
            Assert.AreEqual("x=123", urlBuilder.Query);
            Assert.IsEmpty(urlBuilder.Fragment);
        }

        [Test]
        public void SetParam()
        {
            var urlBuilder = new FFLib.UrlBuilder();
            urlBuilder.Parse("https://www.google.com");
            urlBuilder.SetQueryParam("key1", "value string");

            Assert.AreEqual("key1=value+string",urlBuilder.Query);

            urlBuilder.SetQueryParam("key2", "value+&+string");

            Assert.AreEqual("key1=value+string&key2=value%2b%26%2bstring", urlBuilder.Query);

            urlBuilder.Parse("https://www.google.com");
            urlBuilder.SetQueryParam("key1", "value string");
            urlBuilder.SetQueryParam("key2", "value+&+string");
            Assert.AreEqual("https://www.google.com?key1=value+string&key2=value%2b%26%2bstring", urlBuilder.ToString());


            urlBuilder.Parse("https://www.google.com/path/page.html#anchor");
            urlBuilder.SetQueryParam("key1", "value string");
            urlBuilder.SetQueryParam("key2", "value+&+string");
            Assert.AreEqual("https://www.google.com/path/page.html?key1=value+string&key2=value%2b%26%2bstring#anchor", urlBuilder.ToString());

        }

        [Test]
        public void GetParam()
        {
            var urlBuilder = new FFLib.UrlBuilder();
            urlBuilder.Parse("https://www.google.com");
            urlBuilder.SetQueryParam("key1", "value string");

            Assert.AreEqual("value string", urlBuilder.GetQueryParam("key1"));

            urlBuilder.SetQueryParam("key2", "value+&+string");

            Assert.AreEqual("value+&+string", urlBuilder.GetQueryParam("key2"));

            urlBuilder.Parse("https://www.google.com?key1=value+string&key2=value%2b%26%2bstring");
            Assert.AreEqual("value+&+string", urlBuilder.GetQueryParam("key2"));

            urlBuilder.SetQueryParam("key3", "some_other_value");
            Assert.AreEqual("value+&+string", urlBuilder.GetQueryParam("key2"));

        }
    }
}
