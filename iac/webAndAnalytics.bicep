param location string = resourceGroup().location
@description('Provide a unique datetime and initials string to make your instances unique. Use only lower case letters and numbers')
@minLength(11)
@maxLength(11)
param uniqueIdentifier string = '20251231acw'

param logAnalyticsWorkspaceName string = 'LA-ProtectingYourSecrets'
param appInsightsName string = 'AI-ProtectingYourSecrets'
param appServicePlanName string = 'ASP-ProtectingYourSecrets'
@minLength(24)
@maxLength(31)
param appServiceName string = 'ProtectingYourSecretsWeb'

param storageAccountName string
param imagesContainerName string = 'images'

var webAppHostingPlan = 'F1'
var webAppName = '${appServiceName}-${uniqueIdentifier}'

resource hostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: webAppHostingPlan
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      searchVersion: 1
      legacy: 0
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      netFrameworkVersion:'v8.0'
      appSettings: [
          {
            name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
            value: applicationInsights.properties.InstrumentationKey
          }
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: applicationInsights.properties.ConnectionString
          }
          {
            name: 'StorageDetails:ImagesAccountName'
            value: storageAccountName
          }
          {
            name: 'StorageDetails:ImagesContainerName'
            value: imagesContainerName
          }
          {
            name: 'StorageDetails:ImagesSASToken'
            value: 'your-sas-token-here'
          }
        ]
        connectionStrings: [
          {
            name: 'DefaultConnection'
            type: 'SQLAzure'
            connectionString: 'your-db-connection-string-here'
          }
          {
            name: 'AzureAppConfigConnection'
            type: 'Custom'
            connectionString: 'your-app-config-connection-string-here'
          }
        ]
        ftpsState: 'FtpsOnly'
        minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}
