namespace FC.Codeflix.Catalog.UnitTests.Application.Category.UpdateCategory;
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

    public static IEnumerable<object[]> GetInvalidInputs(int numberOfTest = 12)
    {
        var fixture = new UpdateCategoryTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int i = 0; i < numberOfTest; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(new object[] {
                        fixture.GetInvalidInputShortName(),
                        "Name should be at leats 3 characters long."
                    });
                    break;
                case 1:
                    invalidInputList.Add(new object[] {
                        fixture.GetInvalidInputTooLongName(),
                        "Name should be less or equal 255 characters long."
                    });
                    break;
                case 2:
                    invalidInputList.Add(new object[] {
                        fixture.GetInvalidPutToLongDescription(),
                        "Description should be less or equal 10000 characters long."
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputList;
    }
}
