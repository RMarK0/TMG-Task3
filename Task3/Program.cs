using System;
using System.Collections.Generic;
using System.IO;

namespace Task3
{
    class TextIndexPair : IComparable<TextIndexPair>
    {
        internal readonly string Text;
        internal double Index;
        internal double CommentIndex;
        internal readonly string Language;
        internal readonly string Comment;

        public TextIndexPair(string text, string language)
        {
            Text = text;
            Index = 0;
            CommentIndex = 0;
            Language = language;
            Comment = null;
        }

        public TextIndexPair(string text, string language, string comment)
        {
            Text = text;
            Index = 0;
            CommentIndex = 0;
            Language = language;
            Comment = comment;
        }

        /// <summary>
        /// Implementation of comparing pairs between each other
        /// </summary>
        /// <param name="other"> Element, with which we need to compare our current element </param>
        /// <returns> 1 if OTHER is more than CURRENT
        /// 0 if OTHER equals to CURRENT
        /// -1 if OTHER is less than CURRENT </returns>
        public int CompareTo(TextIndexPair other)
        {
            if (Language == "ru")
            {
                if (other.Index > Index)
                    return 1;
                if (Math.Abs(other.Index - Index) < 0.01)
                    return 0;
                return -1;
            }

            if (Language == "en")
            {
                if (other.Index + other.CommentIndex > Index + CommentIndex)
                    return 1;
                if (Math.Abs(other.Index + other.CommentIndex - (Index + CommentIndex)) < 0.01)
                    return 0;
                return -1;
            }
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (Comment == null)
                return $"--> {Text} ({Index})";
            return $"--> {Text} ({Index}) {Comment} ({CommentIndex})";
        }
    }

    static class ExtensionClass
    {
        /// <summary>
        /// Method that gets english and russian strings from .txt files
        /// </summary>
        /// <param name="pathRu"> Path for file with russian strings </param>
        /// <param name="pathEn"> Path for file with english strings </param>
        /// <param name="stringsRu"> List of russian strings </param>
        /// <param name="stringsEn"> List of english strings </param>
        public static void GetStringsFromFile(string pathRu, string pathEn, List<string> stringsRu, List<string> stringsEn)
        {
            StreamReader streamReaderRu = new StreamReader(pathRu);
            StreamReader streamReaderEn = new StreamReader(pathEn);

            try
            {
                string line;
                while((line = streamReaderRu.ReadLine()) != null)
                    stringsRu.Add(line);

                line = null;
                while((line = streamReaderEn.ReadLine()) != null)
                    stringsEn.Add(line);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Method that processes the input, splits english string into text and comment and adds russian and english inputs to lists of pairs
        /// </summary>
        /// <param name="stringsRu"> List of russian strings </param>
        /// <param name="stringsEn"> List of english strings </param>
        /// <param name="pairsRu"> List of russian (Index+Text) pairs </param>
        /// <param name="pairsEn"> List of english (Index+Text) AND (CommentIndex+Comment) pairs</param>
        public static void ProcessInput(List<string> stringsRu, List<string> stringsEn, List<TextIndexPair> pairsRu, List<TextIndexPair> pairsEn)
        {
            foreach (string str in stringsRu)
            {
                TextIndexPair pairRu = new TextIndexPair(str, "ru");
                pairsRu.Add(pairRu);
            }

            foreach (string str in stringsEn)
            {
                string[] subStrings = str.Split('|');
                TextIndexPair pairEn = new TextIndexPair(subStrings[0], "en", subStrings[1]);
                pairsEn.Add(pairEn);
            }
        }

        /// <summary>
        /// Method that calculates Petrenko-Goltzman index for texts and comments in given pairs
        /// </summary>
        /// <param name="pair"> Input pair of text + index </param>
        public static void CalculateIndex(TextIndexPair pair)
        {

            double index = 0.5;
            int lengthText = 0;
            int lengthComment = 0;
            switch (pair.Language)
            {
                case ("ru"):
                    foreach (char ch in pair.Text)
                        if (Char.IsLetter(ch))
                        {
                            pair.Index += index;
                            index++;
                            lengthText++;
                        }
                    
                    pair.Index *= lengthText;
                    break;

                case ("en"):
                    foreach (char ch in pair.Text)
                        if (Char.IsLetter(ch))
                        {
                            pair.Index += index;
                            index++;
                            lengthText++;
                        }
                    
                    pair.Index *= lengthText;

                    index = 0.5;
                    foreach (char ch in pair.Comment)
                        if (Char.IsLetter(ch))
                        {
                            pair.CommentIndex += index;
                            index++;
                            lengthComment++;
                        }
                    
                    pair.CommentIndex *= lengthComment;
                    break;

                default:
                    throw new NotImplementedException("Language was not russian or english");
            }
        }

        /// <summary>
        /// Method that searches for the same Petrenko-Goltzman values in given array
        /// </summary>
        /// <param name="target"> Target Petrenko-Goltzman index value </param>
        /// <param name="pairs"> List of english (Index+Text) AND (CommentIndex+Comment) pairs </param>
        /// <returns></returns>
        public static int BinarySearch(double target, List<TextIndexPair> pairs)
        {
            int left = 0;
            int right = pairs.Count - 1;
            int middle = (right + left) / 2;

            while (left < right - 1)
            {
                middle = (right + left) / 2;
                if (Math.Abs(pairs[middle].Index + pairs[middle].CommentIndex - target) < 0.01)
                    return middle;

                if (pairs[middle].Index + pairs[middle].CommentIndex < target)
                    left = middle;
                else
                    right = middle;
            }
            if (Math.Abs(pairs[middle].Index + pairs[middle].CommentIndex - target) > 0.01)
            {
                if (Math.Abs(pairs[middle].Index + pairs[middle].CommentIndex - target) < 0.01)
                    middle = left;
                else
                {
                    if (Math.Abs(pairs[middle].Index + pairs[middle].CommentIndex - target) < 0.01)
                        middle = right;
                    else
                        middle = -1;
                }
            }
            return middle;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<string> stringsRu = new List<string>();
            List<string> stringsEn = new List<string>();

            List<TextIndexPair> pairsRu = new List<TextIndexPair>();
            List<TextIndexPair> pairsEn = new List<TextIndexPair>();

            try
            {
                Console.Write("Enter path for russian strings: ");
                var pathRu = Console.ReadLine();
                Console.Write("Enter path for english strings: ");
                var pathEn = Console.ReadLine();

                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                ExtensionClass.GetStringsFromFile(pathRu, pathEn, stringsRu, stringsEn);

                ExtensionClass.ProcessInput(stringsRu, stringsEn, pairsRu, pairsEn);
                if (pairsRu.Count > 0 && pairsEn.Count > 0)
                {
                    foreach (TextIndexPair pair in pairsRu)
                        ExtensionClass.CalculateIndex(pair);
                    
                    foreach (TextIndexPair pair in pairsEn)
                        ExtensionClass.CalculateIndex(pair);

                    pairsRu.Sort();
                    pairsEn.Sort();

                    foreach (TextIndexPair pair in pairsRu)
                    {
                        int index = ExtensionClass.BinarySearch(pair.Index, pairsEn);
                        if (index == -1)
                            Console.WriteLine($"{pair}\nNo alternative pair\n");
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{pair}\n{pairsEn[ExtensionClass.BinarySearch(pair.Index, pairsEn)]}\n");
                            Console.ResetColor();
                        }
                    }

                    watch.Stop();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
                    Console.Read();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
