using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Validator;
public class VideoValidator : Validator
{
    private const int MAX_LENGHT = 255;
    private readonly Video _video;

    public VideoValidator(Video video, ValidationHandler handler) : base(handler)
        => _video = video;

    public override void Validate()
    {
        ValidateTitle();
    }

    private void ValidateTitle()
    {
        if (string.IsNullOrEmpty(_video.Title))
            _handler.HandleError(string.Format(ConstantsMessages.fieldNotEmpty, nameof(_video.Title)));

        if (_video.Title.Length > MAX_LENGHT)
            _handler.HandleError(string.Format(ConstantsMessages.fieldNotMaxLenght, nameof(_video.Title), MAX_LENGHT));
    }
}
