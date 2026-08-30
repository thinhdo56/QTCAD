using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTCAD.Inv25.Adapter
{
        public sealed class InventorContext
        {
            public Application Application { get; }

            public InventorContext(Application application)
            {
                Application = application ?? throw new ArgumentNullException(nameof(application));
            }

            public Documents Documents => Application.Documents;

            public Document? ActiveDocument => Application.ActiveDocument;
        }
}
