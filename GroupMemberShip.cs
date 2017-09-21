using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ad_connect
{
    internal class GroupMemberShip
    {
        private static string ExtractDistinguishedNameFromLDAPResponse(string p)
        {
            var equalsIndex = p.IndexOf("=", 1);
            var commaIndex = p.IndexOf(",", 1);

            if (equalsIndex == -1)
            {
                throw new NotSupportedException(p + " is invalid");
            }

            return p.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1);
        }

        private static bool MatchingUserInformation(string inputUserInformation, string userNameOrSid)
        {
            return string.Equals(inputUserInformation, userNameOrSid, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsMemberOf(string delegatedDomain, string userdomain, string username, string password, string group)
        {
            const string STR_NAME = "IsMemberOf";

            bool found = false;

            if (string.IsNullOrWhiteSpace(group)) return false;

            Console.WriteLine("Checking that User '" + username + "' is member of Group '" + group + "'...");

            try
            {
                string domainAndUsername = userdomain + @"\" + username;
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + delegatedDomain, domainAndUsername, password);

                PrincipalContext context = new PrincipalContext(ContextType.Domain, userdomain);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, username);

                var sid = user.Sid;
                var displayName = user.DisplayName;
                var userName = user.Name;

                Console.WriteLine($"User Principal properties: sid={sid} displayName={displayName} userName={userName}");

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(CN=" + group + ")";

                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["member"].Count;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    try
                    {
                        var dn = (string)result.Properties["member"][propertyCounter];
                        var userNameOrSid = ExtractDistinguishedNameFromLDAPResponse(dn);

                        var userMatches = MatchingUserInformation(sid.ToString(), userNameOrSid) ||
                                          MatchingUserInformation(displayName, userNameOrSid) ||
                                          MatchingUserInformation(userName, userNameOrSid);

                        if (userMatches)
                        {
                            found = true;
                            Console.WriteLine("User is a valid member of group " + group);
                            break;
                        }
                    }
                    catch (NotSupportedException ex)
                    {
                        Console.WriteLine($"ERROR: NotsupportedException {ex.Message}");
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                if (!found)
                    Console.WriteLine("User cannot be found in Group.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Generic Exception {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            return found;
        }
    }
}
