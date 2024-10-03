using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

public class DataAccess
{
    private string mssqlConnectionString;
    private string sqliteConnectionString;

    public DataAccess(string mssqlConnectionString, string sqliteConnectionString)
    {
        this.mssqlConnectionString = mssqlConnectionString;
        this.sqliteConnectionString = sqliteConnectionString;
    }

    public DataTable GetCustomersFromMSSQL()
    {
        using (SqlConnection connection = new SqlConnection(mssqlConnectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Customer", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }
    }

    public DataTable GetLocationsFromMSSQL()
    {
        using (SqlConnection connection = new SqlConnection(mssqlConnectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Location", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }
    }

    public void SyncDataToSQLite(DataTable customers, DataTable locations)
    {
        using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
        {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Customer", connection);
            command.ExecuteNonQuery();
            command = new SQLiteCommand("DELETE FROM Location", connection);
            command.ExecuteNonQuery();

            foreach (DataRow row in customers.Rows)
            {
                command = new SQLiteCommand("INSERT INTO Customer (CustomerID, Name, Email, Phone) VALUES (@CustomerID, @Name, @Email, @Phone)", connection);
                command.Parameters.AddWithValue("@CustomerID", row["CustomerID"]);
                command.Parameters.AddWithValue("@Name", row["Name"]);
                command.Parameters.AddWithValue("@Email", row["Email"]);
                command.Parameters.AddWithValue("@Phone", row["Phone"]);
                command.ExecuteNonQuery();
            }

            foreach (DataRow row in locations.Rows)
            {
                command = new SQLiteCommand("INSERT INTO Location (LocationID, CustomerID, Address) VALUES (@LocationID, @CustomerID, @Address)", connection);
                command.Parameters.AddWithValue("@LocationID", row["LocationID"]);
                command.Parameters.AddWithValue("@CustomerID", row["CustomerID"]);
                command.Parameters.AddWithValue("@Address", row["Address"]);
                command.ExecuteNonQuery();
            }
        }
    }

    public void LogSyncOperation(string tableName, int recordID, string fieldName, string oldValue, string newValue)
    {
        using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
        {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand("INSERT INTO SyncLog (Timestamp, TableName, RecordID, FieldName, OldValue, NewValue) VALUES (@Timestamp, @TableName, @RecordID, @FieldName, @OldValue, @NewValue)", connection);
            command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString());
            command.Parameters.AddWithValue("@TableName", tableName);
            command.Parameters.AddWithValue("@RecordID", recordID);
            command.Parameters.AddWithValue("@FieldName", fieldName);
            command.Parameters.AddWithValue("@OldValue", oldValue);
            command.Parameters.AddWithValue("@NewValue", newValue);
            command.ExecuteNonQuery();
        }
    }
}