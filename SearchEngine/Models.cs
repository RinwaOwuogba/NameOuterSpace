using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.IO;
using LiteDB;

namespace SearchEngine{

    /// <summary>
    ///     Represents A collection of Details in the Database
    /// </summary>
    public class MetaDetails
    {   
        /// <summary>
        ///     The Id that will be used to access a document
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Path to the repo where the documents will be stored
        /// </summary>
        public string repositoryPath{ get; set; }
        
        /// <summary>
        ///     The number of documents that have been indexed so far
        /// </summary>
        public int indexedDocumentCount { get; set; }
        
        /// <summary>
        ///     Keeps track of the last time the repo was traversed
        /// </summary>
        public DateTime? lastRepoTraverseTime { get; set; }

        /// <summary>
        ///     A list of stopwords
        /// </summary>
        public List<string> stopWords { get; set; }
    }
    /// <summary>
    ///     Represents a file in in collection of Files
    /// </summary>
    public class FileDocument{
        public int Id { get; set; }

        /// <summary>
        ///     The filename of the document
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        ///     A hash of the contents of the file
        /// </summary>
        public string MD5Hash { get; set; }

        /// <summary>
        ///     Hash The contents of a file
        /// </summary>
        /// <param name="pathToFile"> the full file path </param>
        /// <returns> An md5hash string </returns>
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

    /// <summary>
    ///     Represents A Word and a list of its associated docs
    ///     The full Collection is the inverted index, a collection of Word Documents
    /// </summary>
    public class WordDocument{

        /// <summary>
        ///     A word in the reverse index
        /// </summary>
        private string word;

        /// <summary>
        ///     A dictionary that contains all the document Ids that have the word in them and the occurence of the word
        /// </summary>
        private Dictionary<int, long> documents;
        
        /// <summary>
        ///     An object Id property used to uniquely identify a document
        /// </summary>
        public ObjectId Id{ get; set; }

        /// <summary>
        ///     Accompanying Word Property for word field
        /// </summary>
        public string Word { get => word; private set => word = value.ToLower(); }

        /// <summary>
        ///     Accompanying Document Property for documents field
        /// </summary>
        public Dictionary<int, long> Documents { get => documents; private set => documents = value; }

        /// <summary>
        ///     1st constructor for WordDocument
        /// </summary>
        /// <param name="word"> the word that will be stored in the index</param>
        public WordDocument(string word){
            Id = ObjectId.NewObjectId();
            this.word = word.ToLower();
            this.Documents = new Dictionary<int, long>();
        }
        

        /// <summary>
        ///     The Constructor that the db uses to initialise a WordDoc object
        /// </summary>
        /// <param name="_id">unique identifier</param>
        /// <param name="word">the word that wiil be stored in the index </param>
        /// <param name="doc"> a dictionary of Docids and the word's occurence</param>
        [BsonCtor]
        public WordDocument(ObjectId _id, string word, Dictionary<int, long> doc){
            Id = _id;
            this.Word = word;
            this.Documents = doc;
        }

        /// <summary>
        ///     remove a doc id from the dictionary
        /// </summary>
        /// <param name="docId"> a docid that maps to a document in the db</param>
        public void RemoveDoc(int docId){
            Documents.Remove(docId);
        }

        /// <summary>
        ///     add a doc id to the dictionary
        /// </summary>
        /// <param name="docId">a docid that maps to a document in the db</param>
        /// <param name="occurences"> the number of times the word appears in the doc</param>
        public void AddDoc(int docId, long occurences){
            Documents.Add(docId, occurences);
        }

        public override string ToString()
        {
            string output = "";
            output += Word + "\n";
            foreach(var x in Documents.Keys){
                output += "docid: " + x + " occurence:" + Documents[x] + "\n";
            }
            return output;
        }

    }
}