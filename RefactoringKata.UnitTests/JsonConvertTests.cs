using NUnit.Framework;

namespace RefactoringKata.UnitTests
{
    [TestFixture]
    public class JsonConvertTests
    {
        [Test]
        public void SerializeTest_ObjectHasProperties()
        {
            var product = new PropertyObject {Number = 3, DoubleNumber = 2.2, Name = "Ya"};
            var expected = "{\"number\": 3, \"doublenumber\": 2.2, \"name\": \"Ya\"}";

            Assert.AreEqual(expected, product.JsonSerialize());
        }

        [Test]
        public void SerializeTest_ShouldSerializeProperties()
        {
            var product = new ShouldSerializeOjbect { NotSerializeProp = "SHOULD NOT EXIST", ShouldSerializeProp = "exist"};
            var expected = "{\"shouldserializeprop\": \"exist\"}";

            Assert.AreEqual(expected, product.JsonSerialize());
        }

        [Test]
        public void SerializeTest_JsonPropertyAttribute()
        {
            var product = new JsonPropertyObject {Name = "Name"};
            var expected = "{\"jsonname\": \"Name\"}";

            Assert.AreEqual(expected, product.JsonSerialize());
        }
    }

    internal class PropertyObject
    {
        public int Number { get; set; }
        public double DoubleNumber { get; set; }
        public string Name { get; set; }
    }

    internal class ShouldSerializeOjbect{
        public string NotSerializeProp { get; set; }
        public string ShouldSerializeProp { get; set; }

        public bool ShouldSerializeNotSerializeProp()
        {
            return false;
        }

        public bool ShouldSerializeShouldSerializeProp()
        {
            return true;
        }

    }

    internal class SelectObject
    {
        public int SelectedProp { get; set; }

        public string SelectSelectedProp()
        {
            return "Selected string";
        }
    }

    internal class JsonPropertyObject
    {
        [JsonProperty("JsonName")]
        public string Name { get; set; }
    }
}
