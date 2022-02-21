using System.Collections.Generic;

namespace ObscureAddress.Model
{
    public class Result<T>
    {
        //public bool Success;
        public List<string> PreExistingErrors;

        public List<string> FinalErrors;
        public T Data;
    }
}