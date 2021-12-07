using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Faker;

namespace TestProject1
{
    class CollectionTest
    {
        [Test]
        public void CollectionCreate()
        {
            Faker.Faker faker = new Faker.Faker();

            List<List<char>> difficult = faker.Create<List<List<char>>>();
            char b = difficult[0][0];
            Assert.AreEqual(b == default(char), false, "Item in List of Lists hasn't been initialized.");
        }
    }
}
