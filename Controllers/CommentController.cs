using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Movies.Models;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Repository.IRepository;

namespace Movies.Controllers
{
    [Route("api/Comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ICommentRepository _dbComment;
        private readonly IMapper _mapper;

        public CommentController(ICommentRepository dbComment, IMapper mapper)
        {
            _dbComment = dbComment;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetComments(int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Comment> commentList = await _dbComment.GetAllAsync(includeProperties:"Item,User",pageSize:pageSize,pageNumber:pageNumber);
                _response.Result = _mapper.Map<List<CommentDTO>>(commentList);
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

        [HttpGet("{id:int}", Name = "GetComment")]

        public async Task<ActionResult<APIResponse>> GetComment(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var comment = await _dbComment.GetAsync(u => u.Id == id, includeProperties:"Item,User");
                if (comment == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<CommentDTO>(comment);
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

        [HttpPost]
        [Authorize(Roles = "admin,user")]

        public async Task<ActionResult<APIResponse>> CreateComment([FromBody] CommentCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                Comment comment = _mapper.Map<Comment>(createDTO);

                await _dbComment.CreateAsync(comment);
                await _dbComment.SaveAsync();
                _response.Result = _mapper.Map<CommentDTO>(comment);
                _response.StatusCode = HttpStatusCode.Created;

                _response.IsSuccess = true;
                return CreatedAtRoute("GetComment", new { id = comment.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteComment")]
        [Authorize(Roles = "admin,user")]


        public async Task<ActionResult<APIResponse>> DeleteComment(int id)
        {
            try
            {
                var comment = await _dbComment.GetAsync(u => u.Id == id);
                if (comment == null)
                {
                    return NotFound();
                }

                await _dbComment.DeleteAsync(comment);
                await _dbComment.SaveAsync();
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

        [HttpPut("{id:int}", Name = "UpdateComment")]
        [Authorize(Roles = "admin,user")]

        public async Task<ActionResult<APIResponse>> UpdateComment(int id, [FromBody] CommentUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

                Comment model = _mapper.Map<Comment>(updateDTO);

                await _dbComment.UpdateAsync(model);
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
        [HttpPut("{id:int}/like", Name = "LikeComment")]
        public async Task<ActionResult<APIResponse>> LikeComment(int id)
        {
            try
            {
                var comment = await _dbComment.GetAsync(u => u.Id == id);
                if (comment == null)
                {
                    return NotFound();
                }

                comment.Likes += 1;

                await _dbComment.UpdateAsync(comment);
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

        [HttpPut("{id:int}/dislike", Name = "DislikeComment")]
        public async Task<ActionResult<APIResponse>> DislikeComment(int id)
        {
            try
            {
                var comment = await _dbComment.GetAsync(u => u.Id == id);
                if (comment == null)
                {
                    return NotFound();
                }

                comment.Dislikes += 1;

                await _dbComment.UpdateAsync(comment);
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
        public async Task<ActionResult<IEnumerable<CommentDTO>>> SearchComments([FromQuery] string? searchText, string? orderBy,int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                Expression<Func<Comment, object>> orderByExpression = null;

                if (string.IsNullOrEmpty(orderBy))
                {
            
                    orderByExpression = p => p.Id;
                }
                else
                {
                    switch (orderBy)
                    {
                        case "Text":
                            orderByExpression = p => p.Text;
                            break;
                        case "Date":
                            orderByExpression = p => p.CreatedDate;
                            break;

                        default:

                            return BadRequest("Invalid orderBy parameter.");
                    }
                }

                IEnumerable<Comment> searchResults = await _dbComment.SearchAsync(searchText, includeProperties:"Item,User",pageSize,pageNumber,orderBy:orderByExpression);
                var searchResultDTOs = _mapper.Map<IEnumerable<CommentDTO>>(searchResults);
                var totalCount = await _dbComment.GetTotalCountAsync();

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