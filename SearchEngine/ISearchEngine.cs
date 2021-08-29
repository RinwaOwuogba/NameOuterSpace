using System.Collections.Generic;

namespace SearchEngine
{
    public interface IEngine
    {
        List<WordDocument> GetWordDocuments(List<string> words);
        long GetAllDocumentsCount();
    }

    /// <summary>
    /// Forward index of a text
    /// </summary>
    public class ForwardIndex : Dictionary<string, long> { }
}