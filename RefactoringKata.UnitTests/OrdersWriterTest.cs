using System;
using NUnit.Framework;

namespace RefactoringKata.UnitTests
{
    [TestFixture]
    public class OrdersWriterTest
    {
        private Orders _orders;
        private Order _order111;

        [SetUp]
        public void SetupOneOrder()
        {
            _order111 = new Order(111);
            _orders = new Orders();
            _orders.AddOrder(_order111);
        }

        [Test]
        public void NoOrder()
        {
            Assert.AreEqual("{\"orders\": []}", new Orders().JsonSerialize());
        }

        [Test]
        public void OneOrder()
        {
            var order111 = "{\"id\": 111, \"products\": []}";
            Assert.AreEqual("{\"orders\": [" + order111 + "]}", new OrdersWriter(_orders).GetContents());
        }

        [Test]
        public void OneOrderWithOneProduct()
        {
            _order111.AddProduct(new Product("Shirt", 1, 3, 2.99, "TWD"));

            var order111Json = JsonOrder111WithProduct("{\"code\": \"Shirt\", \"color\": \"blue\", \"size\": \"M\", \"price\": 2.99, \"currency\": \"TWD\"}");
            Assert.AreEqual("{\"orders\": [" + order111Json + "]}", new OrdersWriter(_orders).GetContents());
        }

        [Test]
        public void OneOrderWithOneProductNoSize()
        {
            _order111.AddProduct(new Product("Pot", 2, -1, 16.50, "SGD"));

            var order111Json = JsonOrder111WithProduct("{\"code\": \"Pot\", \"color\": \"red\", \"price\": 16.5, \"currency\": \"SGD\"}");
            Assert.AreEqual("{\"orders\": [" + order111Json + "]}", new OrdersWriter(_orders).GetContents());
        }

        [Test]
        public void TwoOrders()
        {
            _orders.AddOrder(new Order(222));

            var order111Json = JsonOrder111WithProduct("");
            var order222Json = "{\"id\": 222, \"products\": []}";
            Assert.AreEqual("{\"orders\": [" + order111Json + ", " + order222Json + "]}", new OrdersWriter(_orders).GetContents());
        }

        private string JsonOrder111WithProduct(string productJson)
        {
            return "{\"id\": 111, \"products\": [" + productJson + "]}";
        }
    }
}