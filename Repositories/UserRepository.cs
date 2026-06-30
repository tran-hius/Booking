using Booking.Config;
using Booking.Entities.User.User;
using Booking.Interfaces.Repositories;
using Booking.Mappings;
using Npgsql;
using System.Data;

namespace Booking.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Database _database;

        public UserRepository(Database database)
        {
            _database = database;
        }

        private async Task EnsureOpenConnectionAsync(NpgsqlConnection conn)
        {
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
        }

        public async Task<User> Create(User user)
        {
            await using (var conn = _database.GetConnection())
            {
                await EnsureOpenConnectionAsync(conn);

                string sql = @"INSERT INTO users (email, password_hash, role, full_name, phone_number, status, created_at, updated_at)
                               VALUES (@email, @password_hash, @role, @full_name, @phone_number, @status, @created_at, @updated_at)
                               RETURNING *";

                await using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("email", user.Email);
                    cmd.Parameters.AddWithValue("password_hash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("role", (int)user.Role);
                    cmd.Parameters.AddWithValue("full_name", user.FullName);
                    cmd.Parameters.AddWithValue("phone_number", user.PhoneNumber);
                    cmd.Parameters.AddWithValue("status", (int)user.Status);
                    cmd.Parameters.AddWithValue("created_at", user.CreatedAt);
                    cmd.Parameters.AddWithValue("updated_at", user.UpdatedAt);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return UserMapper.ToEntity(reader);
                        }
                    }
                }
            }

            throw new Exception("Admin tạo user thất bại, không nhận được phản hồi từ Database.");
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            await using (var conn = _database.GetConnection())
            {
                await EnsureOpenConnectionAsync(conn);

                string sql = "SELECT EXISTS(SELECT 1 FROM users WHERE email = @email)";

                await using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("email", email);

                    var result = await cmd.ExecuteScalarAsync();

                    return result is bool b && b;
                }
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var users = new List<User>();

            await using (var conn = _database.GetConnection())
            {
                await EnsureOpenConnectionAsync(conn);

                string sql = "SELECT * FROM users WHERE deleted_at IS NULL ORDER BY id DESC";

                await using (var cmd = new NpgsqlCommand(sql, conn))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(UserMapper.ToEntity(reader));
                    }
                }
            }

            return users;
        }

        public async Task<User?> GetByEmail(string email)
        {
            await using (var conn = _database.GetConnection())
            {
                await EnsureOpenConnectionAsync(conn);

                string sql = "SELECT * FROM users WHERE email = @email AND deleted_at IS NULL LIMIT 1";

                await using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("email", email);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return UserMapper.ToEntity(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<User?> GetById(int id)
        {
            await using var conn = _database.GetConnection();

            await EnsureOpenConnectionAsync(conn);

            const string sql = """
                SELECT *
                FROM users
                WHERE id = @id
                  AND deleted_at IS NULL
                LIMIT 1;
                """;

            await using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("id", id);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return UserMapper.ToEntity(reader);
            }

            return null;
        }

        public async Task Restore(int id)
        {
            await using var conn = _database.GetConnection();

            await EnsureOpenConnectionAsync(conn);

            const string sql = """
                UPDATE users
                SET
                    deleted_at = NULL,
                    status = @status,
                    updated_at = @updated_at
                WHERE id = @id
                  AND deleted_at IS NOT NULL;
                """;

            await using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("status", (int)UserStatus.Active);
            cmd.Parameters.AddWithValue("updated_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task SoftDelete(int id)
        {
            await using var conn = _database.GetConnection();

            await EnsureOpenConnectionAsync(conn);

            const string sql = """
                UPDATE users
                SET
                    deleted_at = @deleted_at,
                    status = @status,
                    updated_at = @updated_at
                WHERE id = @id
                  AND deleted_at IS NULL;
                """;

            await using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("deleted_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("status", (int)UserStatus.Inactive);
            cmd.Parameters.AddWithValue("updated_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}