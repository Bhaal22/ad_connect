namespace ad_connect
{
    class Program
    {
        static void Main(string[] args)
        {
            var delegatedDomain = args[0];
            var domain = args[1];
            var username = args[2];
            var password = args[3];
            var group = args[4];

            GroupMemberShip.IsMemberOf(delegatedDomain, domain, username, password, group);
        }
    }
}
