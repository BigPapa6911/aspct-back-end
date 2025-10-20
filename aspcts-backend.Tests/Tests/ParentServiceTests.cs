// Tests/ParentServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using aspcts_backend.Services;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Tests;

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
    
    private User CreateTestUser(string role = "Parent", string email = "parent@test.com")
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Username = "testparent",
            Email = email,
            Role = role,
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };
    }
    
    private Parent CreateTestParent(Guid userId, string relationship = "Father")
    {
        return new Parent
        {
            ParentId = Guid.NewGuid(),
            UserId = userId,
            ChildRelationship = relationship
        };
    }
    
    [Fact]
    public async Task FindParentByEmailAsync_WithValidEmail_ReturnsParentSearchResult()
    {
        // Arrange
        var parentUser = CreateTestUser("Parent", "parent@test.com");
        var parent = CreateTestParent(parentUser.UserId);
        
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
        result.Email.Should().Be(parentUser.Email);
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
        var parentUser = CreateTestUser("Parent");
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
        var psychologistUser = CreateTestUser("Psychologist");
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(psychologistUser);
        
        // Act
        var result = await _parentService.FindParentByEmailAsync("psych@test.com");
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task SearchParentsAsync_WithValidQuery_ReturnsMatchingParents()
    {
        // Arrange
        var parent1User = CreateTestUser("Parent", "john@test.com");
        parent1User.FirstName = "John";
        parent1User.LastName = "Doe";
        
        var parent2User = CreateTestUser("Parent", "jane@test.com");
        parent2User.FirstName = "Jane";
        parent2User.LastName = "Doe";
        
        var parent1 = CreateTestParent(parent1User.UserId);
        var parent2 = CreateTestParent(parent2User.UserId);
        
        _userRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
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
    public async Task GetParentInfoAsync_WithValidParentId_ReturnsDetailedInfo()
    {
        // Arrange
        var parentUser = CreateTestUser("Parent");
        var parent = CreateTestParent(parentUser.UserId);
        var children = new List<Child>
        {
            new()
            {
                ChildId = Guid.NewGuid(),
                FirstName = "Child1",
                LastName = "Test",
                PrimaryParentId = parent.ParentId,
                IsActive = true
            },
            new()
            {
                ChildId = Guid.NewGuid(),
                FirstName = "Child2",
                LastName = "Test",
                PrimaryParentId = parent.ParentId,
                IsActive = true
            }
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
        var parentUser = CreateTestUser("Parent");
        parentUser.IsActive = false;
        var parent = CreateTestParent(parentUser.UserId);
        
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