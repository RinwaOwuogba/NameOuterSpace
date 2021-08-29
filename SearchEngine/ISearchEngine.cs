using System.Collections.Generic;

namespace SearchEngine
{
    public interface IEngine
    {
        List<WordDocument> GetWordDocuments(List<string> words);
        long GetAllDocumentsCount();
    }


    public interface IParsedQuery
    {
        string NaturalLangQuery { get; }
        Dictionary<string, long> QueryIndex { get; }
        long GetMaxQueryFreq();
    }

    /// <summary>
    /// Forward index of a text
    /// </summary>
    public class ForwardIndex : Dictionary<string, long> { }
}