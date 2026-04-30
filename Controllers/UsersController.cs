using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.Wrappers;
using ShoppingApp.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class UsersController(IUserService userService, ITokenService tokenService, IMapper mapper) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IMapper _mapper = mapper;
    }
}
