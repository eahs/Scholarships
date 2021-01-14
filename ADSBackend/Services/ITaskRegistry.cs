using System.Threading.Tasks;

namespace Scholarships.Services
{
    public interface ITaskRegistry
    {
        void Register(Task task);
    }
}
