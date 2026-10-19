# Copilot Instructions

## Project Guidelines
- User's preferred terminal shell is powershell.exe and workspace root is C:\Users\Ádám\source\repos\CRM_WEB. Use Blazor context and Visual Studio 2026.
- User prefers manual SQL script files for schema modifications instead of EF Core migrations, following the existing *_Migration.sql file convention in the BiztvillCRM.Data folder with idempotent IF NOT EXISTS checks.
- When debugging production API 500 errors in CRM_WEB, check for invalid JSON stored in path-list columns (e.g. Hitelesitesek.MunkalapPaths/BizonyitvanyPaths). Unescaped backslashes in Windows file paths stored as JSON can throw JsonException. Getters deserializing such JSON should be wrapped in try/catch.

## Mobile Application Guidelines
- When developing the mobile (Android) application from the Blazor web CRM, include only read-only access to logs and authentications (certificates); editing and admin interfaces must remain accessible only via the web.