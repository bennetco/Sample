using Sample.Domain.Abstract;
using System.Threading.Tasks;

namespace Sample.Infrastructure
{
    public class HelloWorldRepo : IHelloWorldRepo
    {
        public async Task<string> GetMessageAsync()
        {
            return await Task.Run(() => "hello world");
        }
    }
}
