using Aimo.Core;
using Aimo.Core.Infrastructure;
using Aimo.Domain.Issues;
using AutoMapper;

namespace Aimo.Application.Issues;

internal class IssueMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public IssueMapperProfile()
    {
        CreateMap<Issue, IssueDto>().AfterMap((_, dto) => dto.Localize());
    }
}