using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerHandEvaluator; // Make sure this matches your main project's namespace

namespace PokerHandEvaluator.Tests
{
    [TestClass]
    public class HandParserTests
    {
        private HandParser parser;

        [TestInitialize]
        public void Setup()
        {
            parser = new HandParser();
        }

        [TestMethod]
        public void TestFlush()
        {
            // All cards are of the same suit.
            string input = "AS,3S,5S,9S,KS";
            string expected = "Flush";
            string result = parser.GetHandName(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestThreeOfAKind()
        {
            // Three Aces in the hand.
            string input = "AS,AH,AD,7D,KC";
            string expected = "ThreeOfAKind";
            string result = parser.GetHandName(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPair()
        {
            // A pair of Aces.
            string input = "AS,AH,4D,7D,KC";
            string expected = "Pair";
            string result = parser.GetHandName(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNoMatch()
        {
            // No pattern is found.
            string input = "AS,2H,3D,7C,KC";
            string expected = "No Match";
            string result = parser.GetHandName(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestInvalidTokenLength()
        {
            // Token with an invalid length.
            string input = "A,AH,4D,7D,KC";
            string expected = "Error";
            string result = parser.GetHandName(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTooManySameRank()
        {
            // Five cards of the same rank should return "Error".
            string input = "AS,AH,AD,AC,AK";
            string expected = "Error";
            string result = parser.GetHandName(input);
            Assert.AreEqual(expected, result);
        }
    }
}
