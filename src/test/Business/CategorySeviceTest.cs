using Moq;
using RecipeBook.Business.Services;
using RecipeBook.Data.Models;
using RecipeBook.Data.Repositories;

namespace RecipeBookTest.Business;

public class CategoryServiceTest
{
    private readonly CategoryService _categoryService;
    private readonly Mock<CategoryRepository> _mockRepo;

    public CategoryServiceTest()
    {
        _mockRepo = new Mock<CategoryRepository>(MockBehavior.Strict, null!);

        _categoryService = new CategoryService(_mockRepo.Object);
    }

    [Fact]
    public void AddNewCategory_ValidName_ReturnsMappedDto()
    {
        // Arrange
        var name = "Dessert";
        var category = new Category(name);
        _mockRepo.Setup(r => r.Add(It.IsAny<Category>())).Returns(category);

        // Act
        var result = _categoryService.AddNewCategory(name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result!.Name);
        _mockRepo.Verify(r => r.Add(It.Is<Category>(c => c.Name == name)), Times.Once);
    }

    [Fact]
    public void GetAll_WhenRepositoryReturnsList_ReturnsMappedList()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Appetizer"),
            new Category("Main")
        };
        _mockRepo.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _categoryService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.Equal("Appetizer", result[0].Name);
        Assert.Equal("Main", result[1].Name);
        _mockRepo.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetAll_WhenRepositoryReturnsNull_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAll()).Returns((List<Category>?)null);

        // Act
        var result = _categoryService.GetAll();

        // Assert
        Assert.Null(result);
        _mockRepo.Verify(r => r.GetAll(), Times.Once);
    }
}
