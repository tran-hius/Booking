namespace Booking.Mappings;

using Booking.DTOs.User.Response;
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
            Status = (UserStatus)reader.GetInt32(reader.GetOrdinal("status")),
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
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

