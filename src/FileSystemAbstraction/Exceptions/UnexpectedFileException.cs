using System;
using System.IO;
using System.Runtime.Serialization;

namespace FileSystemAbstraction.Exceptions
{
    [Serializable]
    public class UnexpectedFileException : IOException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UnexpectedFileException()
        {
        }

        public UnexpectedFileException(string message) : base(message)
        {
        }

        public UnexpectedFileException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnexpectedFileException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}