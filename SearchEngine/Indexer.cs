using System;
using System.Collections.Generic;
using System.IO;
using OpenNLP.Tools.Tokenize;
using Porter2Stemmer;

namespace SearchEngine
{
    /// <summary>
    /// Class to compute the reverse index of a file's
    /// text content
    /// </summary>
    public class Indexer
    {
        /// <summary>
        /// Full path to file
        /// </summary>
        private string filePath;

        /// <summary>
        /// Name of file to index
        /// </summary>
        private string fileName;

        /// <summary>
        /// Text content of file
        /// </summary>
        private string text;

        /// <summary>
        /// Stop words to skip in reverse   index
        /// </summary>
        private string[] stopWords;

        public Indexer(string filePath, string[] stopWords)
        {
            this.fileName = Path.GetFileNameWithoutExtension(filePath);
            this.filePath = filePath;
            this.stopWords = stopWords;
        }

        public Dictionary<string, long> Index()
        {
            Parser parser = AutoDetectParser.GetContextParser(this.filePath);

            this.text = this.fileName + " " + parser.Parse();

            // create tokens from text string
            EnglishRuleBasedTokenizer tokenizer = new EnglishRuleBasedTokenizer(true);
            string[] tokens = tokenizer.Tokenize(this.text);

            // stem tokens to root word e.g
            // happier -> happy
            EnglishPorter2Stemmer stemmer = new EnglishPorter2Stemmer();
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = stemmer.Stem(tokens[i]).Value;
            }

            List<string> filteredWords = new List<string>();

            // remove all stop words
            for (int i = 0; i < tokens.Length; i++)
            {
                string word = tokens[i];

                if (Array.Exists<string>(
                    this.stopWords,
                    stopWord => stopWord == word
                ))
                {
                    continue;
                }

                filteredWords.Add(word.ToLower());
            }

            // create reverse index
            Dictionary<string, long> reverseIndex =
                new Dictionary<string, long>();

            foreach (string word in filteredWords)
            {
                long numberOfOccurences = 0;

                if (reverseIndex.TryGetValue(word, out numberOfOccurences))
                {
                    reverseIndex[word] = numberOfOccurences + 1;
                }
                else
                {
                    reverseIndex.Add(word, 1);
                };
            }

            return reverseIndex;
        }

    }
}