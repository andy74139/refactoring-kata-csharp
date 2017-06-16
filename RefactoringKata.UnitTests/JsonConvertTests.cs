using NUnit.Framework;

namespace RefactoringKata.UnitTests
{
    [TestFixture]
    public class JsonConvertTests
    {
        [Test]
        public void SerializeTest_Product()
        {
            var product = new Product("Shirt", 1, 3, 2.99, "TWD");
            var expected ="{\"code\": \"Shirt\", \"color\": \"blue\", \"size\": \"M\", \"price\": 2.99, \"currency\": \"TWD\"}";

            Assert.AreEqual(expected, product.JsonSerialize());
        }
    }
}
