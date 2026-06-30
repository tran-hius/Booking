namespace Booking.Mappings;
using Npgsql;

    public class UserMapper
    {
       public static User ToEntity(NpgsqlDataReader reader)
        {
        if (reader == null) return null;
        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            FullName = reader.GetString(reader.GetOrdinal("full_name")),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? string.Empty : reader.GetString(reader.GetOrdinal("phone_number"))
        };
        }
    }

