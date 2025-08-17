using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        ConfigureMovies();
        ConfigureMappingUsers();
    }

    private void ConfigureMappingUsers()
    {
        CreateMap<IdentityUser, UserDTO>();
    }

    private void ConfigureMovies()
    {
        CreateMap<MovieCreationDTO, Movie>()                                    
            .ForMember(ent => ent.Poster, options => options.Ignore())
            .ForMember(ent => ent.MoviesGenres, dto=> dto
                .MapFrom(p=> p.GenresIds!.Select((id=>new MovieGenre(){GenreId = id} ))))
            .ForMember(ent => ent.MoviesTheaters, dto=> dto
                .MapFrom(p=> p.TheatersIds!.Select((id=>new MovieTheater(){TheaterId = id} ))))
            .ForMember(ent => ent.MoviesActors, dto=> dto
                .MapFrom(p=> p.Actors!.Select((actor=>new MovieActor{ActorId = actor.Id, Character = actor.Character} ))));

        CreateMap<Movie, MovieDTO>();
        CreateMap<MovieDetailsDTO, MovieDTO>();

        CreateMap<Movie, MovieDetailsDTO>()
            .ForMember(dto => dto.Genres, ent => ent.MapFrom(p => p.MoviesGenres))
            .ForMember(dto => dto.Theaters, ent => ent.MapFrom(p => p.MoviesTheaters))
            .ForMember(dto => dto.Actors, ent => ent.MapFrom(p => p.MoviesActors.OrderByDescending(ma=>ma.Order )));

        CreateMap<MovieGenre, GenreDTO>()
            .ForMember(dto => dto.Id, ent => ent.MapFrom(p => p.GenreId))
            .ForMember(dto => dto.Name, ent => ent.MapFrom(p => p.Genre.Name));


        CreateMap<MovieTheater, TheaterDTO>()
            .ForMember(dto => dto.Id, ent => ent.MapFrom(p => p.TheaterId))
            .ForMember(dto => dto.Name, ent => ent.MapFrom(p => p.Theater.Name))
            .ForMember(dto => dto.Longitude, ent => ent.MapFrom(p => p.Theater.Location.X))
            .ForMember(dto => dto.Latitude, ent => ent.MapFrom(p => p.Theater.Location.Y));


        CreateMap<MovieActor, MovieActorDTO>()
            .ForMember(dto => dto.Id, ent => ent.MapFrom(property => property.ActorId))
            .ForMember(dto => dto.Name, ent => ent.MapFrom(property => property.Actor.Name))
            .ForMember(dto => dto.Picture, ent => ent.MapFrom(property => property.Actor.Picture));
    }
    private void ConfigureTheaters(GeometryFactory geometryFactory)
    {
        CreateMap<Theater, TheaterDTO>()
            .ForMember(dto => dto.Latitude,  opt => opt.MapFrom(src => src.Location.Y))
            .ForMember(dto => dto.Longitude, opt => opt.MapFrom(src => src.Location.X));

        CreateMap<TheaterCreationDTO, Theater>().ForMember(entity => entity.Location, dto => dto.MapFrom(p =>
            geometryFactory.CreatePoint(new Coordinate(p.Longitude, p.Latitude))
        ));
    }


    private void ConfigureActors()
    {
        CreateMap<ActorCreationDTO, Actor>().ForMember(x => x.Picture, 
            options => options.Ignore());
        CreateMap<Actor, ActorDTO>();
        CreateMap<Actor, MovieActorDTO>();
    }

    private void ConfigureGenres()
    {
        CreateMap<GenreCreationDTO, Genre>();
        CreateMap<Genre, GenreDTO>();
    }
}