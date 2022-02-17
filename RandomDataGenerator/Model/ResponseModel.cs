using System.Collections.Generic;

namespace RandomDataGenerator.Model
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