using Domain;
using Services.Tags.Dto;

namespace IntegrationTests.TestData;

public static class TagsTestData
{
    private static readonly List<Tag> _data = new();

    public static IReadOnlyCollection<Tag> Data => _data;
    public static IReadOnlyCollection<object[]> TestData => _data.Select(tag => new object[] { tag }).ToList();
    public static IReadOnlyCollection<TagDto> DtoData => _data.Select(TagDto.FromTag).ToList();

    static TagsTestData()
    {
        _data = new()
        {
            new Tag("Cat"),
            new Tag("Dog"),
            new Tag("Starving"),
            new Tag("Injured"),
            new Tag("Sick"),
            new Tag("Adoption"),
            new Tag("Aggressive")
        };
    }
}
