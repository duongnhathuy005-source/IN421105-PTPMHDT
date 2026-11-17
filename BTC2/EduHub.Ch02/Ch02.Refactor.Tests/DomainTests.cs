using EduHub.Ch02;
using EduHub.Ch02.Domain;

namespace Ch02.Refactor.Tests;

// Mẫu AAA: Arrange (chuẩn bị) - Act (thực hiện) - Assert (kiểm tra).
// Mỗi test chỉ kiểm tra MỘT hành vi. Đặt tên: TênMethod_TìnhHuống_KếtQuảMongĐợi.
public class DomainTests
{
    // Test mẫu: kiểm tra luật validate qua ClubService (sau khi refactor).
    [Fact]
    public void CreateClub_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var service = new ClubService(
            new InMemoryClubRepository(),
            new InMemoryMemberRepository(),
            new NullAppLogger(),
            new NotificationServiceFactory());

        // Act
        var act = () => service.CreateClub("", "mô tả", "p@gmail.com");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_Create_WithValidName_ReturnsClub()
    {
        // Arrange
        var name = "CLB Lập trình";
        var description = "Sinh hoạt hàng tuần";

        // Act
        var club = Club.Create(name, description);

        // Assert
        Assert.Equal(name, club.Name);
        Assert.Equal(description, club.Description);
        Assert.NotEqual(Guid.Empty, club.Id);
    }

    [Fact]
    public void Club_Create_WithInvalidName_ThrowsArgumentException()
    {
        // Arrange
        var emptyName = "   ";
        var tooLongName = new string('a', Club.MaxNameLength + 1);

        // Act
        var actEmpty = () => Club.Create(emptyName, "mô tả");
        var actTooLong = () => Club.Create(tooLongName, "mô tả");

        // Assert
        Assert.Throws<ArgumentException>(actEmpty);
        Assert.Throws<ArgumentException>(actTooLong);
    }

    [Fact]
    public void Club_HasRoom_AtMaxMembers_ReturnsFalse()
    {
        // Arrange
        var club = Club.Create("CLB A", "mô tả");

        // Act
        var atBoundary49 = club.HasRoom(Club.MaxMembers - 1);
        var atBoundary50 = club.HasRoom(Club.MaxMembers);

        // Assert
        Assert.True(atBoundary49);
        Assert.False(atBoundary50);
    }

    [Fact]
    public void Event_Register_NewMember_ReturnsTrue()
    {
        // Arrange
        var ev = new Event { Title = "Workshop" };
        var memberId = Guid.NewGuid();

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.True(result);
        Assert.Contains(memberId, ev.RegisteredMemberIds);
    }

    [Fact]
    public void Event_Register_SameMemberTwice_ReturnsFalse()
    {
        // Arrange
        var ev = new Event { Title = "Workshop" };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Event_CheckIn_UnregisteredMember_ThrowsInvalidOperationException()
    {
        // Arrange
        var ev = new Event { Title = "Workshop" };
        var memberId = Guid.NewGuid();

        // Act
        var act = () => ev.CheckIn(memberId);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}

// Logger giả: không ghi file, dùng riêng cho test.
internal class NullAppLogger : IAppLogger
{
    public void Log(string message) { }
}