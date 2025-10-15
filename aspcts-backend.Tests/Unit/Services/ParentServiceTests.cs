// Unit/Services/ParentServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using aspcts_backend.Services;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.Entities;
using aspcts_backend.Tests.Fixtures;

namespace aspcts_backend.Tests.Unit.Services;

public class ParentServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IChildRepository> _childRepositoryMock;
    private readonly ParentService _parentService;
    
    public ParentServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _childRepositoryMock = new Mock<IChildRepository>();
        _parentService = new ParentService(_userRepositoryMock.Object, _childRepositoryMock.Object);
    }
    
    [Fact]
    public async Task FindParentByEmailAsync_WithValidEmail_ReturnsParentSearchResult()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent", "parent@test.com");
        var parent = TestDataBuilder.CreateTestParent(parentUser.UserId);
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(parentUser);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(parentUser.UserId))
            .ReturnsAsync(parent);
        
        // Act
        var result = await _parentService.FindParentByEmailAsync("parent@test.com");
        
        // Assert
        result.Should().NotBeNull();
        result!.ParentId.Should().Be(parent.ParentId);
        result.UserId.Should().Be(parentUser.UserId);
        result.Email.Should().Be(parentUser.Email);
        result.FirstName.Should().Be(parentUser.FirstName);
        result.LastName.Should().Be(parentUser.LastName);
    }
    
    [Fact]
    public async Task FindParentByEmailAsync_WithNonExistentEmail_ReturnsNull()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);
        
        // Act
        var result = await _parentService.FindParentByEmailAsync("nonexistent@test.com");
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task FindParentByEmailAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent", "parent@test.com");
        parentUser.IsActive = false;
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(parentUser);
        
        // Act
        var result = await _parentService.FindParentByEmailAsync("parent@test.com");
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task FindParentByEmailAsync_WithPsychologistRole_ReturnsNull()
    {
        // Arrange
        var psychologistUser = TestDataBuilder.CreateTestUser("Psychologist", "psych@test.com");
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(psychologistUser);
        
        // Act
        var result = await _parentService.FindParentByEmailAsync("psych@test.com");
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task FindParentByEmailAsync_WithNullOrEmptyEmail_ReturnsNull()
    {
        // Act
        var resultNull = await _parentService.FindParentByEmailAsync(null!);
        var resultEmpty = await _parentService.FindParentByEmailAsync("");
        var resultWhitespace = await _parentService.FindParentByEmailAsync("   ");
        
        // Assert
        resultNull.Should().BeNull();
        resultEmpty.Should().BeNull();
        resultWhitespace.Should().BeNull();
    }
    
    [Fact]
    public async Task SearchParentsAsync_WithValidQuery_ReturnsMatchingParents()
    {
        // Arrange
        var parent1User = TestDataBuilder.CreateTestUser("Parent", "john@test.com");
        parent1User.FirstName = "John";
        parent1User.LastName = "Doe";
        
        var parent2User = TestDataBuilder.CreateTestUser("Parent", "jane@test.com");
        parent2User.FirstName = "Jane";
        parent2User.LastName = "Doe";
        
        var parent1 = TestDataBuilder.CreateTestParent(parent1User.UserId);
        var parent2 = TestDataBuilder.CreateTestParent(parent2User.UserId);
        
        _userRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { parent1User, parent2User });
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(parent1User.UserId))
            .ReturnsAsync(parent1);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(parent2User.UserId))
            .ReturnsAsync(parent2);
        
        // Act
        var result = await _parentService.SearchParentsAsync("Doe");
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.FirstName == "John");
        result.Should().Contain(p => p.FirstName == "Jane");
    }
    
    [Fact]
    public async Task SearchParentsAsync_WithShortQuery_ReturnsEmptyList()
    {
        // Act
        var result = await _parentService.SearchParentsAsync("Jo");
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task SearchParentsAsync_WithNullOrEmptyQuery_ReturnsEmptyList()
    {
        // Act
        var resultNull = await _parentService.SearchParentsAsync(null);
        var resultEmpty = await _parentService.SearchParentsAsync("");
        
        // Assert
        resultNull.Should().BeEmpty();
        resultEmpty.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetParentInfoAsync_WithValidParentId_ReturnsDetailedInfo()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent", "parent@test.com");
        var parent = TestDataBuilder.CreateTestParent(parentUser.UserId);
        var children = new List<Child>
        {
            TestDataBuilder.CreateTestChild(Guid.NewGuid(), parent.ParentId),
            TestDataBuilder.CreateTestChild(Guid.NewGuid(), parent.ParentId)
        };
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(parent);
        
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(parent.UserId))
            .ReturnsAsync(parentUser);
        
        _childRepositoryMock
            .Setup(x => x.GetByParentIdAsync(parent.ParentId))
            .ReturnsAsync(children);
        
        // Act
        var result = await _parentService.GetParentInfoAsync(parent.ParentId);
        
        // Assert
        result.Should().NotBeNull();
        result!.ParentId.Should().Be(parent.ParentId);
        result.ChildrenCount.Should().Be(2);
        result.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetParentInfoAsync_WithInvalidParentId_ReturnsNull()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Parent?)null);
        
        // Act
        var result = await _parentService.GetParentInfoAsync(Guid.NewGuid());
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetParentInfoAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent", "parent@test.com");
        parentUser.IsActive = false;
        var parent = TestDataBuilder.CreateTestParent(parentUser.UserId);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(parent);
        
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(parent.UserId))
            .ReturnsAsync(parentUser);
        
        // Act
        var result = await _parentService.GetParentInfoAsync(parent.ParentId);
        
        // Assert
        result.Should().BeNull();
    }
}