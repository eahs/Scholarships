using System.Threading.Tasks;

namespace Scholarships.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model, string controller = null);
    }
}
