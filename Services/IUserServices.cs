using Microsoft.AspNetCore.Identity;

namespace MyDotNet9Api.Services;

public interface IUserServices
{
    Task<string> ObtainUserId();
}