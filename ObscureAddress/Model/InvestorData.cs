namespace ObscureAddress.Model
{
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

		public string Type;

		public InvestorData()
		{
			Id = Name = Line1 = Line2 = Line3 = Line4 = Line5 = Line6 = Postcode = Domicile = Type = string.Empty;
		}

		public override bool Equals(object obj)
		{
			var compare = obj as InvestorData;

			if (compare == null)
				return false;

			if (Id != compare.Id || Name != compare.Name ||
				Line1 != compare.Line1 || Line2 != compare.Line2 || Line3 != compare.Line3 || Line4 != compare.Line4 || Line5 != compare.Line5 || Line6 != compare.Line6 ||
				Postcode != compare.Postcode || Domicile != compare.Domicile || Type != compare.Domicile)
				return false;

			return true;
		}
	}
}