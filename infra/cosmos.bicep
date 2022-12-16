param connectionStringKeySecretName string
param cosmosContainerName string
param cosmosContainerPartitionKey string
param cosmosDatabaseAccountName string
param cosmosSqlDatabaseName string
param keyVaultName string
param location string
param logAnalyticsWorkspaceName string
param managedIdentityName string

resource cosmosDatabaseAccount 'Microsoft.DocumentDB/databaseAccounts@2022-08-15' = {
  name: cosmosDatabaseAccountName
  kind: 'GlobalDocumentDB'
  location: location
  properties: {
    consistencyPolicy: { defaultConsistencyLevel: 'Session' }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    apiProperties: {}
    capabilities: [ { name: 'EnableServerless' } ]
  }

  resource database 'sqlDatabases@2022-05-15' = {
    name: cosmosSqlDatabaseName
    properties: {
      resource: { id: cosmosSqlDatabaseName }
    }
    resource container 'containers' = {
      name: cosmosContainerName
      properties: {
        resource: {
          id: cosmosContainerName
          partitionKey: { paths: [ cosmosContainerPartitionKey ] }
        }
        options: {}
      }
    }

  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource cosmosConnectionString 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: connectionStringKeySecretName
  properties: {
    value: cosmosDatabaseAccount.listConnectionStrings().connectionStrings[0].connectionString
  }
}
