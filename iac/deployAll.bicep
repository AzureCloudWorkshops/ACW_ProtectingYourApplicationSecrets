@description('Name of the Resource Group')
param groupName string = 'RG-ProtectingYourSecrets'

@description('Location for deployment of the resources')
param location string = 'centralus'

@description('Provide a unique datetime and initials string in the format YYYYMMDDabc to make your instances unique. Use only lower case letters and numbers')
@minLength(11)
@maxLength(11)
param uniqueIdentifier string

/* web and analytics parameters*/
@description('Name of the Log Analytics Workspace')
param logAnalyticsWorkspaceName string = 'LA-ProtectingYourSecrets'
@description('Name of the App Insights instance')
param appInsightsName string = 'AI-ProtectingYourSecrets'
@description('Name of the App Service Plan')
param appServicePlanName string = 'ASP-ProtectingYourSecrets'
@description('Name of the Web App (before adding unique identifier)')
param appServiceName string = 'ProtectingYourSecretsWeb'

/* storage account parameters */
@description('Name of the Storage account (before adding the unique identifier)')
@minLength(11)
@maxLength(11)
param storageAccountName string = 'protsecstor'
@description('Name of the images container')
param imagesContainerName string = 'images'

/* keyvault parameters */
@description('Name of the Key Vault')
@minLength(10)
@maxLength(13)
param keyVaultName string = 'KV-ProtSec'

/* app configuration parameters */
@description('Name of the App configuration')
param appConfigName string = 'AC-ProtectingYourSecrets'

/* SQL Server parameters */
@description('Name of the SQL Db Server')
param serverName string = 'ProtectYourSecretsDBServer'

@description('Name of the Sql Database')
param sqlDatabaseName string = 'ProtectYourSecretsDB'

@description('Admin UserName for the SQL Server')
param sqlServerAdminLogin string = 'secretweb_user'

@description('Admin Password for the SQL Server')
@secure()
param sqlServerAdminPassword string

@description('Client IP Address for allow remote server connections')
param clientIPAddress string

targetScope = 'subscription'

/* create resource group */
resource group 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: groupName
  location: location
}

/* create web and analytics */
module webAndAnalytics 'webAndAnalytics.bicep' = {
  name: 'webAndAnalytics'
  scope: group
  params: {
    location: location
    uniqueIdentifier: uniqueIdentifier
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    appInsightsName: appInsightsName
    appServicePlanName: appServicePlanName
    appServiceName: appServiceName
  }
}

/* create storage account */
module storageAccount 'storage.bicep' = {
  name: 'storageAccount'
  scope: group
  params: {
    location: location
    uniqueIdentifier: uniqueIdentifier
    storageAccountName: storageAccountName
    imagesContainerName: imagesContainerName
  }
}

/* create key vault */
module vault 'keyVault.bicep' = {
  name: 'keyVault'
  scope: group
  params: {
    location: location
    uniqueIdentifier: uniqueIdentifier
    keyVaultName: keyVaultName
  }
}

/* create app configuration */
module appConfig 'appConfiguration.bicep' = {
  name: 'appConfig'
  scope: group
  params: {
    location: location
    uniqueIdentifier: uniqueIdentifier
    appConfigName: appConfigName
  }
}

module database 'azureSqlServer.bicep' = {
  name: 'database'
  scope: group
  params: {
    location: location
    uniqueIdentifier: uniqueIdentifier
    serverName: serverName
    sqlDatabaseName: sqlDatabaseName
    sqlServerAdminLogin: sqlServerAdminLogin
    sqlServerAdminPassword: sqlServerAdminPassword
    clientIPAddress: clientIPAddress
  }
}
