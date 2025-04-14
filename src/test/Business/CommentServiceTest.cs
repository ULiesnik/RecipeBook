using RecipeBookTest.Business.Proxy;

namespace RecipeBookTest.Business;

using Moq;
using RecipeBook.Data.Models;
using RecipeBook.Business.Services;

public sealed class CommentServiceTest
{
    readonly Mock<CommentRepositoryMoqProxy> _commentRepositoryMock;
    readonly CommentService _commentService;
    readonly DateTime _testTime;
    readonly string _wrongUserId;
    readonly long _wrongRecipeId;
    readonly Comment _testComment;

    public CommentServiceTest()
    {
        _commentRepositoryMock = new Mock<CommentRepositoryMoqProxy>();
        InitMockMethods();
        _commentService = new CommentService(_commentRepositoryMock.Object);
        _testTime = DateTime.SpecifyKind(new DateTime(2021, 1, 1),
            DateTimeKind.Utc);

        _wrongUserId = "-1";
        _wrongRecipeId = -1L;
        _testComment = new Comment("Test", _testTime);
    }

    void InitMockMethods()
    {
        _commentRepositoryMock
            .Setup(r => r.Add(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<Comment>()))
            .Returns((string uid, long recipeId, Comment comment) =>
                uid == _wrongUserId || recipeId == _wrongRecipeId
                    ? null
                    : _testComment);
    }

    [Fact]
    public void AddComment_ReturnsTestComment()
    {
        var comment = _commentService.AddComment("1", 1, "Test");
        _commentRepositoryMock.Verify(r => r.Add("1", 1L, It.IsAny<Comment>()));
        Assert.Equal("Test", comment?.Text);
        Assert.Equal(_testTime, comment?.Time);
    }

    [Fact]
    public void AddComment_WithWrongId_ReturnsNull()
    {
        var comment = _commentService.AddComment(_wrongUserId, _wrongRecipeId, "Test");
        _commentRepositoryMock.Verify(r =>
            r.Add(_wrongUserId, _wrongRecipeId, It.IsAny<Comment>()));
        Assert.Null(comment);
    }
}