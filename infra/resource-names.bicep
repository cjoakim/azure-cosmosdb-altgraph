param resourceToken string

output appInsightsName string = 'ai-${resourceToken}'
output containerAppEnvironmentName string = 'cae-${resourceToken}'
output keyVaultName string = 'kv-${resourceToken}'
output logAnalyticsWorkspaceName string = 'la-${resourceToken}'
output managedIdentityName string = 'mi-${resourceToken}'
output redisCacheName string = 'redis-${resourceToken}'
output redisCacheConnectionStringKeySecretName string = 'REDIS-CONNECTION-STRING'
output cosmosDatabaseAccountConnectionStringKeySecretName string = 'COSMOS-CONNECTION-STRING'
output cosmosContainerName string = 'altgraph'
output cosmosContainerPartitionKey string = '/pk'
output cosmosDatabaseAccountName string = 'cdb-${resourceToken}'
output cosmosSqlDatabaseName string = 'dev'
