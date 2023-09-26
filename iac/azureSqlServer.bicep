param location string = 'eastus'
@description('Provide a unique datetime and initials string to make your instances unique. Use only lower case letters and numbers')
@minLength(11)
@maxLength(11)
param uniqueIdentifier string = '20251231acw'

@description('Name of the SQL Db Server')
param serverName string = 'ProtectYourSecretsDBServer'

@description('Name of the Sql Database')
param sqlDatabaseName string = 'ProtectYourSecretsDB'

@description('Admin UserName for the SQL Server')
param sqlServerAdminLogin string = 'secretweb_user'

@description('Admin Password for the SQL Server')
@secure()
param sqlServerAdminPassword string = 'Azure#54321!'

var dbSKU = 'Basic'
var dbCapacity = 5
var sqlDBServerName = '${serverName}${uniqueIdentifier}'

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlDBServerName
  location: location
  properties: {
    administratorLogin: sqlServerAdminLogin
    administratorLoginPassword: sqlServerAdminPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: {
    name: dbSKU
    capacity: dbCapacity
  }
  properties: {
    requestedBackupStorageRedundancy: 'local'
  }
}
