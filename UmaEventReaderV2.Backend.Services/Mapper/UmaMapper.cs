using UmaEventReaderV2.Common;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Common.Models.Enums;
using UmaEventReaderV2.Models.dtos;

namespace UmaEventReaderV2.Services.Mapper;

public static class UmaMapper
{
    public static Uma Map(UmaDto dto)
    {
        return new Uma
        {
            Id = GuidGenerator.Generate(dto.Name),
            FullName = dto.Name,
            Character = GetCharacter(dto.Name),
            Variant = GetVariant(dto.Name),
            Rarity = Enum.TryParse<Rarity>(dto.Rarity, true, out var rarity) ? rarity : Rarity.Unknown,
            ImageUrl = dto.ImageUrl
        };
    }

    public static Uma Map(string name, bool supportUma = false)
    {
        return new Uma
        {
            Id = GuidGenerator.Generate(name),
            FullName = name,
            Character = GetCharacter(name),
            Variant = GetVariant(name),
            SupportUma = supportUma
        };
    }

    private static string GetCharacter(string fullName)
    {
        var nameParts = fullName.Split("(", 2, StringSplitOptions.TrimEntries);

        return nameParts[0];
    }

    private static string GetVariant(string fullName)
    {
        var nameParts = fullName.Split("(", 2, StringSplitOptions.TrimEntries);

        return nameParts.Length > 1 ? nameParts[1].TrimEnd(')') : string.Empty;
    }
}