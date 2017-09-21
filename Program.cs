using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ad_connect
{
    class Program
    {
        static void Main(string[] args)
        {
            var domain = args[0];
            var username = args[1];
            var password = args[2];
            var delegatedDomain = args[3];
            var group = args[4];

            GroupMemberShip.IsMemberOf(delegatedDomain, domain, username, password, group);
        }
    }
}
