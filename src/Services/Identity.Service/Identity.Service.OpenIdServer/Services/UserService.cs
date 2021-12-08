using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Service.OpenIdServer.Models.DataTransferObjects;
using Identity.Service.OpenIdServer.Models.Entities;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.LanguageExt.Common;

namespace Identity.Service.OpenIdServer.Services;

public interface IUserService
{
    Task<Option<(string id, string email, string password, string roles)>> CreateUserWithRolesAsync(CreateUserWithRolesDto dto);
}

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserService(ILogger<UserService> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<Option<(string id, string email, string password, string roles)>> CreateUserWithRolesAsync(CreateUserWithRolesDto dto)
    {
        if (dto.Email.IsNullOrEmpty() || (await _userManager.FindByEmailAsync(dto.Email) != null))
        {
            return Option<(string id, string email, string password, string roles)>.None;
        }

        var applicationUser = _mapper.Map<ApplicationUser>(dto);
        applicationUser.Id = Guid.NewGuid().ToString();

        var password = dto.Password.IsNullOrEmpty()
            ? new StringBuilder("rand")
                .AddRandomNumber(10, 99)
                .AddRandomAlphabet(3)
                .AddRandomSpecialString(3)
                .AddRandomAlphabet(3, true)
                .AddRandomSpecialString(3)
                .ToString()
            : dto.Password;

        await _userManager.CreateAsync(applicationUser, password);

        if (!_roleManager.Roles.Any())
        {
            var roles = dto.Roles
                .Select(r => new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = r,
                    NormalizedName = r.ToUpper()
                });

            foreach (var role in roles) await _roleManager.CreateAsync(role);
        }

        await _userManager.AddToRolesAsync(applicationUser, dto.Roles);
        return (applicationUser.Id, applicationUser.Email, password, dto.Roles.Join(", "));
    }
}