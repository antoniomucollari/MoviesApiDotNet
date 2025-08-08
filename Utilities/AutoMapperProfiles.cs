using AutoMapper;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;
using NetTopologySuite.Geometries;

namespace MyDotNet9Api.Utilities;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles(GeometryFactory geometryFactory)
    {
        ConfigureGenres();
        ConfigureActors();
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