using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Movies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Models.DTO;
using Movies.Repository.IRepository;

namespace Movies.Controllers
{
    [Route("api/Review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IReviewRepository _dbReview;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository dbReview, IMapper mapper)
        {
            _dbReview = dbReview;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetReviews(int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Review> reviewList = await _dbReview.GetAllAsync(includeProperties:"Item,User",pageSize:pageSize,pageNumber:pageNumber);
                _response.Result = _mapper.Map<List<ReviewDTO>>(reviewList);
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

        [HttpGet("{id:int}", Name = "GetReview")]

        public async Task<ActionResult<APIResponse>> GetReview(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var review = await _dbReview.GetAsync(u => u.Id == id, includeProperties:"Item,User");
                if (review == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<ReviewDTO>(review);
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

        public async Task<ActionResult<APIResponse>> CreateReview([FromBody] ReviewCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                Review review = _mapper.Map<Review>(createDTO);

                await _dbReview.CreateAsync(review);
                await _dbReview.SaveAsync();
                _response.Result = _mapper.Map<ReviewDTO>(review);
                _response.StatusCode = HttpStatusCode.Created;

                _response.IsSuccess = true;
                return CreatedAtRoute("GetReview", new { id = review.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteReview")]
        [Authorize(Roles = "admin,user")]


        public async Task<ActionResult<APIResponse>> DeleteReview(int id)
        {
            try
            {
                var review = await _dbReview.GetAsync(u => u.Id == id);
                if (review == null)
                {
                    return NotFound();
                }

                await _dbReview.DeleteAsync(review);
                await _dbReview.SaveAsync();
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

        [HttpPut("{id:int}", Name = "UpdateReview")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<APIResponse>> UpdateReview(int id, [FromBody] ReviewUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

                Review model = _mapper.Map<Review>(updateDTO);

                await _dbReview.UpdateAsync(model);
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
        
        [HttpPut("{id:int}/like", Name = "LikeReview")]
        public async Task<ActionResult<APIResponse>> LikeReview(int id)
        {
            try
            {
                var review = await _dbReview.GetAsync(u => u.Id == id);
                if (review == null)
                {
                    return NotFound();
                }

                review.Likes += 1;

                await _dbReview.UpdateAsync(review);
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

        [HttpPut("{id:int}/dislike", Name = "DislikeReview")]
        public async Task<ActionResult<APIResponse>> DislikeReview(int id)
        {
            try
            {
                var review = await _dbReview.GetAsync(u => u.Id == id);
                if (review == null)
                {
                    return NotFound();
                }

                review.Dislikes += 1;

                await _dbReview.UpdateAsync(review);
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
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> Reviews([FromQuery] string? searchText, string? orderBy, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                Expression<Func<Review, object>> orderByExpression = null;

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

                IEnumerable<Review> searchResults = await _dbReview.SearchAsync(searchText, includeProperties:"Item,User",pageSize,pageNumber,orderBy:orderByExpression);
                var searchResultDTOs = _mapper.Map<IEnumerable<ReviewDTO>>(searchResults);
                var totalCount = await _dbReview.GetTotalCountAsync();
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