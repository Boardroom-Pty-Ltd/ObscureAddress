namespace ObscureAddress.Model
{
	public class CrsData
	{
		// primary key, use to seed random generator
		public int Id;

		// ignore it
		public char EntityType;

		// luckily, these names are all in separate fields which makes your task easier
		// substitute them, preserving gender, from dictionary, length can change

		public string FirstName;        // max length 50
		public string MiddleName;       // max length 50
		public string LastName;         // max length 50

		// in case this is company and not individual
		// replace with random word (maybe find human-sounding word generator?)

		public string RegisteredName;   // max length 300
		public string CorporationName;  // max length 300

		// use same logic as for your previous code.
		// do not replace some keywords
		// do not replace country names

		public string Address1;         // max length 30
		public string Address2;         // max length 30
		public string Address3;         // max length 30
		public string Address4;         // max length 30
		public string Address5;         // max length 30

		// replace each letter with other random letter
		// replace each number with other random number
		// length should not change

		public string GiinNumber;
		public string AbnNumber;

		// don't change, you may (or may not) want to use it in logic
		public string AddressDomicile;

		public CrsData()
		{
			FirstName = MiddleName = LastName = RegisteredName = CorporationName = Address1 = Address2 = Address3 = Address4 = Address5 = GiinNumber = AbnNumber = AddressDomicile = string.Empty;
		}

		public override bool Equals(object obj)
		{
			var compare = obj as CrsData;

			if (compare == null)
				return false;

			if (Id != compare.Id || EntityType != compare.EntityType || FirstName != compare.FirstName || MiddleName != compare.MiddleName || LastName != compare.LastName ||
				RegisteredName != compare.RegisteredName || CorporationName != compare.CorporationName ||
				Address1 != compare.Address1 || Address2 != compare.Address2 || Address3 != compare.Address3 || Address4 != compare.Address4 || Address5 != compare.Address5 ||
				GiinNumber != compare.GiinNumber || AbnNumber != compare.AbnNumber || AddressDomicile != compare.AddressDomicile)
				return false;

			return true;
		}
	}
}