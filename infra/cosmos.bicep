param cosmosDatabaseAccountConnectionStringKeySecretName string
param cosmosNpmContainerName string
param cosmosContainerPartitionKey string
param cosmosDatabaseAccountName string
param cosmosSqlDatabaseName string
param keyVaultName string
param location string
param logAnalyticsWorkspaceName string
//param managedIdentityName string

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
      name: cosmosNpmContainerName
      properties: {
        resource: {
          id: cosmosNpmContainerName
          partitionKey: { paths: [ cosmosContainerPartitionKey ] }
        }
        options: {}
      }
    }
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logAnalyticsWorkspaceName
}

resource diagnosticSettings 'Microsoft.Insights/diagnosticsettings@2017-05-01-preview' = {
  name: 'Logging'
  scope: cosmosDatabaseAccount
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'DataPlaneRequests'
        enabled: true
      }
      {
        category: 'MongoRequests'
        enabled: true
      }
      {
        category: 'QueryRuntimeStatistics'
        enabled: true
      }
      {
        category: 'PartitionKeyStatistics'
        enabled: true
      }
      {
        category: 'PartitionKeyRUConsumption'
        enabled: true
      }
      {
        category: 'ControlPlaneRequests'
        enabled: true
      }
      {
        category: 'CassandraRequests'
        enabled: true
      }
      {
        category: 'GremlinRequests'
        enabled: true
      }
      {
        category: 'TableApiRequests'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'Requests'
        enabled: true
      }
    ]
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource cosmosConnectionString 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: cosmosDatabaseAccountConnectionStringKeySecretName
  properties: {
    value: cosmosDatabaseAccount.listConnectionStrings().connectionStrings[0].connectionString
  }
}

output cosmosDatabaseAccountName string = cosmosDatabaseAccountName
output cosmosSqlDatabaseName string = cosmosSqlDatabaseName
output cosmosNpmContainerName string = cosmosNpmContainerName
output cosmosDatabaseAccountConnectionStringKeySecretName string = cosmosDatabaseAccountConnectionStringKeySecretName
