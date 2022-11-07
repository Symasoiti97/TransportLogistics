using AutoMapper;
using ApplicationTariffDto = Tariffs.Application.TariffServices;
using WebApiTariffDto = Tariffs.Infrastructure.WebApi.Controllers.Tariff.Dto;

namespace Tariffs.Infrastructure.WebApi.Controllers.Tariff;

internal sealed class TariffMapperProfile : Profile
{
    public TariffMapperProfile()
    {
        CreateMap<WebApiTariffDto.SaveTariffRouteRequest, ApplicationTariffDto.SaveTariffRouteCommand>()
            .ForMember(d => d.Points, o => o.MapFrom(s => s.Route.Points))
            .ForMember(d => d.ManagerProfileId, o => o.Ignore())
            .ForMember(d => d.TariffId, o => o.Ignore());

        CreateMap<WebApiTariffDto.PointDto, ApplicationTariffDto.PointDto>();

        CreateMap<WebApiTariffDto.SaveTariffCargoRequest, ApplicationTariffDto.SaveTariffCargoCommand>()
            .ForMember(d => d.CargoType, o => o.MapFrom(s => s.CargoType))
            .ForMember(d => d.ContainerOwn, o => o.MapFrom(s => s.ContainerOwn))
            .ForMember(d => d.ContainerSize, o => o.MapFrom(s => s.ContainerSize))
            .ForMember(d => d.TariffId, o => o.Ignore());

        CreateMap<WebApiTariffDto.SaveTariffPriceRequest, ApplicationTariffDto.SaveTariffPriceCommand>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.TariffId, o => o.Ignore());
    }
}