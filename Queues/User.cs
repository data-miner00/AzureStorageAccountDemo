using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queues
{
    internal class User
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string[] Emails { get; set; } = Array.Empty<string>();
    }
}
