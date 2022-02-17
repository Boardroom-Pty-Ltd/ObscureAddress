namespace RandomDataGenerator.Model
{
    public class InvestorModel
    {
        //SMASTNUMB
        public string Id { get; set; }

        //SMASTSORT
        public string Name { get; set; }

        //SMASTNAME1
        public string Line1 { get; set; }

        //SMASTNAME2
        public string Line2 { get; set; }

        //SMASTNAME3
        public string Line3 { get; set; }

        //SMASTNAME4
        public string Line4 { get; set; }

        //SMASTNAME5
        public string Line5 { get; set; }

        //SMASTNAME6
        public string Line6 { get; set; }

        //SMASTDOM
        public string Domicile { get; set; }

        //SMASTPCODE
        public string Postcode { get; set; }

        //SMASTTYPE
        public string Type { get; set; }

        public InvestorModel()
        {
            Id = Name = Line1 = Line2 = Line3 = Line4 = Line5 = Line6 = Domicile = Postcode = Type = string.Empty;
        }
    }
}