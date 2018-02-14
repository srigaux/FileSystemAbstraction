using System.Collections.Generic;

namespace FileSystemAbstraction
{
    public class ListKeysResult
    {
        public List<string> Directories { get; set; } = new List<string>();
        public List<string> Keys { get; set; } = new List<string>();
    }
}