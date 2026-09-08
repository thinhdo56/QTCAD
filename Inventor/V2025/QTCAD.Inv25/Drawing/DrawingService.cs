using QTCAD.Inv25.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Drawing
{
    internal sealed class DrawingService
    {
        private readonly InventorContext _context;

        public DrawingService(InventorContext context)
        {
            _context = context;
        }
    }
}
