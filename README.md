# pgAzureADPass
Virtual file system to provide virtualized PostgreSQL password file with embedded Azure AD tokens instead of passwords

### Contents

[About](#about)<br/>
[Before you begin](#before-you-begin)<br/>
[Configure](#configure)<br/>
[Run](#run)<br/>
[Disclaimers](#disclaimers)<br/>
[Related links](#related-links)<br/>

<a name=about-this-sample></a>

## About

This application provides virtual mounted drive that is hosting virtualized [PostgreSQL password files](https://www.postgresql.org/docs/9.3/libpq-pgpass.html). 

Password files are created on the fly and contain Azure AD JWT token as a password.

Password files are _only_ available to user that runs the app so the token could not be compromised.

Currently available Azure AD token acquisition flows:
- ActiveDirectoryManagedIdentity
- ActiveDirectoryInteractive
- ActiveDirectoryIntegrated

<a name=before-you-begin></a>

## Before you begin

To run this app, you need the following prerequisites.

**Prerequisites:**

1. Microsoft .NET Framework 4.0
2. Dokan library installed. Download [here](https://github.com/dokan-dev/dokany/releases/tag/v1.5.0.3000).

<a name=configure></a>

## Configure

```yaml

volumeLabel: AAD Demo pgPass
driveLetter: P
files: 
    - name: active-directory-uami
      hostName: <server>.postgres.database.azure.com
      authentication: ActiveDirectoryManagedIdentity
      database: postgres
      userName: <user>@<server>
      clientId: <client-id>

    - name: active-directory-sami
      hostName: <server>.postgres.database.azure.com
      authentication: ActiveDirectoryManagedIdentity
      database: postgres
      userName: <user>@<server>

    - name: active-directory-interactive 
      hostName: <server>.postgres.database.azure.com
      authentication: ActiveDirectoryInteractive
      database: postgres
      tenantId: <tenant-id>

    - name: active-directory-integrated
      hostName: <server>.postgres.database.azure.com
      authentication: ActiveDirectoryIntegrated
      database: postgres
      tenantId: <tenant-id>

```
<a name=run></a>

## Run

```powershell

pgAzureADPass config.yml

```

<a name=disclaimers></a>

## Disclaimers
The scripts and this guide are copyright Microsoft Corporations and are provided as samples. They are not part of any Azure service and are not covered by any SLA or other Azure-related agreements. They are provided as-is with no warranties express or implied. Microsoft takes no responsibility for the use of the scripts or the accuracy of this document. Familiarize yourself with the scripts before using them.

<a name=related-links></a>

## Related Links
<!-- Links to more articles. Remember to delete "en-us" from the link path. -->

For more information, see these articles:

- [Use Azure Active Directory for authenticating with PostgreSQL](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-managed-instance-index)
- [Connect with Managed Identity to Azure Database for PostgreSQL](https://docs.microsoft.com/en-us/azure/postgresql/howto-connect-with-managed-identity)
- [Dokany-User mode file system library](https://github.com/dokan-dev/dokany/)
