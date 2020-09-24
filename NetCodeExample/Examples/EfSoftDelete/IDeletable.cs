using System;
using System.Collections.Generic;
using System.Text;

namespace NetCodeExample.Examples.EfSoftDelete
{
    interface IDeletable
    {
        public bool IsDeleted { get; set; }
    }
}
