using Moq;
using NUnit.Framework;

namespace NumberNamesKata
{
    [TestFixture]
    public class NumbersToNamesTests
    {
        [TestCase(1, "one")]
        [TestCase(2, "two")]
        [TestCase(3, "three")]
        [TestCase(4, "four")]
        [TestCase(5, "five")]
        [TestCase(6, "six")]
        [TestCase(7, "seven")]
        [TestCase(8, "eight")]
        [TestCase(9, "nine")]
        [TestCase(10, "ten")]
        [TestCase(11, "eleven")]
        [TestCase(12, "twelve")]
        [TestCase(13, "thirteen")]
        [TestCase(14, "fourteen")]
        [TestCase(15, "fifteen")]
        [TestCase(16, "sixteen")]
        [TestCase(17, "seventeen")]
        [TestCase(18, "eighteen")]
        [TestCase(19, "nineteen")]
        public void GivenUnitsBelowTwentyWhenConvertingThenTheCorrectNameIsReturned(int number, string expectedName)
        {
            var mockUnitRetriever = new Mock<IUnitNameRetriever>();
            mockUnitRetriever
                .Setup(retriever => retriever.GetName(It.IsAny<int>()))
                .Returns(expectedName);

            var numberToNameConverter = new NumberToNameConverter(mockUnitRetriever.Object);

            var actualName = numberToNameConverter.GetName(number);

            Assert.That(actualName, Is.EqualTo(expectedName));

            mockUnitRetriever.Verify(retriever => retriever.GetName(number));
        }

        [TestCase(20, "twenty")]
        [TestCase(30, "thirty")]
        [TestCase(40, "fourty")]
        [TestCase(50, "fifty")]
        [TestCase(60, "sixty")]
        [TestCase(70, "seventy")]
        [TestCase(80, "eighty")]
        [TestCase(90, "ninety")]
        public void GivenTensWhenConvertingThenTheCorrectNameIsReturned(int number, string expectedName)
        {
            var mockUnitRetriever = new Mock<IUnitNameRetriever>();

            var numberToNameConverter = new NumberToNameConverter(mockUnitRetriever.Object);

            Assert.That(numberToNameConverter.GetName(number), Is.EqualTo(expectedName));
        }

        [TestCase(100, "one")]
        [TestCase(200, "two")]
        [TestCase(300, "three")]
        public void GivenHundredsWhenConvertingThenTheCorrectNameIsReturned(int number, string unitName)
        {
            var expectedName = string.Format("{0} hundred", unitName);

            var mockUnitRetriever = new Mock<IUnitNameRetriever>();
            mockUnitRetriever
                .Setup(retriever => retriever.GetName(It.IsAny<int>()))
                .Returns(unitName);

            var numberToNameConverter = new NumberToNameConverter(mockUnitRetriever.Object);

            Assert.That(numberToNameConverter.GetName(number), Is.EqualTo(expectedName));

            mockUnitRetriever.Verify(retriever => retriever.GetName(number / 100));
        }

        [TestCase(1000, "one")]
        [TestCase(2000, "two")]
        [TestCase(3000, "three")]
        public void GivenThousandsWhenConvertingThenTheCorrectNameIsReturned(int number, string unitName)
        {
            var expectedName = string.Format("{0} thousand", unitName);

            var mockUnitRetriever = new Mock<IUnitNameRetriever>();
            mockUnitRetriever
                .Setup(retriever => retriever.GetName(It.IsAny<int>()))
                .Returns(unitName);

            var numberToNameConverter = new NumberToNameConverter(mockUnitRetriever.Object);

            Assert.That(numberToNameConverter.GetName(number), Is.EqualTo(expectedName));

            mockUnitRetriever.Verify(retriever => retriever.GetName(number / 1000));
        }
    }

    public interface IUnitNameRetriever
    {
        string GetName(int number);
    }

    public class NumberToNameConverter
    {
        private readonly IUnitNameRetriever _unitNameRetriever;

        private static readonly string[] Tens = new[]
            {
                "", "","twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninety"
            };

        private static readonly string[] Units = new[]
            {
                "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
            };

        public NumberToNameConverter(IUnitNameRetriever unitNameRetriever)
        {
            _unitNameRetriever = unitNameRetriever;
        }

        private const string Hundred = "hundred", 
                             Thousand = "thousand";

        public string GetName(int number)
        {
            if (number >= 1000)
                return GetNameForThousands(number);

            if (number >= 100)
                return GetNameForHundreds(number);

            return number < Units.Length
                ? _unitNameRetriever.GetName(number)
                : Tens[number / 10];
        }

        private string GetNameForHundreds(int number)       // GetNameForHundreds and GetNameforThousands are pretty much identical other than number to divide by plus name - could 
                                                            // be same dependency??
        {
            var unitIndex = number / 100;

            var unitName = _unitNameRetriever.GetName(unitIndex);

            return string.Format("{0} {1}", unitName, Hundred);
        }

        private string GetNameForThousands(int number)
        {
            var unitIndex = number / 1000;

            var unitName = _unitNameRetriever.GetName(unitIndex);

            return string.Format("{0} {1}", unitName, Thousand);
        }
    }
}
