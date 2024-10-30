using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbData
{
    internal class Country
    {

        public string Name { get; }
        public string Code { get; }

        public Country(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
