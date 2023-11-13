namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;
public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int numberOfTest = 12)
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 4;

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
                        fixture.GetInvalidPutDescriptionNull(),
                        "Description should not be null."
                    });
                    break;
                case 3:
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
