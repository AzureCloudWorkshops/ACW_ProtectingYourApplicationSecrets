param location string = resourceGroup().location
@description('Provide a unique datetime and initials string to make your instances unique. Use only lower case letters and numbers')
@minLength(11)
@maxLength(11)
param uniqueIdentifier string = '20251231acw'

@minLength(11)
@maxLength(11)
param storageAccountName string = 'protsecstor'
param imagesContainerName string = 'images'

var storageAccountNameFormatted = substring('${storageAccountName}${uniqueIdentifier}${uniqueString(resourceGroup().id)}', 0, 24) 
var storageSku = 'Standard_LRS'
var storageTier = 'Hot'
var allowSharedKeyAccess = true
var allowBlobPublicAccess = false
var blobEncryptionEnabled = true
var enableBlobRetention = false
var blobRetentionDays = 1

@description('The storage account.  Toggle the public access to true if you want public blobs on the account in any containers')
resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageAccountNameFormatted
  location: location
  kind: 'StorageV2'
  sku: {
    name: storageSku
  }
  properties: {
    allowBlobPublicAccess: allowBlobPublicAccess
    accessTier: storageTier
    allowSharedKeyAccess: allowSharedKeyAccess
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: blobEncryptionEnabled
        }
      }
    }
  }
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2021-04-01' = {
  parent: storageaccount
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: enableBlobRetention
      days: blobRetentionDays
    }
  }
}

// Create the images container
resource images 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: imagesContainerName
  parent: blobServices
  properties: {
    immutableStorageWithVersioning: {
      enabled: false
    }
    metadata: {}
    publicAccess: allowBlobPublicAccess ? 'Blob' : 'None'
  }
}
