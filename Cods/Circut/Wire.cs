using Laboratorio.Cods.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio.Cods.Circut
{
    public class Wire
    {
        public SchematicPin StartPin { get; set; }
        public SchematicPin EndPin { get; set; }
        // Визуальные свойства: координаты для рисования
    }
}
