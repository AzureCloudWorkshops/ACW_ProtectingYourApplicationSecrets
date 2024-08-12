param location string = resourceGroup().location
@description('Provide a unique datetime and initials string to make your instances unique. Use only lower case letters and numbers')
@minLength(11)
@maxLength(11)
param uniqueIdentifier string

param appConfigName string = 'AC-ProtectingYourSecrets'

var configStoreName = '${appConfigName}-${uniqueIdentifier}'

resource configStore 'Microsoft.AppConfiguration/configurationStores@2021-10-01-preview' = {
  name: configStoreName
  location: location
  sku: {
    name: 'standard'
  }
}

output appConfigName string = configStore.name
