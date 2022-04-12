#nullable disable
using System.Reflection;
using System.Text.RegularExpressions;

namespace Aimo.Core;

public static class DtoLocalization
{
    public static bool EnableLocalization => false;

    public static IEnumerable<TDto> Coalesce<TDto>(this IEnumerable<TDto> dtos) where TDto : Dto
    {
        if (!EnableLocalization) return dtos;

        foreach (var dto in dtos)
            dto.Coalesce();
        return dtos;
    }

    public static TDto Coalesce<TDto>(this TDto dto) where TDto : Dto
    {
        if (!EnableLocalization) return dto;

        var en = Language.En.ToString();
        var ar = Language.Ar.ToString();

        bool OnlyLanguageAndNestedDtoProps(PropertyInfo w)
            => w.Name.EndsWith(en) || w.Name.EndsWith(ar) ||
               typeof(Dto).IsAssignableFrom(w.PropertyType) ||
               typeof(IEnumerable<Dto>).IsAssignableFrom(w.PropertyType);

        var properties = dto?.GetType()?.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            ?.Where(OnlyLanguageAndNestedDtoProps)?.ToArray();

        if (properties.IsNullOrEmpty())
            return dto;

        foreach (var propInfo in properties!)
        {
            var propValue = propInfo.GetValue(dto, default);

            #region RecurseIfPropIsDto

            if (typeof(Dto).IsAssignableFrom(propInfo.PropertyType))
            {
                (propValue as TDto)?.Coalesce();
                continue;
            }

            if (typeof(IEnumerable<Dto>).IsAssignableFrom(propInfo.PropertyType))
            {
                (propValue as IEnumerable<TDto>)!.Coalesce();
                continue;
            }

            if (propValue.IsNotNull()) continue;

            #endregion

            var otherPropName =
                $"{Regex.Replace(propInfo.Name, $"({ar}|{en})", string.Empty)}{(propInfo.Name.EndsWith(en) ? ar : en)}";
            var otherLangPropInfo =
                properties?.FirstOrDefault(w => w.Name.ToLower().Equals(otherPropName.ToLower()));
            var coalesceValue = otherLangPropInfo?.GetValue(dto, null);
            if (coalesceValue.IsNotNull()) propInfo?.SetValue(dto, coalesceValue, default);
        }

        return dto;
    }


    public static IEnumerable<TDto> Localize<TDto>(this IEnumerable<TDto> dtos, Language language = Language.En)
        where TDto : Dto
    {
        if (!EnableLocalization) return dtos;

        foreach (var dto in dtos)
            dto.Localize(language);
        return dtos;
    }

    public static TDto Localize<TDto>(this TDto dto, Language language = Language.En) where TDto : Dto
    {
        if (!EnableLocalization) return dto;

        var en = Language.En.ToString();
        var ar = Language.Ar.ToString();
        const string localized = "Localized";

        bool OnlyLanguageAndNestedDtoProps(PropertyInfo w)
            => w.Name.EndsWith(en) || w.Name.EndsWith(ar) || w.Name.EndsWith(localized) ||
               typeof(Dto).IsAssignableFrom(w.PropertyType) ||
               typeof(IEnumerable<Dto>).IsAssignableFrom(w.PropertyType);

        var properties = dto?.GetType()?.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            ?.Where(OnlyLanguageAndNestedDtoProps)?.ToArray();

        if (properties.IsNullOrEmpty())
            return dto;

        var propEndingWithLocalized =
            properties?.Where(pi =>
                pi.Name.EndsWith(localized) ||
                typeof(IEnumerable<Dto>).IsAssignableFrom(pi.PropertyType) ||
                typeof(Dto).IsAssignableFrom(pi.PropertyType)) ??
            Enumerable.Empty<PropertyInfo>();

        foreach (var propInfo in propEndingWithLocalized)
        {
            var propValue = propInfo.GetValue(dto, default);

            #region RecurseIfPropIsDto

            if (typeof(Dto).IsAssignableFrom(propInfo.PropertyType))
            {
                (propValue as TDto)?.Localize(language);
                continue;
            }

            if (typeof(IEnumerable<Dto>).IsAssignableFrom(propInfo.PropertyType))
            {
                (propValue as IEnumerable<TDto>)?.Localize(language);
                continue;
            }

            if (propValue.IsNotNull()) continue;

            #endregion

            var activeLanguagePropName = $"{Regex.Replace(propInfo.Name, localized, string.Empty)}{language}";
            var activeLanguageProp =
                properties?.FirstOrDefault(w => w.Name.ToLower().Equals(activeLanguagePropName.ToLower()));

            var fallbackPropName =
                $"{Regex.Replace(propInfo.Name, $"({ar}|{en})", string.Empty)}{(propInfo.Name.EndsWith(en) ? ar : en)}";

            var value = activeLanguageProp?.GetValue(dto, null);

            value ??= properties?.FirstOrDefault(w =>
                    w.Name.ToLower().Equals(fallbackPropName.ToLower()))
                ?.GetValue(dto, null);

            if (value.IsNotNull()) propInfo?.SetValue(dto, value, default);
        }

        return dto;
    }

    /*public static Result<IEnumerable<TDto>> Localize<TDto>(this Result<IEnumerable<TDto>> result) where TDto : Dto
        {
            if (!EnableLocalization) return dto;
             result.Data.Localize();
             return result;
        }
        
        public static Result<TDto> Localize<TDto>(this Result<TDto> result) where TDto : Dto
        {
            if (!EnableLocalization) return dto;
            result.Data.Localize();
            return result;
        }*/

    #region Mvc _ViewStart

    /*public static TDto ApplyLocalization<TDto>(this TDto model, Language activeLanguage)
    {
        if (model is null) return model;

        switch (model)
        {
            case Dto dto:
                dto.Localize(activeLanguage);
                break;
            case IEnumerable<Dto> dtos:
                dtos.Localize(activeLanguage);
                break;

            case Result result:
            {
                var dataType = result.Data.GetType();
                //#Result<TDto> where TDto :Dto
                if (typeof(Dto).IsAssignableFrom(dataType))
                {
                    if (result.Data is Dto rDto)
                        rDto.Localize(activeLanguage);
                }
                //#Result<IEnumerable<TDto>> where TDto :Dto
                else if (typeof(IEnumerable<Dto>).IsAssignableFrom(dataType))
                {
                    if (result.Data is IEnumerable<Dto> rDtos)
                        rDtos.Localize(activeLanguage);
                }

                break;
            }
        }

        return model;
    }*/

    #endregion
}