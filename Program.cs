using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerHandEvaluator
{
    public class Program
    {
        public static void Main()
        {
            var parser = new HandParser();
            Console.WriteLine("Enter your hand (e.g., AS,3S,5S,9S,KS):");
            string input = Console.ReadLine();
            Console.WriteLine(parser.GetHandName(input));
        }
    }

    public class HandParser
    {
        private readonly AToBConverter<char, Rank> _rankConverter = new AToBConverter<char, Rank>(_rankConversions);
        private readonly AToBConverter<char, Suit> _suitConverter = new AToBConverter<char, Suit>(_suitConversions); // Added this, parallel to the rank one

        private static readonly IDictionary<char, Suit> _suitConversions = new Dictionary<char, Suit> // Added this, parallel to the rank one
        {
            { 'S', Suit.Spades },
            { 'D', Suit.Diamonds },
            { 'H', Suit.Hearts },
            { 'C', Suit.Clubs }
        };

        private static readonly IDictionary<char, Rank> _rankConversions = new Dictionary<char, Rank>
        {
            { 'A', Rank.Ace },
            { '2', Rank.Two },
            { '3', Rank.Three },
            { '4', Rank.Four },
            { '5', Rank.Five },
            { '6', Rank.Six },
            { '7', Rank.Seven },
            { '8', Rank.Eight },
            { '9', Rank.Nine },
            { 'T', Rank.Ten },
            { 't', Rank.Ten },
            { 'J', Rank.Jack },
            { 'Q', Rank.Queen },
            { 'O', Rank.Queen },
            { 'K', Rank.King }
        };

        public string GetHandName(string input)
        {
            var splitter = new StringSplitter();
            splitter.AddCharacterToSplitOn(',');
            IEnumerable<string> tokens = splitter.Split(input);

            // Ensure each token is exactly 2 characters.
            if (tokens.Any(token => token.Length != 2))
                return "Error";

            // Convert tokens into Card objects safely.
            var cards = new List<Card<Suit, Rank>>();
            foreach (var token in tokens)
            {
                var rank = ConvertRank(token[0]);
                var suit = ConvertSuit(token[1]);
                if (!rank.HasValue || !suit.HasValue)
                {
                    // If conversion fails for either the rank or suit, return an error.
                    return "Error";
                }
                cards.Add(new Card<Suit, Rank>(suit.Value, rank.Value));
            }

            // Must have at least 2 cards.
            if (cards.Count < 2)
                return "Error";

            // Invalid if there are five cards of the same rank.
            if (cards.GroupBy(c => c.Rank).Any(g => g.Count() == 5))
                return "Error";

            // Define matchers for different hand types.
            IMatch<Suit, Rank> twoOfAKind = new TwoOfAKind<Suit, Rank>();
            IMatch<Suit, Rank> threeOfAKind = new ThreeOfAKind<Suit, Rank>();
            IMatch<Suit, Rank> flush = new Flush<Suit, Rank>();

            // Check for hand types in order (highest ranking first).
            if (flush.IsMatch(cards))
            {
                return "Flush";
            }
            else if (threeOfAKind.IsMatch(cards))
            {
                return "ThreeOfAKind";
            }
            else if (twoOfAKind.IsMatch(cards))
            {
                return "Pair";
            }

            return "No Match";
        }

        private Rank? ConvertRank(char input) // Added this to access later
        {
            return _rankConverter.Convert(input);
        }

        private Suit? ConvertSuit(char input)
        {
            return _suitConverter.Convert(input);
        }
    }

    public interface IMatch<TSuit, TRank>
    {
        bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards);
    }

    public class TwoOfAKind<TSuit, TRank> : IMatch<TSuit, TRank>
    {
        public bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards)
        {
            // Returns true if any rank appears exactly twice.
            return cards.GroupBy(c => c.Rank).Any(g => g.Count() == 2);
        }
    }

    public class Flush<TSuit, TRank> : IMatch<TSuit, TRank>
    {
        public bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards)
        {
            // Returns true if all cards are of the same suit.
            return cards.GroupBy(c => c.Suit).Any(g => g.Count() == cards.Count());
        }
    }

    public class ThreeOfAKind<TSuit, TRank> : IMatch<TSuit, TRank>
    {
        public bool IsMatch(IEnumerable<Card<TSuit, TRank>> cards)
        {
            // Returns true if any rank appears exactly three times.
            return cards.GroupBy(c => c.Rank).Any(g => g.Count() == 3);
        }
    }

    public enum Rank
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public enum Suit
    {
        Hearts,
        Spades,
        Diamonds,
        Clubs
    }

    public class Card<TSuit, TRank>
    {
        public Card(TSuit suit, TRank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public TSuit Suit { get; private set; }
        public TRank Rank { get; private set; }
    }

    public class AToBConverter<TA, TB> where TB : struct
    {
        private readonly IDictionary<TA, TB> _conversions;

        public AToBConverter(IDictionary<TA, TB> conversions)
        {
            _conversions = conversions;
        }

        public TB? Convert(TA input)
        {
            if (_conversions.TryGetValue(input, out TB output))
            {
                return output;
            }
            return null;
        }
    }

    public class StringSplitter
    {
        private readonly IList<char> _splitChars = new List<char>();

        public void AddCharacterToSplitOn(char splitCharacter)
        {
            _splitChars.Add(splitCharacter);
        }

        public IEnumerable<string> Split(string input)
        {
            return input.Split(_splitChars.ToArray())
                        .Where(s => !string.IsNullOrEmpty(s));
        }
    }
}
