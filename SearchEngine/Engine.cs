using System;
using System.IO;
using System.Transactions;
using System.Linq;
using System.Collections.Generic;
using LiteDB;

namespace SearchEngine
{

    /// <summary>
    ///     A Singleton Class That interfaces between the file database and other parts of the library
    /// </summary>
    public class Engine:IEngine
    {
        private LiteDatabase db;

        private ILiteCollection<MetaDetails> metaCollection;
        private ILiteCollection<WordDocument> invertedIndex;
        private ILiteCollection<FileDocument> documentCollection;

        /// <summary>
        ///     connection string for the db instance
        /// </summary>
        private string connectionString;
        /// <summary>
        ///     file path to where the documents that will be indexed are stored
        /// </summary>
        private string pathToRepository;
        /// <summary>
        ///     Path to stop words
        /// </summary>
        private string pathToStopWords;

        /// <summary>
        ///      Initialises a connection to the db
        ///      Creates the Relevant Collections in the Db if they do not yet exist
        ///      Creates The relevant meta information if it does not already exist
        /// </summary>
        /// <param name="pathToRepo"> string path to repository where to be indexed files are stored</param>
        /// <param name="connectionString"> optional string arguement that denotes the path the db should store the local db in</param>
        /// <param name="pathToStopWords"> optional string argument, path to stop words file</param>
        public Engine(
            string pathToRepo,
            string connectionString = "database.db",
            string pathToStopWords = "stopwords.txt"
            )
        {
            this.pathToRepository = pathToRepo;
            this.connectionString = connectionString;
            this.pathToStopWords = pathToStopWords;

            db = new LiteDatabase(connectionString);
            metaCollection = db.GetCollection<MetaDetails>("Meta");
            documentCollection = db.GetCollection<FileDocument>("Document");
            invertedIndex = db.GetCollection<WordDocument>("InvertedIndex");

            var metainfo = metaCollection.FindById(1);
            if (metainfo == null)
            {
                metainfo = new MetaDetails()
                {
                    Id = 1,
                    repositoryPath = pathToRepository,
                    indexedDocumentCount = 0,
                    stopWords = loadStopWords(pathToStopWords),
                    lastRepoTraverseTime = null
                };
                metaCollection.Insert(metainfo);
            }
            else if(metainfo.repositoryPath != pathToRepository){
                metainfo.repositoryPath = pathToRepository;
                metainfo.indexedDocumentCount = 0;
                metainfo.lastRepoTraverseTime = null;
                metainfo.stopWords = loadStopWords(pathToStopWords);
                UpdateMetaInfo(metainfo);
            }
            else{
                metainfo.stopWords = loadStopWords(pathToStopWords);
                UpdateMetaInfo(metainfo);
            }

        
        }
        
        /// <summary>
        ///     utility method that gets the stop from a file and loads into memory
        /// </summary>
        /// <param name="pathtostopwords"> string; file path to stopwords txt</param>
        /// <returns> A List of stop words </returns>
        private List<string> loadStopWords(string pathtostopwords){
            var stopwords = new List<string>(System.IO.File.ReadAllLines(pathtostopwords));
            return stopwords;
        }

        /// <summary>
        ///     Gets meta info from the db
        /// </summary>
        /// <returns> A meta details Object that holds all the relevant config data</returns>
        public MetaDetails GetMetaInfo(){
            return metaCollection.FindById(1);
        }

        /// <summary>
        ///     updates the meta details in db to reflect the updated meta details object
        /// </summary>
        /// <param name="updatedmeta"> the meta details object with updated properties</param>
        public void UpdateMetaInfo(MetaDetails updatedmeta){
            var meta = metaCollection.FindById(1);
            meta.indexedDocumentCount = updatedmeta.indexedDocumentCount;
            meta.lastRepoTraverseTime = updatedmeta.lastRepoTraverseTime;
            meta.repositoryPath = updatedmeta.repositoryPath;
            meta.stopWords = updatedmeta.stopWords;
            metaCollection.Update(meta);
        }
        
        /// <summary>
        ///     fetches all the file documents from the db
        /// </summary>
        /// <returns> A List of all the file documents in the collections</returns>
        public List<FileDocument> GetAllDocuments(){
            documentCollection.EnsureIndex("Filename");
            var documents = new List<FileDocument>(documentCollection.Find(Query.All("Filename")));
            return documents;
        }
        
        /// <summary>
        ///     Gets all the file documents that correspond to the ids 
        /// </summary>
        /// <param name="ids"> a list of file document ids</param>
        /// <returns> A list of File documents </returns>
        public List<FileDocument> GetDocuments(HashSet<int> ids){
            var matcheddocs = new List<FileDocument>(documentCollection.Find(x => ids.Contains(x.Id)));
            return matcheddocs;
        }

        /// <summary>
        ///     Gets all the file documents that correspond to the filenames
        /// </summary>
        /// <param name="names"> a list of file names of the documents</param>
        /// <returns> A list of File documents </returns>
        public List<FileDocument> GetDocuments(HashSet<string> names){
            documentCollection.EnsureIndex("Filename");
            var matcheddocs = new List<FileDocument>(documentCollection.Find(x => names.Contains(x.Filename)));
            return matcheddocs;
        }
        
        /// <summary>
        ///     returns a file document that belongs to the id given
        /// </summary>
        /// <param name="id">a file document id</param>
        /// <returns> A single file document</returns>
        public FileDocument GetDocument(int id){
            return documentCollection.FindById(id);
        }

