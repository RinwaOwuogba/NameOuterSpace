using System.Collections.Generic;

namespace SearchEngine
{
    /// <summary>
    /// Representation of a natural language query
    /// </summary>
    public class ParserQuery : Dictionary<string, long> { }

    /// <summary>
    /// Forward index of a text
    /// </summary>
    public class ForwardIndex : Dictionary<string, long> { }
}