using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace EmployeeManagementSystem.DAL
{
    /// <summary>
    /// Central database helper class for all database operations
    /// Implements connection management and query execution
    /// </summary>
    public class DatabaseHelper : IDisposable
    {
        private SqlConnection connection;
        private readonly string connectionString;
        private bool disposed = false;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["EmployeeDB"].ConnectionString;
        }

        /// <summary>
        /// Gets an open database connection
        /// </summary>
        private SqlConnection GetConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(connectionString);
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        /// <summary>
        /// Executes a stored procedure and returns a DataTable
        /// </summary>
        public DataTable ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, GetConnection()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error executing {procedureName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE)
        /// Returns number of rows affected
        /// </summary>
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = 30;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a scalar query and returns single value
        /// </summary>
        public object ExecuteScalar(string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = 30;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a query and returns DataTable
        /// </summary>
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, GetConnection()))
                {
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = 30;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a transaction with multiple commands
        /// </summary>
        public bool ExecuteTransaction(params SqlCommand[] commands)
        {
            SqlTransaction transaction = null;
            try
            {
                SqlConnection conn = GetConnection();
                transaction = conn.BeginTransaction();

                foreach (var cmd in commands)
                {
                    cmd.Connection = conn;
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction?.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Tests database connectivity
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection testConn = new SqlConnection(connectionString))
                {
                    testConn.Open();
                    return testConn.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}