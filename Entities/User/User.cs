using Booking.Entities.User;
using Booking.Entities.User.User;
namespace Booking.Entities.User.User;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Role Role { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public UserStatus Status { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User()
    {
    }

    public User(
        int id,
        string email,
        string passwordHash,
        Role role,
        string fullName,
        string phoneNumber,
        UserStatus status,
        DateTime? deletedAt,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Status = status;
        DeletedAt = deletedAt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void UpdateProfile(string fullName, string phoneNumber)
    {
        EnsureUserIsActive();
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Touch();
    }

    public void ChangePassword(string passwordHash)
    {
        EnsureUserIsActive();
        PasswordHash = passwordHash;
        Touch();
    }

    public void VerifyEmail()
    {
        EnsureNotDeleted();

        if (Status == UserStatus.Active)
        {
            throw new Exception("Tài khoản này đã được xác thực rồi.");
        }

        if (Status == UserStatus.Banned)
        {
            throw new InvalidOperationException("Tài khoản đã bị khóa, không thể xác thực.");
        }

        Status = UserStatus.Active;
        Touch();
    }

    public void Ban()
    {
        EnsureNotDeleted();
        if (Status == UserStatus.Banned)
        {
            return;
        }
        Status = UserStatus.Banned;
        Touch();
    }

    public void UnBan()
    {
        EnsureNotDeleted();
        if (Status == UserStatus.Banned)
        {
            Status = UserStatus.Active;
            Touch();
        }
    }

    public void AssignRole(Role role) 
    {
        EnsureUserIsActive();
        Role = role;
        Touch();
    }

    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }

    public bool IsBanned()
    {
        return Status == UserStatus.Banned;
    }

    public bool IsCustomer()
    {
        return Role == Role.Customer;
    }

    public bool IsAdmin()
    {
        return Role == Role.Admin;
    }

    public bool IsGuest()
    {
        return Role == Role.Guest;
    }

    //Đảm bảo tài khoản đang hoạt động
    private void EnsureUserIsActive()
    {
        EnsureNotDeleted();
        if (Status == UserStatus.Pending || Status == UserStatus.Inactive || Status == UserStatus.Banned)
        {
            throw new Exception("Không thể thực hiện thao tác này trên tài khoản không hoạt động.");
        }
    }

    //Đảm bảo tài khoản không bị xóa
    private void EnsureNotDeleted()
    {
        if (DeletedAt.HasValue)
        {
            throw new Exception("Không thể thực hiện thao tác trên tài khoản đã bị xóa.");
        }
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"User [Id={Id}, Email={Email}, FullName={FullName}, Role={Role}, Status={Status}, DeletedAt={(DeletedAt.HasValue ? DeletedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Null")}, CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt}]";
    }
}