namespace BusBookingSystem.API.Helpers
{
	public static class RoleConstants
	{
		public const string SuperAdmin = "SuperAdmin";
		public const string Admin = "Admin";
		public const string BusOperator = "BusOperator";
		public const string Customer = "Customer";

		public static readonly string[] AllRoles =
		{
			SuperAdmin,
			Admin,
			BusOperator,
			Customer
		};
	}
}