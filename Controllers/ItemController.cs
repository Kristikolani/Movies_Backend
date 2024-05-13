using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Movies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models.DTO;
using Movies.Repository.IRepository;

namespace Movies.Controllers
{
    [Route("api/Item")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IItemRepository _dbItem;
        private readonly IMapper _mapper;
        public ItemController(IItemRepository dbItem, IMapper mapper)
        {
            _dbItem = dbItem;
            _mapper = mapper;
            this._response = new();
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetItems(int pageSize=0, int pageNumber=1)
        {
            try 
            { 
                IEnumerable<Item> itemList = await _dbItem.GetAllAsync(includeProperties:"Reviews,Comments,Reviews.User,Comments.User");
                _response.Result = _mapper.Map<List<ItemDTO>>(itemList);
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
        [HttpGet("{id:int}", Name = "GetItem")]

        public async Task<ActionResult<APIResponse>> GetItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var item = await _dbItem.GetAsync(u => u.Id == id, includeProperties:"Reviews,Comments");
                if (item == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<ItemDTO>(item);
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
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> CreateItem([FromBody] ItemCreateDTO createDTO)
        {
            try 
            {
                if (await _dbItem.GetAsync(u => u.Title.ToLower() == createDTO.Title.ToLower()) != null)
                {
                    ModelState.AddModelError("Errors", "Item already Exists!");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }
                Item item = _mapper.Map<Item>(createDTO);

                await _dbItem.CreateAsync(item);
                await _dbItem.SaveAsync();
                _response.Result = _mapper.Map<ItemDTO>(item);
                _response.StatusCode = HttpStatusCode.Created;

                _response.IsSuccess = true;  
                return CreatedAtRoute("GetItem", new { id = item.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.ToString() };
            }
            return _response;
        }   
        [HttpDelete("{id:int}", Name = "DeleteItem")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteItem(int id)
        {
            try
            {
                var item = await _dbItem.GetAsync(u => u.Id == id);
                if (item == null)
                {
                    return NotFound();
                }
                await _dbItem.DeleteAsync(item);
                await _dbItem.SaveAsync();
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
        [HttpPut("{id:int}", Name = "UpdateItem")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateItem(int id, [FromBody]ItemUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

                Item model = _mapper.Map<Item>(updateDTO);

                await _dbItem.UpdateAsync(model);
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
        public async Task<ActionResult<IEnumerable<ItemDTO>>> SearchItems([FromQuery] string? searchText,
            string? orderBy, int pageNumber = 1,
            int pageSize = 0)
        {
            try
            {
                Expression<Func<Item, object>> orderByExpression = null;

                if (string.IsNullOrEmpty(orderBy))
                {
            
                    orderByExpression = p => p.Id;
                }
                else
                {
                    switch (orderBy)
                    {
                        case "Title":
                            orderByExpression = p => p.Title;
                            break;
                        case "Date":
                            orderByExpression = p => p.CreatedDate;
                            break;

                        default:

                            return BadRequest("Invalid orderBy parameter.");
                    }
                }

                IEnumerable<Item> searchResults = await _dbItem.SearchAsync(searchText, includeProperties:"Reviews,Comments,Reviews.User,Comments.User",pageSize,pageNumber,orderBy:orderByExpression);
                
                
                var searchResultDTOs = _mapper.Map<IEnumerable<ItemDTO>>(searchResults);
                var totalCount = await _dbItem.GetTotalCountAsync();
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