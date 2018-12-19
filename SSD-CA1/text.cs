using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD_CA1
{
    internal class Text
    {

        internal string value { get; set; }

        internal Text(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
