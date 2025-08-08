using AutoMapper;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;
using NetTopologySuite.Geometries;

namespace MyDotNet9Api.Utilities;

public class AutoMapperProfiles: Profile
{
    // private readonly GeometryFactory _geometryFactory;
    public AutoMapperProfiles(GeometryFactory geometryFactory)
    {
        ConfigureGenres();
        ConfigureActors();
        ConfigureTheaters(geometryFactory);
    }

    private void ConfigureTheaters(GeometryFactory geometryFactory)
    {
        CreateMap<Theater, TheaterDTO>()
            .ForMember(dto => dto.Latitude,  opt => opt.MapFrom(src => src.Location.Y))
            .ForMember(dto => dto.Longitude, opt => opt.MapFrom(src => src.Location.X));

        CreateMap<TheaterCreationDTO, Theater>().ForMember(entity => entity.Location, dto => dto.MapFrom(p =>
            geometryFactory.CreatePoint(new Coordinate(p.Latitude, p.Longitude))
        ));
    }

    private void ConfigureActors()
    {
        CreateMap<ActorCreationDTO, Actor>().ForMember(x => x.Picture, 
            options => options.Ignore());
        CreateMap<Actor, ActorDTO>();
    }

    private void ConfigureGenres()
    {
        CreateMap<GenreCreationDTO, Genre>();
        CreateMap<Genre, GenreDTO>();
    }
}