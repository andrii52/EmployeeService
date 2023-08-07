using System.Data.SqlClient;

namespace EmployeeService
{
	public static class SqlExtension
	{
		public static string SafeGetString(this SqlDataReader reader, int colIndex)
		{
			if (!reader.IsDBNull(colIndex))
				return reader.GetString(colIndex);
			return string.Empty;
		}

		public static int SafeGetInt(this SqlDataReader reader, int colIndex)
		{
			if (!reader.IsDBNull(colIndex))
				return reader.GetInt32(colIndex);
			return default(int);
		}

		public static bool SafeGetBool(this SqlDataReader reader, int colIndex)
		{
			if (!reader.IsDBNull(colIndex))
				return reader.GetBoolean(colIndex);
			return default(bool);
		}
	}
}