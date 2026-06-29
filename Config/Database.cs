using Npgsql;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Booking.Config
{
    public class Database
    {
        private static readonly string _connectionString;

        static Database()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                var config = builder.Build();

                _connectionString = config.GetConnectionString("PostgresConnection");

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new Exception("Không tìm thấy chuỗi kết nối 'PostgresConnection' trong appsettings.json");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi Khởi Tạo Database]: {ex.Message}");
                throw;
            }
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}