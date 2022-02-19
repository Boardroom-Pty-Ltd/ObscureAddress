using System.Collections.Generic;

namespace ObscureAddress.Model
{
    public class ResponseModel
    {
        public List<string> PreExistingErrors { get; set; }
        public List<string> FinalErrors { get; set; }
        public InvestorModel Data { get; set; }

        public ResponseModel()
        { }
    }
}