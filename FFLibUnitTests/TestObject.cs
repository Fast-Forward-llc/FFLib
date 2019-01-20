using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFLib.Attributes;

namespace FFLibUnitTests
{
    public class TestObject
    {
        public TestObject() { }

        [HtmlEncodingAttribute(HtmlEncoding.AllowSafeHtml)]
        public string propertyAllowSafeHtml { get; set; }

        [HtmlEncodingAttribute(HtmlEncoding.AllowUnsafeHtml)]
        public string propertyAllowUnsafeHtml { get; set; }

        [HtmlEncodingAttribute(HtmlEncoding.EncodeHtml)]
        public string propertyEncodeHtml { get; set; }

        [HtmlEncodingAttribute(HtmlEncoding.StripHtml)]
        public string propertyStripHtml { get; set; }

        public string propertyNoAttribute { get; set; }

       
        public int propertyInt { get; set; }
    }
}
