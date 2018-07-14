using System.Threading.Tasks;

namespace Sample.Domain.Abstract
{
    public interface IHelloWorldRepo
    {
        Task<string> GetMessageAsync();
    }
}
