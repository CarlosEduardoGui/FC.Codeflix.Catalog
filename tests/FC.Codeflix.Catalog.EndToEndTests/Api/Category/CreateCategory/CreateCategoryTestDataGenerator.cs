namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;
public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int i = 0; i < totalInvalidCases; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetExampleInput();
                    input1.Name = fixture.GetInvalidNameTooShort();
                    invalidInputList.Add(new object[] {
                        input1,
                        "Name should be at leats 3 characters long."
                    });
                    break;
                case 1:
                    var input2 = fixture.GetExampleInput();
                    input2.Name = fixture.GetInvalidNameTooLong();
                    invalidInputList.Add(new object[] {
                        input2,
                        "Name should be less or equal 255 characters long."
                    });
                    break;
                case 2:
                    var input3 = fixture.GetExampleInput();
                    input3.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputList.Add(new object[] {
                        input3,
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
