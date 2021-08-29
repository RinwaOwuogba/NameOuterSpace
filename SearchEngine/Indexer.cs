using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
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
        /// Stop words to skip in forward index
        /// </summary>
        private HashSet<string> stopWords;

        public Indexer(HashSet<string> stopWords)
        {
            this.stopWords = stopWords;
        }

        /// <summary>
        /// Generates the forward index for a file
        /// </summary>
        /// <returns>Dictionary containing file forward index</returns>
        /// <param name="filePath">Full path to file to index</param>
        public Dictionary<string, long> IndexFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            Parser parser = AutoDetectParser.GetContextParser(filePath);
            string fileContent = fileName + " " + parser.Parse();

            Dictionary<string, long> index = this.IndexText(fileContent);

            return index;
        }

        /// <summary>
        /// Generates forward index of a string of text
        /// </summary>
        /// <param name="text">Text to index</param>
        /// <param name="stopWords">Stop words to remove from index</param>
        /// <returns>Dictionary containing string forward index</returns>
        public Dictionary<string, long> IndexText(string text)
        {
            // splits text string into individual tokens
            string[] tokens = Regex.Split(
                Regex.Replace(text,
                    "[^a-zA-Z0-9']",
                    " "
                )
                .Trim(),
                "\\s+"
            );

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

                if (this.stopWords.Contains(word))
                {
                    continue;
                }

                filteredWords.Add(word.ToLower());
            }

            // create forward index
            Dictionary<string, long> forwardIndex = new Dictionary<string, long>();

            foreach (string word in filteredWords)
            {
                long numberOfOccurences = 0;

                if (forwardIndex.TryGetValue(word, out numberOfOccurences))
                {
                    forwardIndex[word] = numberOfOccurences + 1;
                }
                else
                {
                    forwardIndex.Add(word, 1);
                };
            }

            return forwardIndex;
        }
    }
}