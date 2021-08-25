using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using LiteDB;

namespace SearchEngine
{

    public class Engine
    {
        private LiteDatabase db;

        private ILiteCollection<MetaDetails> metaCollection;
        private ILiteCollection<WordDocument> invertedIndex;
        private ILiteCollection<FileDocument> documentCollection;

        private string connectionString;
        private string pathToRepository;
        private string pathToStopWords;
        public Engine(
            string pathToRepo,
            string connectionString = "database.db",
            string pathToStopWords = "stopwords.txt"
            )
        {
            this.pathToRepository = pathToRepo;
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

        private List<string> loadStopWords(string pathtostopwords){
            var stopwords = new List<string>(System.IO.File.ReadAllLines(pathtostopwords));
            return stopwords;
        }

        public MetaDetails GetMetaInfo(){
            return metaCollection.FindById(1);
        }

        public void UpdateMetaInfo(MetaDetails updatedmeta){
            var meta = metaCollection.FindById(1);
            meta.indexedDocumentCount = updatedmeta.indexedDocumentCount;
            meta.lastRepoTraverseTime = updatedmeta.lastRepoTraverseTime;
            meta.repositoryPath = updatedmeta.repositoryPath;
            meta.stopWords = updatedmeta.stopWords;
            metaCollection.Update(meta);
        }

        public List<FileDocument> GetAllDocuments(){
            documentCollection.EnsureIndex("Filename");
            var documents = new List<FileDocument>(documentCollection.Find(Query.All("Filename")));
            return documents;
        }
        
        public List<FileDocument> GetDocuments(List<int> ids){
            var matcheddocs = new List<FileDocument>(documentCollection.Find(x => ids.Contains(x.Id)));
            return matcheddocs;
        }

        public List<FileDocument> GetDocuments(List<string> names){
            documentCollection.EnsureIndex("Filename");
            var matcheddocs = new List<FileDocument>(documentCollection.Find(x => names.Contains(x.Filename)));
            return matcheddocs;
        }

        public FileDocument GetDocument(int id){
            return documentCollection.FindById(id);
        }

        public FileDocument GetDocument(string filename){
            documentCollection.EnsureIndex("Filename");
            return documentCollection.FindOne(Query.EQ("Filename", filename));
        }       
        
        public int AddDocument(string filename){
            var document = new FileDocument()
            {
                Filename = filename,
                MD5Hash = FileDocument.CalculateMD5Hash(this.pathToRepository + filename)
            };
            documentCollection.Insert(document);
            var meta = GetMetaInfo();
            meta.indexedDocumentCount++;
            UpdateMetaInfo(meta);
            return document.Id;
        }
        public void DeleteDocument(int id){
            documentCollection.Delete(id);
            DeleteDocumentReferencesFromInvertedIndex(id);
            var meta = GetMetaInfo();
            meta.indexedDocumentCount--;
            UpdateMetaInfo(meta);
        }

        public void DeleteDocument(string filename){
            DeleteDocument(GetDocument(filename).Id);
        }

        public void DeleteDocumentReferencesFromInvertedIndex(int docId){
            IEnumerable<WordDocument> relevantWordDocs = invertedIndex
                                                        .Find(x => x.Documents.ContainsKey(docId));
            foreach(var worddoc in relevantWordDocs){
                worddoc.RemoveDoc(docId);
            }
            invertedIndex.Update(relevantWordDocs);
        }
       
        public void UpdateDocumentName(string newFilename){

        }

        public List<string> GetAllWords(){
            invertedIndex.EnsureIndex("Word");
            var words = new List<string>();
            words = invertedIndex.Find(Query.All("Word")).Select(x => x.Word).ToList<string>();
            return words;
        }

        public WordDocument GetWordDocument(string word){
            invertedIndex.EnsureIndex("Word");
            var worddoc = invertedIndex.FindOne(Query.EQ("Word", word.ToLower()));
            return worddoc;
        }

        public List<WordDocument> GetWordDocuments(List<string> words){
            invertedIndex.EnsureIndex("Word");
            var worddocs = new List<WordDocument>();
            worddocs = invertedIndex.Find(x => words.Select(y => y.ToLower())
                                                    .Contains(x.Word)).ToList<WordDocument>();
            return worddocs;
        }
        
        public void AddWordDocument(int docId, Dictionary<string,int> words){
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

        public void DeleteWord(string word){
            invertedIndex.Delete(GetWordDocument(word).Id);

        }
        public void Kill(){
            db.DropCollection(invertedIndex.Name);
            db.DropCollection(metaCollection.Name);
            db.DropCollection(documentCollection.Name);
        }
    }
}