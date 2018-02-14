namespace FileSystemAbstraction.Schemes
{
    /// <summary>
    /// Contains the options used by the <see cref="IFileSystem" />.
    /// </summary>
    public class FileSystemSchemeOptions
    {
        /// <summary>
        /// Check that the options are valid. Should throw an exception if things are not ok.
        /// </summary>
        public virtual void Validate()
        {
        }
    }
}