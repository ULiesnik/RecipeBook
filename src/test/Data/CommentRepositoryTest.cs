using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBook.Data.Context;
using RecipeBook.Data.Models;
using RecipeBook.Data.Repositories;

namespace RecipeBookTest.Data;

public class CommentRepositoryTest
{
    private readonly Mock<DatabaseContext> _contextMock;
    private readonly Mock<DbSet<Comment>> _commentSetMock;
    private readonly CommentRepository _repository;

    private readonly User _testUser;
    private readonly Recipe _testRecipe;
    private readonly Comment _testComment;

    public CommentRepositoryTest()
    {
        _contextMock = new Mock<DatabaseContext>();
        _repository = new CommentRepository(_contextMock.Object);
        _commentSetMock = new Mock<DbSet<Comment>>();

        _testUser = new User { Id = "user1", Comments = new List<Comment>() };
        _testRecipe = new Recipe { Id = 1, Comments = new List<Comment>() };
        _testComment = new Comment("Test", DateTime.UtcNow);

        var commentList = new List<Comment> { _testComment }.AsQueryable();

        _commentSetMock.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(commentList.Provider);
        _commentSetMock.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(commentList.Expression);
        _commentSetMock.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(commentList.ElementType);
        _commentSetMock.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(commentList.GetEnumerator());

        _contextMock.Setup(c => c.Set<Comment>()).Returns(_commentSetMock.Object);
    }

    [Fact]
    public void Add_WithValidIds_AddsCommentToUserAndRecipe()
    {
        // Arrange
        _contextMock.Setup(c => c.Find<User>("user1")).Returns(_testUser);
        _contextMock.Setup(c => c.Find<Recipe>(1L)).Returns(_testRecipe);
        _contextMock.Setup(c => c.SaveChanges()).Verifiable();

        // Act
        var result = _repository.Add("user1", 1L, _testComment);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(_testComment, _testUser.Comments);
        Assert.Contains(_testComment, _testRecipe.Comments);
        _contextMock.Verify(c => c.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Add_WithInvalidUserId_ReturnsNull()
    {
        _contextMock.Setup(c => c.Find<Recipe>(1L)).Returns(_testRecipe);
        _contextMock.Setup(c => c.Find<User>("invalid")).Returns((User?)null);

        var result = _repository.Add("invalid", 1L, _testComment);

        Assert.Null(result);
        _contextMock.Verify(c => c.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Add_WithInvalidRecipeId_ReturnsNull()
    {
        _contextMock.Setup(c => c.Find<Recipe>(999L)).Returns((Recipe?)null);

        var result = _repository.Add("user1", 999L, _testComment);

        Assert.Null(result);
        _contextMock.Verify(c => c.SaveChanges(), Times.Never);
    }

    [Fact]
    public void GetAll_ReturnsAllComments()
    {
        var result = _repository.GetAll();
        Assert.Single(result!);
        Assert.Equal("Test", result![0].Text);
    }

    [Fact]
    public void Get_ReturnsCorrectComment()
    {
        _commentSetMock.Setup(m => m.Find(1L)).Returns(_testComment);
        var result = _repository.Get(1L);
        Assert.NotNull(result);
        Assert.Equal("Test", result!.Text);
    }

    [Fact]
    public void Delete_RemovesComment()
    {
        _commentSetMock.Setup(m => m.Find(1L)).Returns(_testComment);
        _commentSetMock.Setup(m => m.Remove(_testComment));
        var result = _repository.Delete(1L);

        _commentSetMock.Verify(m => m.Remove(_testComment), Times.Once);
        _contextMock.Verify(m => m.SaveChanges(), Times.Once);
        Assert.Equal("Test", result!.Text);
    }
}
