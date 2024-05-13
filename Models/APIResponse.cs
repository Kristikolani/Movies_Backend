using System.Net;

namespace Movies.Models
{
    public class APIResponse
    {
        public APIResponse() 
        {
            Errors = new List<string>();
        } 
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess {  get; set; }
        public List<string> Errors { get; set; }
        public object Result {  get; set; }   
        public int count { get; set; }
    }
}