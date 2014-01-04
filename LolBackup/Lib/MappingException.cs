using System;

namespace LolBackup
{
    public class MappingException : Exception
    {
        readonly string _file;

        public string File
        {
            get
            {
                return _file;
            }
        }

        public MappingException(string file)
        {
            _file = file;
        }
    }
}