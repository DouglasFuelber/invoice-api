using invoice_api.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace invoice_api.Repositories
{
    public class InvoiceRepository
    {
        private static SqliteConnection _sqliteConnection;

        private static SqliteConnection DbConnection()
        {
            _sqliteConnection = new SqliteConnection("Data Source=database.db");
            _sqliteConnection.Open();

            CreateTables(_sqliteConnection);

            return _sqliteConnection;
        }

        private static void CreateTables(SqliteConnection _sqliteConnection)
        {
            try
            {
                using (var command = _sqliteConnection.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS Invoice (
                                            Id integer PRIMARY KEY AUTOINCREMENT,
                                            ReferenceMonth integer NOT NULL,
                                            ReferenceYear integer NOT NULL,
                                            Document varchar(14) NOT NULL,
                                            Description varchar(256) NOT NULL,
                                            Amount decimal(16, 2) NOT NULL,
                                            IsActive bool NOT NULL,
                                            CreatedAt datetime NOT NULL,
                                            DeactivatedAt datetime
                                        )";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Invoice> Get()
        {
            var invoices = new List<Invoice>();

            using (var connection = DbConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM invoice WHERE IsActive = 1";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        invoices.Add(MapInvoice(reader));
                    }
                }
            }

            return invoices;
        }

        public static Invoice Get(int id)
        {
            var invoice = new Invoice();

            using (var connection = DbConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM invoice WHERE Id = $id AND IsActive = 1";
                command.Parameters.AddWithValue("$id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;

                    while (reader.Read())
                    {
                        invoice = MapInvoice(reader);
                    }
                }
            }

            return invoice;
        }

        public static Invoice Post(Invoice invoice)
        {
            try
            {
                invoice.IsActive = true;
                invoice.CreatedAt = DateTime.Now;

                using (var connection = DbConnection())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"INSERT INTO Invoice
                                            (ReferenceMonth, ReferenceYear, Document, Description, Amount, IsActive, CreatedAt)
                                            values ($ReferenceMonth, $ReferenceYear, $Document, $Description, $Amount, $IsActive, $CreatedAt)";

                    command.Parameters.AddWithValue("$ReferenceMonth", invoice.ReferenceMonth);
                    command.Parameters.AddWithValue("$ReferenceYear", invoice.ReferenceYear);
                    command.Parameters.AddWithValue("$Document", invoice.Document);
                    command.Parameters.AddWithValue("$Description", invoice.Description);
                    command.Parameters.AddWithValue("$Amount", invoice.Amount);
                    command.Parameters.AddWithValue("$IsActive", invoice.IsActive);
                    command.Parameters.AddWithValue("$CreatedAt", invoice.CreatedAt);

                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT LAST_INSERT_ROWID();";
                    invoice.Id = Convert.ToInt32(command.ExecuteScalar());
                }

                return invoice;
            }
            catch
            {
                return null;
            }            
        }

        public static Invoice Update (Invoice invoice)
        {
            try
            {
                using (var connection = DbConnection())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"UPDATE Invoice SET
                                                ReferenceMonth = $ReferenceMonth,
                                                ReferenceYear = $ReferenceYear,
                                                Document = $Document,
                                                Description = $Description,
                                                Amount = $Amount
                                            WHERE Id = $Id;";

                    command.Parameters.AddWithValue("$ReferenceMonth", invoice.ReferenceMonth);
                    command.Parameters.AddWithValue("$ReferenceYear", invoice.ReferenceYear);
                    command.Parameters.AddWithValue("$Document", invoice.Document);
                    command.Parameters.AddWithValue("$Description", invoice.Description);
                    command.Parameters.AddWithValue("$Amount", invoice.Amount);
                    command.Parameters.AddWithValue("$Id", invoice.Id);

                    return (command.ExecuteNonQuery() == 1) ? invoice : null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static Invoice Delete(int id)
        {
            try
            {
                var invoice = Get(id);

                if (invoice == null)
                    return null;

                invoice.IsActive = false;
                invoice.DeactivatedAt = DateTime.Now;

                using (var connection = DbConnection())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"UPDATE Invoice SET
                                                IsActive = $IsActive,
                                                DeactivatedAt = $DeactivatedAt
                                            WHERE Id = $Id;";

                    command.Parameters.AddWithValue("$IsActive", invoice.IsActive);
                    command.Parameters.AddWithValue("$DeactivatedAt", invoice.DeactivatedAt);
                    command.Parameters.AddWithValue("$Id", invoice.Id);

                    return (command.ExecuteNonQuery() == 1) ? invoice : null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static Invoice MapInvoice(SqliteDataReader reader)
        {
            var invoice = new Invoice();

            invoice.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            invoice.ReferenceMonth = reader.GetInt32(reader.GetOrdinal("ReferenceMonth"));
            invoice.ReferenceYear = reader.GetInt32(reader.GetOrdinal("ReferenceYear"));
            invoice.Document = reader.GetString(reader.GetOrdinal("Document"));
            invoice.Description = reader.GetString(reader.GetOrdinal("Description"));
            invoice.Amount = reader.GetDecimal(reader.GetOrdinal("Amount"));
            invoice.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
            invoice.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
            if (!reader.IsDBNull(reader.GetOrdinal("DeactivatedAt")))
                invoice.DeactivatedAt = reader.GetDateTime(reader.GetOrdinal("DeactivatedAt"));

            return invoice;
        }
    }
}