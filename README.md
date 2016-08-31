# ActiveDirectoryUser

Convenient way to interface with `System.DirectoryServices.AccountManagement.UserPrincipal` to obtain information about the current user of the application.

Currently, the scope is limited to a single domain, but could be expanded in the future. Also, the exact pattern is not set in stone. Right now it is basically a wrapper for UserPrincipal that encapsulates the initialization and use of the configured PrincipalContext.


Primary Use Case
* Extract Name, EmpId, and/or Email from the current user of an intranet web app.

Other Uses
* Authenticate users against desired roles
* Find users by first/last name using a pattern search (not currently implemented)

Simple Role Authentication will more likely be handled by an `Authorize` attribute in the Controllers or the `IPrincipal.IsInRole("RoleName/ADGroup")` method in Views, but using RoleGroups encapsulates the ADGroup list so that only a role name needs to be referenced.
