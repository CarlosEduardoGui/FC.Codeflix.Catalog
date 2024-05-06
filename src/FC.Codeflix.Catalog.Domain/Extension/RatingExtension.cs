using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Domain.Extension;
public static class RatingExtension
{
    public static Rating ToRating(this string ratingString) => ratingString switch
    {
        "ER" => Rating.ER,
        "L" => Rating.L,
        "10" => Rating.Rate_10,
        "12" => Rating.Rate_12,
        "14" => Rating.Rate_14,
        "16" => Rating.Rate_16,
        "18" => Rating.Rate_18,
        _ => throw new ArgumentOutOfRangeException("Invalid")
    };

    public static string ToStringRating(this Rating rating) => rating switch
    {
        Rating.ER => "ER",
        Rating.L => "L",
        Rating.Rate_10 => "10",
        Rating.Rate_12 => "12",
        Rating.Rate_14 => "14",
        Rating.Rate_16 => "16",
        Rating.Rate_18 => "18"
    };
}
