using System.Collections.Generic;

namespace NameOuterSpace
{
    public interface IEngine
    {
        List<WordDocument> GetWordDocuments(HashSet<string> words);
        long GetAllDocumentsCount();
    }

    public interface IParsedQuery
    {
        string NaturalLangQuery { get; }
        Dictionary<string, long> QueryIndex { get; }
        long GetMaxQueryTermFreq();
    }

    public interface IIndexer
    {
        Dictionary<string, long> IndexFile(string filePath);
        Dictionary<string, long> IndexText(string text);
    }
}