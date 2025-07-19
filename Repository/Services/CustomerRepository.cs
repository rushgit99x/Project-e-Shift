using e_Shift.Config;
using e_Shift.Models;
using e_Shift.Repository.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace e_Shift.Repository.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        public async Task<Customer> RegisterCustomerAsync(Customer customer)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DBConnection.ConnectionString))
                {
                    await conn.OpenAsync();

                    // Insert customer data
                    string insertQuery = "INSERT INTO Customers (CustomerNumber, FirstName, LastName, Email, Phone, Address, Password) " +
                                        "VALUES (@CustomerNumber, @FirstName, @LastName, @Email, @Phone, @Address, @Password);";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@CustomerNumber", customer.CustomerNumber);
                        insertCmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                        insertCmd.Parameters.AddWithValue("@LastName", customer.LastName);
                        insertCmd.Parameters.AddWithValue("@Email", customer.Email);
                        insertCmd.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@Address", customer.Address ?? (object)DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@Password", customer.Password);

                        await insertCmd.ExecuteNonQueryAsync();
                    }

                    // Retrieve the last inserted ID
                    string selectQuery = "SELECT LAST_INSERT_ID(), @CustomerNumber;";
                    using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@CustomerNumber", customer.CustomerNumber);
                        using (MySqlDataReader reader = (MySqlDataReader)await selectCmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                customer.CustomerID = reader.GetInt32(0); // LAST_INSERT_ID()
                                customer.CustomerNumber = reader.GetString(1); // @CustomerNumber
                            }
                            else
                            {
                                throw new Exception("Failed to retrieve the inserted customer ID.");
                            }
                        }
                    }

                    return customer;
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                // Check if the error is specifically for the email field
                if (ex.Message.Contains("Email"))
                {
                    throw new Exception("Email already registered. Please use a different email.", ex);
                }
                else if (ex.Message.Contains("CustomerNumber"))
                {
                    throw new Exception("Customer number already exists. Please try again.", ex);
                }
                else
                {
                    throw new Exception("A duplicate entry error occurred.", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while registering the customer: " + ex.Message, ex);
            }
        }

        public async Task<Customer> GetCustomerByCredentialsAsync(string email, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DBConnection.ConnectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT CustomerID, CustomerNumber, FirstName, LastName, Email, Phone, Address " +
                                   "FROM Customers WHERE Email = @Email AND Password = @Password";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Password", password ?? (object)DBNull.Value);

                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Customer
                                {
                                    CustomerID = reader.GetInt32("CustomerID"),
                                    CustomerNumber = reader.GetString("CustomerNumber"),
                                    FirstName = reader.GetString("FirstName"),
                                    LastName = reader.GetString("LastName"),
                                    Email = reader.GetString("Email"),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address")
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Database error occurred while retrieving customer credentials.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving customer credentials.", ex);
            }
        }
    }
}