using System;
using System.Collections.Generic;

namespace EmployeeService
{
	public class EmployeeEntity
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public int ManagerID { get; set; }

		public bool Enable { get; set; }
		public ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
	}
}