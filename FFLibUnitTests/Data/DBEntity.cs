using FFLib.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFLibUnitTests.Data
{
    [TestFixture]
    public class DBEntity_Test
    {
        [Test]
        public void DBEntityEquality()
        {
            var e1 = new Entity1()
            {
                Id = 1,
                Name = "Test",
                Today = DateTime.Now.Date,
                ModifiedBy = "John",
                ModifiedDate = DateTime.Now
            };
            var e2 = new Entity1()
            {
                Id = 1,
                Name = "Test",
                Today = DateTime.Now.Date,
                ModifiedBy = "Mary",
                ModifiedDate = DateTime.Now
            };
            var e3 = new Entity2()
            {
                Id = 1,
                Name = "Test",
                Today = DateTime.Now.Date,
                ModifiedBy = "Paul",
                ModifiedDate = DateTime.Now
            };

            Assert.IsTrue(FFLib.Data.DBEntityHelper.EntitiesAreEqual(e1, e2));
            Assert.IsTrue(FFLib.Data.DBEntityHelper.EntitiesAreEqual(e1, e3));
            Assert.IsTrue(FFLib.Data.DBEntityHelper.EntitiesAreEqual(e2, e3));

            e3.Name = "NegativeTest";
            Assert.IsFalse(FFLib.Data.DBEntityHelper.EntitiesAreEqual(e2, e3));
        }


        public class Entity1:DBEntity<Entity1>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Today { get; set; }
            public string ModifiedBy { get; set; }
            public DateTime ModifiedDate { get; set; }
        }

        public class Entity2
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Today { get; set; }
            public string ModifiedBy { get; set; }
            public DateTime ModifiedDate { get; set; }
        }
    }
}
