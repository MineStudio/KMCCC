using System;
using System.Collections.Generic;
using System.Text;

namespace KMCCC.Tools
{
    public static class FileValidator
    {
        
    }

    public interface IFileValidator
    {
        bool Validate(string file,string Hash);
    }

    public enum HashType
    {
        SHA1, MD5
    }

    public sealed class FileHashValidator : IFileValidator
    {
        public HashType hashType { get; set; }

        public string FilePath { get; set; }

        public bool Validate(string file,string hash)
        {
            return false;
        }
    }
}
