using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.IO;
using LiteDB;

namespace SearchEngine{
    public class MetaDetails
    {
        public int Id { get; set; }
        public string repositoryPath{ get; set; }
        public int indexedDocumentCount { get; set; }
        public DateTime? lastRepoTraverseTime { get; set; }
        public List<string> stopWords { get; set; }
    }

    public class FileDocument{
        public int Id { get; set; }
        public string Filename { get; set; }
        public string MD5Hash { get; set; }

        public static string CalculateMD5Hash(string pathToFile)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(pathToFile))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }

    public class WordDocument{
        private string word;
        public Dictionary<int, int> documents;

        public int Id{ get; set; }
        public string Word { get => word;}
        public Dictionary<int, int> Documents { get => documents; }

        public WordDocument(string word){
            this.word = word.ToLower();
            this.documents = new Dictionary<int, int>();
        }

        public void RemoveDoc(int docId){
            Documents.Remove(docId);
        }

        public void AddDoc(int docId, int occurences){
            Documents.Add(docId, occurences);
        }

    }
}