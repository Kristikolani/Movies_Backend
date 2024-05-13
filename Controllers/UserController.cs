
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Movies.Models;
using Movies.Models.DTO;
using Movies.Repository.IRepository;

namespace E_Library.Controllers
{
    [Route("api/UsersAuth")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public UsersController(IUserRepository userRepository, IMapper mapper )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            this._response = new();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepository.Login(model);
            if (loginResponse.User == null && string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Email or Password is incorrect");
                return BadRequest(_response);
            }
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = loginResponse;
                return Ok(_response);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserNameUnique = _userRepository.IsUniqueUser(model.UserName) &&
                                    _userRepository.IsUniqueUser(model.Email);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepository.Register(model);
            if (user == null)
            {

                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Error while registering");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = user;
            return Ok(_response);
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetUsers(int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<User> userList = await _userRepository.GetAllAsync(includeProperties:"Reviews,Comments",pageSize:pageSize,pageNumber:pageNumber);
                _response.Result = _mapper.Map<List<UserDTO>>(userList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpGet("{id:int}", Name = "GetUser")]

        public async Task<ActionResult<APIResponse>> GetUser(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var user = await _userRepository.GetAsync(u => u.Id == id, includeProperties:"Reviews,Comments");
                if (user == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<UserDTO>(user);
                _response.StatusCode = HttpStatusCode.OK;

                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteUser")]
        [Authorize(Roles = "admin,user")]


        public async Task<ActionResult<APIResponse>> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepository.GetAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                await _userRepository.DeleteAsync(user);
                await _userRepository.SaveAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPut("{id:int}", Name = "UpdateUser")]
        [Authorize(Roles = "admin,user")]

        public async Task<ActionResult<APIResponse>> UpdateUser(int id, [FromBody] UserUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

                User model = _mapper.Map<User>(updateDTO);

                await _userRepository.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.ToString() };
            }

            return _response;
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Users([FromQuery] string? searchText,string? orderBy, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                Expression<Func<User, object>> orderByExpression = null;

                if (string.IsNullOrEmpty(orderBy))
                {
            
                    orderByExpression = p => p.Id;
                }
                else
                {
                    switch (orderBy)
                    {
                        case "Name":
                            orderByExpression = p => p.Username;
                            break;
                        case "Date":
                            orderByExpression = p => p.CreatedDate;
                            break;

                        default:

                            return BadRequest("Invalid orderBy parameter.");
                    }
                }

                IEnumerable<User> searchResults = await _userRepository.SearchAsync(searchText, includeProperties:"Comments,Reviews",pageSize,pageNumber,orderBy:orderByExpression);
                var searchResultDTOs = _mapper.Map<IEnumerable<UserDTO>>(searchResults);
                var totalCount = await _userRepository.GetTotalCountAsync();
                return Ok(new
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = searchResultDTOs,
                    count = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.ToString() }
                });
            }
        }
    }
}
