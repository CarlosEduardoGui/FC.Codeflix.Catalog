namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;
public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for (int i = 0; i < times; i++)
        {
            var exampleCategory = fixture.GetExampleCategory();
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);
            yield return new object[] { exampleCategory, exampleInput };
        }
    }
}
