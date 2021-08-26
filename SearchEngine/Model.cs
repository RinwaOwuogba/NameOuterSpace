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
        private Dictionary<int, int> documents;

        public ObjectId Id{ get; set; }
        public string Word { get => word; private set => word = value.ToLower(); }
        public Dictionary<int, int> Documents { get => documents; private set => documents = value; }

        public WordDocument(string word){
            Id = ObjectId.NewObjectId();
            this.word = word.ToLower();
            this.Documents = new Dictionary<int, int>();
        }
        
        [BsonCtor]
        public WordDocument(ObjectId _id, string word, Dictionary<int, int> doc){
            Id = _id;
            this.Word = word;
            this.Documents = doc;
        }

        public void RemoveDoc(int docId){
            Documents.Remove(docId);
        }

        public void AddDoc(int docId, int occurences){
            Documents.Add(docId, occurences);
        }

        public override string ToString()
        {
            string output = "";
            output += Word + "\n";
            foreach(var x in Documents.Keys){
                output += "docid: " + x + " occurence:" + Documents[x] + "\n";
            }
            return output + "\n";
        }

    }
}