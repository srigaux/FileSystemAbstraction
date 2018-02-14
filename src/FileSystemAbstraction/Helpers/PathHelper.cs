using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileSystemAbstraction.Helpers
{
    public class PathHelper
    {
        /// <summary>
        /// Normalizes the given path
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>The normalized path</returns>
        public static string Normalize(string path)
        {
            path = path.Replace('\\', '/');
            var prefix = AbsolutePrefix(path);
            path = path.Substring(prefix.Length);
            var parts = path.Split('/').Where(part => !string.IsNullOrEmpty(part));
            var tokens = new Stack<string>();

            foreach (var part in parts)
                switch (part)
                {
                    case ".":
                        break;
                    
                    case "..":
                        if (tokens.Any())
                            tokens.Pop();
                        break;

                    default:
                        tokens.Push(part);
                        break;
                }

            return prefix + string.Join("/", tokens.Reverse());
        }

        /// <summary>
        /// Indicates whether the given path is absolute or not.
        /// </summary>
        /// <param name="path">A normalized path</param>
        /// <returns></returns>
        public static bool IsAbsolute(string path) 
            => !string.IsNullOrEmpty(AbsolutePrefix(path));


        private static readonly Regex AbsoultePrefixRegex = new Regex("^(?<prefix>([a-zA-Z]+:)?\\/\\/?)", RegexOptions.ExplicitCapture);
        /// <summary>
        /// Returns the absolute prefix of the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string AbsolutePrefix(string path)
        {
            var match = AbsoultePrefixRegex.Match(path);
            var prefixGroup = match.Groups["prefix"];

            if (!prefixGroup.Success)
                return "";

            return prefixGroup.Value.ToLowerInvariant();
        }
    }
}