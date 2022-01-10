# pgAzureADPass
Virtual file system to provide PostgreSQL password files with just-in-time acquired Azure AD tokens instead of passwords

### Contents

[About](#about)<br/>
[Before you begin](#before-you-begin)<br/>
[Install](#install)<br/>
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
2. Dokan library installed.
3. Minimum libpq.dll version 13.3

<a name=install></a>

## Install

1. Download and install [Dokan](https://github.com/dokan-dev/dokany/wiki/Installation).
2. Download and extract zip package from the [latest release](https://github.com/srdan-bozovic-msft/pgAzureADPass/releases/).

<a name=configure></a>

## Configure

Create .yml file based on template below.

```yaml

volumeLabel: AAD Demo pgPass
driveLetter: P
files: 
    - name: active-directory-uami
      authentication: ActiveDirectoryManagedIdentity
      clientId: <client-id>

    - name: active-directory-sami
      authentication: ActiveDirectoryManagedIdentity

    - name: active-directory-interactive 
      authentication: ActiveDirectoryInteractive
      tenantId: <tenant-id>

    - name: active-directory-integrated
      authentication: ActiveDirectoryIntegrated
      tenantId: <tenant-id>

```
<a name=run></a>

## Run

```powershell

pgAzureADPass config.yml

```

To try it out with pgAdmin 4 use the latest [daily snapshot](https://www.postgresql.org/ftp/pgadmin/pgadmin4/snapshots/).

<a name=disclaimers></a>

## Disclaimers
The app and this guide are provided as-is with no warranties express or implied. Familiarize yourself with the app before using it.

<a name=related-links></a>

## Related Links
<!-- Links to more articles. Remember to delete "en-us" from the link path. -->

For more information, see these articles:

- [Use Azure Active Directory for authenticating with PostgreSQL](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-managed-instance-index)
- [Connect with Managed Identity to Azure Database for PostgreSQL](https://docs.microsoft.com/en-us/azure/postgresql/howto-connect-with-managed-identity)
- [Dokany-User mode file system library](https://github.com/dokan-dev/dokany/)
