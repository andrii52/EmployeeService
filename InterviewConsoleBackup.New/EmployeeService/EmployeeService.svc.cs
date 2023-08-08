using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EmployeeService.svc or EmployeeService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class EmployeeService : IEmployeeService
    {
        public async Task<EmployeeEntity> GetEmployeeById(int id)
        {
            var emloyee = new EmployeeEntity();

            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=Test;User ID=sa;Password=Rootpassword1; "))
            {
                connection.Open();
                var getEmployeeComamnd = new SqlCommand($"SELECT * FROM Employee WHERE ID = {id} ", connection);
                using (var reader = await getEmployeeComamnd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {

                        emloyee.ID = reader.SafeGetInt(reader.GetOrdinal(nameof(EmployeeEntity.ID)));
                        emloyee.Name = reader.SafeGetString(reader.GetOrdinal(nameof(EmployeeEntity.Name)));
                        emloyee.ManagerID = reader.SafeGetInt(reader.GetOrdinal(nameof(EmployeeEntity.ManagerID)));
                        emloyee.Enable = reader.SafeGetBool(reader.GetOrdinal(nameof(EmployeeEntity.Enable)));

                    }
                }
                var getAllEmloyeesCommand = new SqlCommand($"SELECT * FROM Employee WHERE ManagerID = {id} AND ID <> {id}", connection);

				using (var reader = await getAllEmloyeesCommand.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        var employee = new EmployeeEntity()
                        {
                            ID = reader.SafeGetInt(reader.GetOrdinal(nameof(EmployeeEntity.ID))),
                            Name = reader.SafeGetString(reader.GetOrdinal(nameof(EmployeeEntity.Name))),
                            ManagerID = reader.SafeGetInt(reader.GetOrdinal(nameof(EmployeeEntity.ManagerID))),
                            Enable = reader.SafeGetBool(reader.GetOrdinal(nameof(EmployeeEntity.Enable))),
                        };
						emloyee.Employees.Add(employee);
                    }
                }

			}
            foreach (var employeer in emloyee.Employees)
            {
              await GetEmployeeByManagerID(employeer);
            }

			return emloyee;
        }



        public async Task EnableEmployee(int id, Entity entity)
        {
            var enable = entity.enable;
            if(!bool.Parse(enable.ToString()))
            {
                throw new ArgumentException($"{enable} value is not applicable for Enable field");
            }
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=Test;User ID=sa;Password=Rootpassword1; "))
            {
                connection.Open();
				var updateEnableCommand = new SqlCommand($"Update Employee SET Enable = { enable} WHERE ID = {id} ", connection);
                await updateEnableCommand.ExecuteNonQueryAsync();
			}
        }

        private async Task GetEmployeeByManagerID(EmployeeEntity manager)
        {
            using (var connection = new SqlConnection("Data Source=(local);Initial Catalog=Test;User ID=sa;Password=Rootpassword1; "))
            {
				connection.Open();
				var getAllEmloyeesCommand = new SqlCommand($"SELECT * FROM Employee WHERE ManagerID = {manager.ID} AND ID <> {manager.ID}", connection);

				using (var reader = await getAllEmloyeesCommand.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						var employee = new EmployeeEntity()
						{
							ID = reader.SafeGetInt(reader.GetOrdinal(nameof(EmployeeEntity.ID))),
							Name = reader.SafeGetString(reader.GetOrdinal(nameof(EmployeeEntity.Name))),
							ManagerID = reader.SafeGetInt(reader.GetOrdinal(nameof(EmployeeEntity.ManagerID))),
							Enable = reader.SafeGetBool(reader.GetOrdinal(nameof(EmployeeEntity.Enable))),
						};
						manager.Employees.Add(employee);
					}
				}
			}
            foreach (var emloyee in manager.Employees)
            {
               await GetEmployeeByManagerID(emloyee);
            }
        }
	}

      
}