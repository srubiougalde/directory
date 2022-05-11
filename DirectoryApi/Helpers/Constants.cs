namespace DirectoryApi.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Roles = "roles", Id = "id";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
                public const string SuperAdministrator = "super_admin";
                public const string Administrator = "admin";
                public const string Client = "client";
            }
        }
    }
}