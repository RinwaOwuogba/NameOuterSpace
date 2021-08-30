using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Hangfire;
using Hangfire.Server;
using Hangfire.LiteDB;


namespace SearchEngine
{

    /// <summary>
    /// Class Responsible for indexing Files in the Repo
    /// </summary>
    public class Watcher
    {
        /// <summary>
        ///     the string representing path to the repo that will be watched
        /// </summary>

        private string repoBeingWatched = "";

        /// <summary>
        ///     Keeps Track Of the Number of Valid Files in the repo
        /// </summary>
        private int watchedRepoLength = 0;

        /// <summary>
        ///     A reference to the Engine instance that interfaces with the db
        /// </summary>
        private Engine engine;

        /// <summary>
        ///     The Constructor for watch, gets a reference to engine
        /// </summary>
        /// <param name="eng"> the engine instance</param>
        public Watcher(Engine eng)
        {
            engine = eng;
            // GlobalConfiguration.Configuration.UseLiteDbStorage();
            // var client = new BackgroundJobServer();

        }

        /// <summary>
        ///     Static method that spins up a background thread and starts Indexing files
        /// </summary>
        /// <param name="watcher"></param>
        public static void IndexInBackGround(Watcher watcher)
        {
            Thread thread = new Thread(new ThreadStart(watcher.watch));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        ///     Checks if a file is valid, ie is among the supported extensions
        ///     and exists.
        /// </summary>
        /// <param name="path">the path of a file</param>
        /// <returns> true if a file is valid else false</returns>
        private bool isValidFile(string path)
        {
            var validTypes = new HashSet<string>(new string[] { "pdf", "doc", "docx", "ppt", "ppts", "xls", "xlsx", "txt", "html", "xml" });
            if (!validTypes.Contains(Path.GetExtension(path).Remove(0, 1))) return false;
            if (!File.Exists(path)) return false;
            return true;
        }

        /// <summary>
        ///     Looks through a repo and adds every valid file to a hashset
        /// </summary>
        /// <param name="pathtodirectory">the path to the repo</param>
        /// <returns>a hashset of valid files stored in a repo</returns>
        private HashSet<string> getValidFilesFromRepo(string pathtodirectory)
        {
            var validos = new HashSet<string>();
            foreach (var file in Directory.EnumerateFiles(pathtodirectory))
            {
                if (isValidFile(file))
                    validos.Add(Path.GetFileName(file));
            }
            return validos;
        }

        /// <summary>
        ///     Gets the Files That should Be deleted from the Index.
        ///     Because they do not exist in the repo any more.
        /// </summary>
        /// <param name="indexedfiles">Files that have been successfully indexed</param>
        /// <param name="allfiles">All valid Files in the repo, indexed or not</param>
        /// <returns> A Hashset of Files no longer in the repo </returns>
        private HashSet<string> getFilesNoLongerInRepo(HashSet<string> indexedfiles, HashSet<string> allfiles)
        {
            var tobediscarded = new HashSet<string>();
            foreach (var file in indexedfiles)
            {
                if (!allfiles.Contains(file))
                {
                    tobediscarded.Add(file);
                }
            }
            return tobediscarded;
        }

        /// <summary>
        ///     Gets files that have yet been indexed
        /// </summary>
        /// <param name="indexedfiles"></param>
        /// <param name="allfiles"></param>
        /// <returns></returns>
        private HashSet<string> GetFilesNotInIndex(HashSet<string> indexedfiles, HashSet<string> allfiles)
        {
            var tobeindexed = new HashSet<string>();
            foreach (var file in allfiles)
            {
                if (!indexedfiles.Contains(file))
                {
                    tobeindexed.Add(file);
                }
            }
            return tobeindexed;
        }

        /// <summary>
        ///     Get indexed files whose contents have changed
        /// </summary>
        /// <param name="indexedfiles"></param>
        /// <param name="directorypath"></param>
        /// <returns></returns>
        private HashSet<string> GetIndexedFilesThatHaveChanged(List<FileDocument> indexedfiles, string directorypath)
        {
            var changedfiles = new HashSet<string>();
            foreach (var file in indexedfiles)
            {
                if (file.MD5Hash != FileDocument.CalculateMD5Hash(directorypath + file.Filename))
                {
                    Console.WriteLine("filename in gIF" + file.Filename);
                    changedfiles.Add(file.Filename);
                }
            }
            return changedfiles;
        }

        /// <summary>
        ///     Monitor The Repo, and index files and delete from the index as the need arises
        /// </summary>
        public void watch()
        {

            var meta = engine.GetMetaInfo();
            while (true)
            {
                Console.WriteLine("in here");

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

                if (repo_got_changed_or_new_initialisation || repo_length_changed)
                {
                    FilesToBeIndexed = GetFilesNotInIndex(indexedFileDocumentNames, validFiles);
                    FilesToBeRemoved = getFilesNoLongerInRepo(indexedFileDocumentNames, validFiles);
                }

                indexedFileDocumentNames = indexedFileDocumentNames.Except(FilesToBeRemoved)
                                            .ToHashSet<string>();
                var updatedIndexedFileDocuments = indexedFileDocuments
                                .Where(x => indexedFileDocumentNames.Contains(x.Filename))
                                .ToList<FileDocument>();

                var changedfiles = GetIndexedFilesThatHaveChanged(updatedIndexedFileDocuments, meta.repositoryPath);

                FilesToBeIndexed = FilesToBeIndexed.Union(changedfiles).ToHashSet<string>(); ;
                FilesToBeRemoved = FilesToBeRemoved.Union(changedfiles).ToHashSet<string>(); ;


                foreach (var file in FilesToBeRemoved)
                {
                    Console.WriteLine("oohoo");
                    engine.DeleteDocument(file);
                }
                Console.WriteLine("i dey");

                foreach (var file in FilesToBeIndexed)
                {
                    Console.WriteLine(repoBeingWatched + "  " + file);
                    var i = new Indexer(meta.stopWords.ToHashSet<string>());
                    var dex = i.IndexFile(repoBeingWatched + file);

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
                Thread.Sleep(30000);

            }

        }
    }
}