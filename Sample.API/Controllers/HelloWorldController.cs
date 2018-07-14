using Microsoft.AspNetCore.Mvc;
using Sample.Domain.Abstract;
using Sample.Domain.Entities;
using System.Threading.Tasks;

namespace Sample.API.Controllers
{
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        private readonly IHelloWorldRepo _repo;

        public HelloWorldController(IHelloWorldRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("/api/GetHelloWorldInfo")]
        public async Task<HelloWorldViewModel> GetHelloWorldInfoAsync()
        {
            return new HelloWorldViewModel {
                Message = await _repo.GetMessageAsync()
            };
        }
    }
}