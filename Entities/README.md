# EF Core Migration Commands

check if any model changes have been made since the last migration was added
`dotnet ef migrations has-pending-model-changes`

create migration after making changes to the model
`dotnet ef migrations add <MIGRATION_NAME>`

updating or creating the database after a migration has been added
`dotnet ef database update`

# References
- [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
- [Applying Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli)