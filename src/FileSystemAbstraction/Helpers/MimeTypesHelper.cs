using HeyRed.Mime;

namespace FileSystemAbstraction.Helpers
{
    public class MimeTypesHelper
    {
        public static string GetMimeType(string path)
            => MimeTypesMap.GetMimeType(path);

        public static string GetExtension(string mime)
            => MimeTypesMap.GetExtension(mime);
    }
}