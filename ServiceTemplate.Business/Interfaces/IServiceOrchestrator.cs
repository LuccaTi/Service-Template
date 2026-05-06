using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTemplate.Business.Interfaces
{
    public interface IServiceOrchestrator
    {
        public Task EventHandlerAsync(CancellationToken cancellationToken);
    }
}
