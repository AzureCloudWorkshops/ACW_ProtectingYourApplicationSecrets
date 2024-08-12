param vaults_KV_ProtSec_20241231acw_name string = 'KV-ProtSec-20241231acw'

resource vaults_KV_ProtSec_20241231acw_name_resource 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: vaults_KV_ProtSec_20241231acw_name
  location: 'centralus'
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: '5c8384b7-fa0d-4169-ac32-02f8c35bc0d4'
    accessPolicies: [
      {
        tenantId: '5c8384b7-fa0d-4169-ac32-02f8c35bc0d4'
        objectId: 'd361fae0-9103-4421-a09e-25c021f7a8b5'
        permissions: {
          certificates: [
            'Get'
            'List'
            'Update'
            'Create'
            'Import'
            'Delete'
            'Recover'
            'Backup'
            'Restore'
            'ManageContacts'
            'ManageIssuers'
            'GetIssuers'
            'ListIssuers'
            'SetIssuers'
            'DeleteIssuers'
            'Purge'
          ]
          keys: [
            'Get'
            'List'
            'Update'
            'Create'
            'Import'
            'Delete'
            'Recover'
            'Backup'
            'Restore'
            'GetRotationPolicy'
            'SetRotationPolicy'
            'Rotate'
            'Encrypt'
            'Decrypt'
            'UnwrapKey'
            'WrapKey'
            'Verify'
            'Sign'
            'Purge'
            'Release'
          ]
          secrets: [
            'Get'
            'List'
            'Set'
            'Delete'
            'Recover'
            'Backup'
            'Restore'
            'Purge'
          ]
        }
      }
    ]
    enabledForDeployment: true
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    vaultUri: 'https://kv-protsec-20241231acw.vault.azure.net/'
    provisioningState: 'Succeeded'
    publicNetworkAccess: 'Enabled'
  }
}

resource vaults_KV_ProtSec_20241231acw_name_ProtSecAppConfigConnection 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: vaults_KV_ProtSec_20241231acw_name_resource
  name: 'ProtSecAppConfigConnection'
  location: 'centralus'
  properties: {
    contentType: 'string'
    attributes: {
      enabled: true
    }
  }
}

resource vaults_KV_ProtSec_20241231acw_name_ProtSecDbConnectionString 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: vaults_KV_ProtSec_20241231acw_name_resource
  name: 'ProtSecDbConnectionString'
  location: 'centralus'
  properties: {
    contentType: 'string'
    attributes: {
      enabled: true
    }
  }
}

resource vaults_KV_ProtSec_20241231acw_name_ProtSecStorageSASToken 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: vaults_KV_ProtSec_20241231acw_name_resource
  name: 'ProtSecStorageSASToken'
  location: 'centralus'
  properties: {
    contentType: 'string'
    attributes: {
      enabled: true
    }
  }
}
