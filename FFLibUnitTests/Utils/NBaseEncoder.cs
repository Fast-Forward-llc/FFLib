using FFLib.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFLibUnitTests.Utils
{
    [TestFixture]
    public class NBaseEncoder_Test
    {
        [Test]
        public void EncodeBase16()
        {
            var charset = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            var result0 = NBaseEncoder.Encode(0, charset);
            var result1 = NBaseEncoder.Encode(1, charset);
            var result2 = NBaseEncoder.Encode(10, charset);
            var result3 = NBaseEncoder.Encode(15, charset);
            var result4 = NBaseEncoder.Encode(16, charset);
            var result5 = NBaseEncoder.Encode(16031, charset);

            Assert.AreEqual("0", result0);
            Assert.AreEqual("1", result1);
            Assert.AreEqual("A", result2);
            Assert.AreEqual("F", result3);
            Assert.AreEqual("10", result4);
            Assert.AreEqual("3E9F", result5);
        }
    }
}
