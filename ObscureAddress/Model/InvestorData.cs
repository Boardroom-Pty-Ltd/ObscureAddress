public class InvestorData
{
    // unique Id, primary key, use it to seed random generator
    public string Id;

    public string Name;

    // must be 30 char long, padded with spaces where needed
    public string Line1;

    public string Line2;
    public string Line3;
    public string Line4;
    public string Line5;
    public string Line6;

    // for AUS, 4-char numeric space-padded
    public string Postcode;

    // always 3 chars, don't replace, special processing may be required for AUS
    public string Domicile;

    public override bool Equals(object obj)
    {
        var other = obj as InvestorData;

        if (other == null)
            return false;

        if (Id != other.Id || Name != other.Name || Line1 != other.Line1 || Line2 != other.Line2 ||
            Line3 != other.Line3 || Line4 != other.Line4 || Line5 != other.Line5 || Line6 != other.Line6 ||
            Postcode != other.Postcode || Domicile != other.Domicile)
            return false;

        return true;
    }
}