using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Services
{
    public interface ITaskRegistry
    {
        void Register(Task task);
    }
}
