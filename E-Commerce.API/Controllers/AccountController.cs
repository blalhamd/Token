using AutoMapper;
using E_Commerce.Core.IServices;
using E_Commerce.Core.ViewModels.User;
using E_Commerce.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(ITokenService tokenService, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                return Unauthorized(user);
            }

            var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, model.Password,false);

            if (!checkPassword.Succeeded)
                return Unauthorized();

            await _signInManager.SignInAsync(user, model.RememberMe);

            var token = _tokenService.GetToken(user);

            return Ok(token); 
        }


        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            if(CheckEmailExist(model.Email).Result.Value)
                return BadRequest(model);

            var user = _mapper.Map<AppUser>(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                var token = _tokenService.GetToken(user);

                return Ok(token);
            }

            return BadRequest(model);

        }

        [HttpGet]
        public async Task<ActionResult<bool>> CheckEmailExist([FromQuery]string email)
            => await _userManager.FindByEmailAsync(email) != null;




    }
}
