using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Interface
{
    public interface ICommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }
}
