// Unit/Services/ChildServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using aspcts_backend.Services;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.Entities;
using aspcts_backend.Models.DTOs.Child;
using aspcts_backend.Tests.Fixtures;

namespace aspcts_backend.Tests.Unit.Services;

public class ChildServiceTests
{
    private readonly Mock<IChildRepository> _childRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ChildService _childService;
    
    public ChildServiceTests()
    {
        _childRepositoryMock = new Mock<IChildRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _childService = new ChildService(_childRepositoryMock.Object, _userRepositoryMock.Object, _mapperMock.Object);
    }
    
    [Fact]
    public async Task CreateChildAsync_WithValidData_CreatesChild()
    {
        // Arrange
        var psychologistId = Guid.NewGuid();
        var primaryParent = TestDataBuilder.CreateTestParent(Guid.NewGuid());
        var request = TestDataBuilder.CreateChildRequest(primaryParent.ParentId);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(request.PrimaryParentId))
            .ReturnsAsync(primaryParent);
        
        _childRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Child>()))
            .ReturnsAsync((Child c) => c);
        
        var createdChild = TestDataBuilder.CreateTestChild(psychologistId, primaryParent.ParentId);
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdChild);
        
        // Act
        var result = await _childService.CreateChildAsync(request, psychologistId);
        
        // Assert
        result.Should().NotBeNull();
        _childRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Child>()), Times.Once);
        _childRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CreateChildAsync_WithInvalidPrimaryParent_ThrowsException()
    {
        // Arrange
        var psychologistId = Guid.NewGuid();
        var request = TestDataBuilder.CreateChildRequest(Guid.NewGuid());
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(request.PrimaryParentId))
            .ReturnsAsync((Parent?)null);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _childService.CreateChildAsync(request, psychologistId)
        );
    }
    
    [Fact]
    public async Task CreateChildAsync_WithTwoParents_CreatesChildSuccessfully()
    {
        // Arrange
        var psychologistId = Guid.NewGuid();
        var primaryParent = TestDataBuilder.CreateTestParent(Guid.NewGuid(), "Father");
        var secondaryParent = TestDataBuilder.CreateTestParent(Guid.NewGuid(), "Mother");
        var request = TestDataBuilder.CreateChildRequest(primaryParent.ParentId, secondaryParent.ParentId);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(primaryParent.ParentId))
            .ReturnsAsync(primaryParent);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(secondaryParent.ParentId))
            .ReturnsAsync(secondaryParent);
        
        _childRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Child>()))
            .ReturnsAsync((Child c) => c);
        
        var createdChild = TestDataBuilder.CreateTestChild(psychologistId, primaryParent.ParentId, secondaryParent.ParentId);
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdChild);
        
        // Act
        var result = await _childService.CreateChildAsync(request, psychologistId);
        
        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetChildByIdAsync_AsPsychologist_ReturnsChild()
    {
        // Arrange
        var psychologistUser = TestDataBuilder.CreateTestUser("Psychologist");
        var psychologist = TestDataBuilder.CreateTestPsychologist(psychologistUser.UserId);
        var child = TestDataBuilder.CreateTestChild(psychologist.PsychologistId, Guid.NewGuid());
        
        _userRepositoryMock
            .Setup(x => x.GetPsychologistByUserIdAsync(psychologistUser.UserId))
            .ReturnsAsync(psychologist);
        
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.GetChildByIdAsync(child.ChildId, psychologistUser.UserId, "Psychologist");
        
        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetChildByIdAsync_AsUnauthorizedPsychologist_ReturnsNull()
    {
        // Arrange
        var psychologistUser = TestDataBuilder.CreateTestUser("Psychologist");
        var psychologist = TestDataBuilder.CreateTestPsychologist(psychologistUser.UserId);
        var otherPsychologistId = Guid.NewGuid();
        var child = TestDataBuilder.CreateTestChild(otherPsychologistId, Guid.NewGuid());
        
        _userRepositoryMock
            .Setup(x => x.GetPsychologistByUserIdAsync(psychologistUser.UserId))
            .ReturnsAsync(psychologist);
        
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.GetChildByIdAsync(child.ChildId, psychologistUser.UserId, "Psychologist");
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetChildrenAsync_AsPsychologist_ReturnsOnlyAssignedChildren()
    {
        // Arrange
        var psychologistUser = TestDataBuilder.CreateTestUser("Psychologist");
        var psychologist = TestDataBuilder.CreateTestPsychologist(psychologistUser.UserId);
        var children = new List<Child>
        {
            TestDataBuilder.CreateTestChild(psychologist.PsychologistId, Guid.NewGuid()),
            TestDataBuilder.CreateTestChild(psychologist.PsychologistId, Guid.NewGuid())
        };
        
        _userRepositoryMock
            .Setup(x => x.GetPsychologistByUserIdAsync(psychologistUser.UserId))
            .ReturnsAsync(psychologist);
        
        _childRepositoryMock
            .Setup(x => x.GetByPsychologistIdAsync(psychologist.PsychologistId))
            .ReturnsAsync(children);
        
        // Act
        var result = await _childService.GetChildrenAsync(psychologistUser.UserId, "Psychologist");
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetChildrenAsync_AsParent_ReturnsOnlyOwnChildren()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent");
        var parent = TestDataBuilder.CreateTestParent(parentUser.UserId);
        var children = new List<Child>
        {
            TestDataBuilder.CreateTestChild(Guid.NewGuid(), parent.ParentId)
        };
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(parentUser.UserId))
            .ReturnsAsync(parent);
        
        _childRepositoryMock
            .Setup(x => x.GetByParentIdAsync(parent.ParentId))
            .ReturnsAsync(children);
        
        // Act
        var result = await _childService.GetChildrenAsync(parentUser.UserId, "Parent");
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task UpdateChildAsync_WithValidData_UpdatesChild()
    {
        // Arrange
        var psychologistId = Guid.NewGuid();
        var child = TestDataBuilder.CreateTestChild(psychologistId, Guid.NewGuid());
        var updateRequest = new ChildUpdateRequest
        {
            FirstName = "Updated Name"
        };
        
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.UpdateChildAsync(child.ChildId, updateRequest, psychologistId);
        
        // Assert
        result.Should().NotBeNull();
        _childRepositoryMock.Verify(x => x.Update(It.IsAny<Child>()), Times.Once);
        _childRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateChildAsync_AsUnauthorizedPsychologist_ReturnsNull()
    {
        // Arrange
        var psychologistId = Guid.NewGuid();
        var otherPsychologistId = Guid.NewGuid();
        var child = TestDataBuilder.CreateTestChild(otherPsychologistId, Guid.NewGuid());
        var updateRequest = new ChildUpdateRequest { FirstName = "Updated" };
        
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.UpdateChildAsync(child.ChildId, updateRequest, psychologistId);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteChildAsync_WithValidId_SoftDeletesChild()
    {
        // Arrange
        var psychologistId = Guid.NewGuid();
        var child = TestDataBuilder.CreateTestChild(psychologistId, Guid.NewGuid());
        
        _childRepositoryMock
            .Setup(x => x.GetByIdAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.DeleteChildAsync(child.ChildId, psychologistId);
        
        // Assert
        result.Should().BeTrue();
        _childRepositoryMock.Verify(x => x.Update(It.Is<Child>(c => c.IsActive == false)), Times.Once);
    }
    
    [Fact]
    public async Task CanAccessChildAsync_AsPrimaryParent_ReturnsTrue()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent");
        var parent = TestDataBuilder.CreateTestParent(parentUser.UserId);
        var child = TestDataBuilder.CreateTestChild(Guid.NewGuid(), parent.ParentId);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(parentUser.UserId))
            .ReturnsAsync(parent);
        
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.CanAccessChildAsync(child.ChildId, parentUser.UserId, "Parent");
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task CanAccessChildAsync_AsSecondaryParent_ReturnsTrue()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent");
        var parent = TestDataBuilder.CreateTestParent(parentUser.UserId);
        var child = TestDataBuilder.CreateTestChild(Guid.NewGuid(), Guid.NewGuid(), parent.ParentId);
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(parentUser.UserId))
            .ReturnsAsync(parent);
        
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.CanAccessChildAsync(child.ChildId, parentUser.UserId, "Parent");
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task CanAccessChildAsync_AsUnrelatedParent_ReturnsFalse()
    {
        // Arrange
        var parentUser = TestDataBuilder.CreateTestUser("Parent");
        var parent = TestDataBuilder.CreateTestParent(parentUser.UserId);
        var child = TestDataBuilder.CreateTestChild(Guid.NewGuid(), Guid.NewGuid());
        
        _userRepositoryMock
            .Setup(x => x.GetParentByUserIdAsync(parentUser.UserId))
            .ReturnsAsync(parent);
        
        _childRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(child.ChildId))
            .ReturnsAsync(child);
        
        // Act
        var result = await _childService.CanAccessChildAsync(child.ChildId, parentUser.UserId, "Parent");
        
        // Assert
        result.Should().BeFalse();
    }
}