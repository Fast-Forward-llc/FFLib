using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
namespace FFLibUnitTests
{
    [TestFixture]
    class UnitTests
    {
        [Test]
        public void CanStripHtml()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyStripHtml = "<a>this is a test</a>"};
            
            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("this is a test", testObject.propertyStripHtml);
        }
        public void CanStripHtmlDoc()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyStripHtml = "<html><head><meta type=\"qwerty\"/></head><body><a>this is a test</a></body></html>" };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("this is a test", testObject.propertyStripHtml);
        }

        public void CanStripStringFragment()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyStripHtml = "begin outside an element <a>this is a test</a> then more text" };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("begin outside an element this is a test then more text", testObject.propertyStripHtml);
        }

        [Test]
        public void CanAllowHtml()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowUnsafeHtml = "<script>this is a test</script>" };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("<script>this is a test</script>", testObject.propertyAllowUnsafeHtml);
        }


        [Test]
        public void CanAllowSafeHtml()
        {
            string test = "<div>here</div><p>this is a test</p>";
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowSafeHtml= test};

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("here\r\n<p>this is a test</p>\r\n", testObject.propertyAllowSafeHtml);          
        }

        [Test]
        public void CanAllowEmptySafeHtml()
        {
            string test = "<h1>title</h1><p>this is a paragraph with some <span>stuff</span></p>";
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowSafeHtml = test };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("title\r\n<p>this is a paragraph with some <span>stuff</span></p>\r\n", testObject.propertyAllowSafeHtml);         
            //no tag at beginning

        }

        [Test]
        public void CanAllowMalformedSafeHtml()
        {
            string test = "<h1>title</h1><p>this is a paragraph with some <span>stuff</span></p><a>this is malformed";
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowSafeHtml = test };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("title\r\n<p>this is a paragraph with some <span>stuff</span></p>\r\n<a>this is malformed</a>", testObject.propertyAllowSafeHtml);
        }

        [Test]
        public void CanAllowBeginningWithNoTagSafeHtml()
        {
            string test = "Hello<a>This is html</a>";
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowSafeHtml = test };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("Hello<a>This is html</a>", testObject.propertyAllowSafeHtml);  
        }

        [Test]
        public void CanAllowEndingWithNoTagSafeHtml()
        {
            string test = "<a>This is html</a>Hello";
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowSafeHtml = test };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("<a>This is html</a>Hello", testObject.propertyAllowSafeHtml);
        }

        [Test]
        public void CanAllowAmpersandSafeHtml()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowSafeHtml = "<a>more stuff & things</a>" };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("<a>more stuff &amp; things</a>", testObject.propertyAllowSafeHtml);
        }

        [Test]
        public void CanBlockUnsafeHtml()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyAllowSafeHtml = "<script>this is a test</script><a>more stuff</a>" };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("<a>more stuff</a>", testObject.propertyAllowSafeHtml);
        }




        [Test]
        public void CanEncodeHtml()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyEncodeHtml = "<p>this is a test</p>" };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("&lt;p&gt;this is a test&lt;/p&gt;", testObject.propertyEncodeHtml);

        }


        [Test]
        public void CanLeaveTextUnchanged()
        {
            //Arrange
            string test = "this is a test";
            TestObject testObject = new TestObject() { propertyEncodeHtml = test,propertyAllowSafeHtml = test,propertyAllowUnsafeHtml = test,propertyStripHtml = test };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase(test, testObject.propertyEncodeHtml);
            StringAssert.AreEqualIgnoringCase(test, testObject.propertyAllowSafeHtml);
            StringAssert.AreEqualIgnoringCase(test, testObject.propertyAllowUnsafeHtml);
            StringAssert.AreEqualIgnoringCase(test, testObject.propertyStripHtml);
        }

        [Test]
        public void DoesEncodePropertyWithNoAttribute()
        {
            //Arrange
            string test = "<a>this is a test</a>";
            TestObject testObject = new TestObject() { propertyNoAttribute = test};

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase("&lt;a&gt;this is a test&lt;/a&gt;", testObject.propertyNoAttribute);
        }

        [Test]
        public void DoesNotEncodeNullString()
        {
            //Arrange
            TestObject testObject = new TestObject() { propertyNoAttribute = null };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            StringAssert.AreEqualIgnoringCase(null, testObject.propertyNoAttribute);
        }

        [Test]
        public void OnlyEncodeString()
        {
            //Arrange
           
            TestObject testObject = new TestObject() { propertyInt =5 };

            //Act
            FFLib.HtmlEncoder.Encode(testObject);

            //Assert
            Assert.AreEqual(5, testObject.propertyInt);
        }


    }
}
