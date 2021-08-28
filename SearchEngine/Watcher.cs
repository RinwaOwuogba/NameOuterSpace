using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Hangfire;
using Hangfire.Server;
using Hangfire.LiteDB;

class d:IBackgroundProcess{
    public void Execute(BackgroundProcessContext context){
        Console.WriteLine("ayooooo");
        context.Wait(TimeSpan.FromMinutes(1));
    }

}

namespace SearchEngine{
public class Watcher
    {
        private string repoBeingWatched = "";
        private int watchedRepoLength = 0;
        private Engine engine;
        public Watcher(Engine eng)
        {
            //GlobalConfiguration.Configuration.UseLiteDbStorage();

            engine = eng;
        }

        // public void Init(){
        //     var client = new BackgroundJobServer();
        //     // RecurringJob.AddOrUpdate(() => Console.WriteLine("gojo gojo"), Cron.Minutely);
        
        // }

        private bool isValidFile(string path)
        {
            var validTypes = new HashSet<string>(new string[] { "pdf", "doc", "docx", "ppt", "ppts", "xls", "xlsx", "txt", "html", "xml" });
            if (!validTypes.Contains(Path.GetExtension(path).Remove(0, 1))) return false;
            return true;
        }



        public HashSet<string> getValidFilesFromRepo(string pathtodirectory){
            var validos = new HashSet<string>();
            foreach (var file in Directory.EnumerateFiles(pathtodirectory))
            {
                if (isValidFile(file))
                    validos.Add(Path.GetFileName(file));
                    
            }
            return validos;
        }

        public HashSet<string> getFilesNoLongerInRepo(HashSet<string> indexedfiles, HashSet<string> allfiles){
            var tobediscarded = new HashSet<string>();
            foreach(var file in indexedfiles){
                    if (!allfiles.Contains(file)){
                    tobediscarded.Add(file);
                }
            }
            return tobediscarded;
        }

        public HashSet<string> GetFilesNotInIndex(HashSet<string> indexedfiles, HashSet<string> allfiles){
            var tobeindexed = new HashSet<string>();
            foreach(var file in allfiles){
                if (!indexedfiles.Contains(file)){
                    tobeindexed.Add(file);
                }
            }
            return tobeindexed;
        }

        public HashSet<string> GetIndexedFilesThatHaveChanged(List<FileDocument> indexedfiles, string  directorypath){
            var changedfiles = new HashSet<string>();
            foreach(var file in indexedfiles){
                if (file.MD5Hash != FileDocument.CalculateMD5Hash(directorypath + file.Filename)){
                    Console.WriteLine("filename in gIF" + file.Filename);
                    changedfiles.Add(file.Filename);
                }
            }
            return changedfiles;
        }

        public void watch(){
            var meta = engine.GetMetaInfo();

            HashSet<string> validFiles = getValidFilesFromRepo(meta.repositoryPath);
            HashSet<string> FilesToBeIndexed = new HashSet<string>();
            HashSet<string> FilesToBeRemoved = new HashSet<string>();

            var indexedFileDocuments = engine.GetAllDocuments().ToList<FileDocument>();
            var indexedFileDocumentNames = indexedFileDocuments.Select(x => x.Filename).ToHashSet<string>();


            var repo_got_changed_or_new_initialisation = meta.repositoryPath != repoBeingWatched;
            var repo_length_changed = watchedRepoLength != validFiles.Count;

            // watcher attributes
            repoBeingWatched = meta.repositoryPath;
            watchedRepoLength = validFiles.Count;

            if (repo_got_changed_or_new_initialisation || repo_length_changed){
                FilesToBeIndexed = GetFilesNotInIndex(indexedFileDocumentNames, validFiles);
                FilesToBeRemoved = getFilesNoLongerInRepo(indexedFileDocumentNames, validFiles);
            }

            indexedFileDocumentNames.Except(FilesToBeRemoved);
            var updatedIndexedFileDocuments = indexedFileDocuments
                            .Where(x => indexedFileDocumentNames.Contains(x.Filename))
                            .ToList<FileDocument>();

            var changedfiles = GetIndexedFilesThatHaveChanged(updatedIndexedFileDocuments, meta.repositoryPath);
            FilesToBeIndexed.Union(changedfiles);
            FilesToBeRemoved.Union(changedfiles);

            foreach(var file in FilesToBeRemoved){
                engine.DeleteDocument(file);
            }

            foreach(var file in FilesToBeIndexed){
                Console.WriteLine(repoBeingWatched + "  " + file);
                var i = new Indexer(repoBeingWatched + file, meta.stopWords.ToHashSet<string>());
                var dex = i.IndexFile().index;

                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var id = engine.AddDocument(file);
                        engine.AddIntoReverseIndex(id, dex);
                        scope.Complete();
                    }
                }
                catch (TransactionAbortedException ex)
                {
                    Console.WriteLine("oops" + " " + ex);
                }

            }

        }
        // public RemoveFilesFromIndexThatArentInRepo
    }
}