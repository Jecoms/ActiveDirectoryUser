using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace ActiveDirectoryUserExample
{
	public class ActiveDirectoryUser
	{
		public string FirstName => _userPrin.GivenName;
		public string LastName => _userPrin.Surname;
		public string FullName => $"{FirstName} {LastName}";
		public string LastFirstName => $"{LastName}, {FirstName}";
		public string Email => _userPrin.EmailAddress;
		public string EmpID => _userPrin.EmployeeId;

		private UserPrincipal _userPrin;

		private static PrincipalContext CONTEXT_DEFAULT = new PrincipalContext(ContextType.Domain, "DomainName", "DC=DomainName,DC=com");

		// This dictionary might be something to manage in a config file or load from a db at startup
		private static Dictionary<string, List<string>> RoleGroups = new Dictionary<string, List<string>>() { { "Admin", new List<string>() { "DOMAINADMIN" } } };

		public ActiveDirectoryUser() { }

		public ActiveDirectoryUser(string username)
		{
			_userPrin = UserPrincipal.FindByIdentity(CONTEXT_DEFAULT, username);
		}

		public bool CheckIfInRole(string role)
		{
			var roleADGroups = GetRoleADGroups(role);
			
			// This could be expanded to throw an exception or return a custom result that communicates invalid/incomplete role data

			foreach (var group in roleADGroups)
			{
				if (_userPrin.IsMemberOf(CONTEXT_DEFAULT, IdentityType.Name, group)) return true;
			}
			return false;
		}

		private List<string> GetRoleADGroups(string role)
		{
			List<string> roleADGroups; // To be removed once C#7.0 is out
			if (!RoleGroups.TryGetValue(role, out roleADGroups))
				roleADGroups = new List<string>();

			return roleADGroups;
		}

		public static bool CheckIfUserInRole(string username, string role)
			=> new ActiveDirectoryUser(username).CheckIfInRole(role);		

		public static bool CheckIfUserMemberOfADGroup(string username, string domainADGroup) 
			=> UserPrincipal.FindByIdentity(CONTEXT_DEFAULT, username).IsMemberOf(CONTEXT_DEFAULT, IdentityType.Name, domainADGroup);

		public static bool AddRoleGroup(string usernameSetBy, string role, string groupToAdd)
		{
			if (!string.IsNullOrWhiteSpace(role))
			{
				if (CheckIfUserInRole(usernameSetBy, "Admin"))
				{
					List<string> currentADGroups;
					if (!RoleGroups.TryGetValue(role, out currentADGroups))
					{
						RoleGroups.Add(role, new List<string>() { groupToAdd });
					}
					else
					{
						currentADGroups.Add(groupToAdd);
						RoleGroups[role] = currentADGroups;
					}
						

					return true;
				}
			}

			return false; // Could also throw exception or custom class result
		}

		public static bool SetRoleGroups(string usernameSetBy, string role, List<string> updatedADGroups)
		{
			if (!string.IsNullOrWhiteSpace(role))
			{
				if (CheckIfUserInRole(usernameSetBy, "Admin"))
				{
					List<string> currentADGroups;
					if (!RoleGroups.TryGetValue(role, out currentADGroups))
						RoleGroups.Add(role, updatedADGroups);
					else
						RoleGroups[role] = updatedADGroups;
					return true;
				}
			}

			return false; // Could also throw exception or custom class result
		}

		// To be refactored
		// Ideally, this will provide a way to set the context without hardcoding
		// Alternatively, a using block could be used for the context
		public static void SetDefaultContext(PrincipalContext newContext)
		{
			CONTEXT_DEFAULT = newContext;
		}
	}
}