        /// <summary>
        ///     retiurns a file document that belongs to the filename given
        /// </summary>
        /// <param name="filename"> the file name of a document</param>
        /// <returns>A single file document</returns>
        public FileDocument GetDocument(string filename){
            documentCollection.EnsureIndex("Filename");
            return documentCollection.FindOne(Query.EQ("Filename", filename));
        }       

        /// <summary>
        ///     get the number of documents
        /// </summary>
        /// <returns> the number of indexed documents</returns>
        public long GetAllDocumentsCount(){

            return documentCollection.Count(Query.All("Id"));

        }
        
        /// <summary>
        ///     adds a new file to the DB
        /// </summary>
        /// <param name="filename"> the name of the file to be added</param>
        /// <returns>the file id of the newly stored document</returns>
        public int AddDocument(string filename){
            var document = new FileDocument()
            {
                Filename = filename,
                MD5Hash = FileDocument.CalculateMD5Hash(this.pathToRepository + filename)
            };
            documentCollection.Insert(document);
            return document.Id;
        }
   
        /// <summary>
        ///     Deletes the document associated with the id from the database
        /// </summary>
        /// <param name="id">an id belonging to a document</param>
        public void DeleteDocument(int id){
            try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        DeleteDocumentReferencesFromInvertedIndex(id);
                        documentCollection.Delete(id);
                        scope.Complete();
                    }
                }
                catch (TransactionAbortedException ex)
                {
                    Console.WriteLine("oops" + " " + ex);
                }
            
        }

        /// <summary>
        ///     Deletes the document of the specified filename from the db
        /// </summary>
        /// <param name="filename">A file name of a document</param>
        public void DeleteDocument(string filename){
            DeleteDocument(GetDocument(filename).Id);
        }

        /// <summary>
        ///     Removes all instances of the document id from the reverse index
        /// </summary>
        /// <param name="docId">A document document id</param>
        public void DeleteDocumentReferencesFromInvertedIndex(int docId){
            IEnumerable<WordDocument> allWordDocs = invertedIndex
                                                        .FindAll();

            var relevantWordDocs = allWordDocs.Where(x => x.Documents.ContainsKey(docId)).ToList<WordDocument>();
            foreach(var worddoc in relevantWordDocs){
                worddoc.RemoveDoc(docId);
            }

            // relevantWordDocs = relevantWordDocs.Where(x => x.Documents.Count >= 1).ToList<WordDocument>();
            var relevantids = relevantWordDocs.Select(x => x.Id).ToHashSet<ObjectId>();

            invertedIndex.DeleteMany(x => relevantids.Contains(x.Id));
            invertedIndex.Upsert(relevantWordDocs);
        }
       
        /// <summary>
        ///     Fetches all the words in the reverse index
        /// </summary>
        /// <returns> A list of all the words</returns>
        public List<string> GetAllWords(){
            invertedIndex.EnsureIndex("Word");
            var words = new List<string>();
            words = invertedIndex.Find(Query.All("Word")).Select(x => x.Word).ToList<string>();
            return words;
        }

        /// <summary>
        ///     Fetches the worddocument that is associated to a word in the reverse index 
        /// </summary>
        /// <param name="word"> a word in the reverse index</param>
        /// <returns>A WordDocument Object</returns>
        public WordDocument GetWordDocument(string word){
            invertedIndex.EnsureIndex("Word");
            var worddoc = invertedIndex.FindOne(Query.EQ("Word", word.ToLower()));
            return worddoc;
        }

        /// <summary>
        ///     Fetches All the word documents that are associated with the list of words
        /// </summary>
        /// <param name="words"> A List of words in the reverse</param>
        /// <returns>A list of WordDocuments </returns>
        public List<WordDocument> GetWordDocuments(HashSet<string> words){
            invertedIndex.EnsureIndex("Word");
            return invertedIndex.Find(x => words.Where(y => x.Word.StartsWith(y)).Count() > 0).ToList<WordDocument>();
         }
        
        /// <summary>
        ///     integrates a forward index into the reverse index 
        /// </summary>
        /// <param name="docId"> the docid that contains the words</param>
        /// <param name="words"> a dictionary of words in the docid and thier number of occurencess</param>
        public void AddIntoReverseIndex(int docId, Dictionary<string,long> words){
            var additions = new List<WordDocument>();

            invertedIndex.EnsureIndex("Word");
            foreach (String word in words.Keys)
            {

                var worddoc = invertedIndex.FindOne(Query.EQ("Word", word.ToLower()));
                if (worddoc != null)
                {
                    worddoc.AddDoc(docId, words[word]);
                }
                else
                {
                    worddoc = new WordDocument(word);
                    worddoc.AddDoc(docId, words[word]);
                }
                additions.Add(worddoc);

            }
            invertedIndex.Upsert(additions);
        }
        
        /// <summary>
        ///     Gets the number of word documents in the inverted index
        /// </summary>
        /// <returns> The number of word documents in the inverted index</returns>
        public long CountInvertedIndex(){
            return invertedIndex.Count(Query.All("Word"));
        }
        
        /// <summary>
        ///     remove a word's assocaited worddocument from the reverse index  
        /// </summary>
        /// <param name="word"> a word in the reverse index</param>
        public void DeleteWord(string word){
            invertedIndex.Delete(GetWordDocument(word).Id);

        }

        /// <summary>
        ///     Destroys the Database and everything in it
        /// </summary>
        public void Kill(){
            File.Delete(connectionString);
            File.Delete(connectionString.Replace(".db","-log.db"));
            
        }
    }
}