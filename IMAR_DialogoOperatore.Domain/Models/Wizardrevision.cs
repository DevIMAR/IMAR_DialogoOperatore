namespace ImarConnect.DTO.Entities
{
	public class Wizardrevision
	{
		public bool flgEnb { get; set; }
		public int revRefUid { get; set; }
		public long tsi { get; set; }
		public long tsu { get; set; }
		public int uid { get; set; }
		public string wzdExeCls { get; set; }
		public string wzdMdlCls { get; set; }
		public int wzdRev { get; set; }
		public string wzdRevVer { get; set; }
		public int wzdUid { get; set; }
		public int recVer { get; set; }
		public int lngRecVer { get; set; }
		public bool linkRecVerToConnection { get; set; }
	}
}
