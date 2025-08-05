using AutoMapper;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;

namespace MyDotNet9Api.Utilities;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        ConfigureGenres();
    }

    private void ConfigureGenres()
    {
        CreateMap<GenreCreationDTO, Genre>();
        CreateMap<Genre, GenreDTO>();
    }
}