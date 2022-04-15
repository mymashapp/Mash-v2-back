namespace Aimo.Core;

//TODO: change to smart enums https://github.com/ardalis/SmartEnum
public enum ResultType
{
    Error = 0,
    Success = 1,
    Info = 2,
    Warning = 3
}

public enum Language
{
    En = 1,
    Ar = 2
}

public enum SortDirection
{
    Desc,
    Asc
}

public static class EnumExtension
{
    public static List<IdNameDto> ToIdNameList(this Language lang)
    {
        return Enum.GetValues(typeof(Language)).Cast<Language>()
            .Select(x => new IdNameDto
            {
                Id = (int)x,
                Name = x.ToString(),
            }).ToList();
    }

    public static List<IdNameLocalizedDto> ToIdNameLocalizedList(this Language lang)
    {
        return Enum.GetValues(typeof(Language)).Cast<Language>()
            .Select(x => new IdNameLocalizedDto
            {
                Id = (int)x,
                NameEn = x.ToString(),
                NameAr = x.ToString(),
            }).ToList();
    }
}