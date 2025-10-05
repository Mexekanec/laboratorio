using Laboratorio.Cods.Circut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Interface
{
    public interface IComponentNamingService
    {
        string GetNextName(ComponentType type);
        void ResetCounter(ComponentType type);
        void ComponentDeleted(ComponentType type, string name);
    }
}
