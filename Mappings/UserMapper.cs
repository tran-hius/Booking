namespace Booking.Mappings;

using Booking.DTOs.User.Response;
using Booking.Entities.User;
using Booking.Entities.User.User;
using Npgsql;

public static class UserMapper
{
    public static User ToEntity(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            FullName = reader.GetString(reader.GetOrdinal("full_name")),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? string.Empty : reader.GetString(reader.GetOrdinal("phone_number")),
            Role = (Role)reader.GetInt32(reader.GetOrdinal("role")),
            Status = (UserStatus)reader.GetInt32(reader.GetOrdinal("status")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
            DeletedAt = reader.IsDBNull(reader.GetOrdinal("deleted_at")) ? null : reader.GetDateTime(reader.GetOrdinal("deleted_at"))
        };
    }

    public static UserResponseDTO ToResponseDTO(User user)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = (int)user.Role,
            Status = user.Status,
            DeletedAt = user.DeletedAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            
        };
    }
}

