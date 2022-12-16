param appName string
param region string
param env string

output appInsightsName string = 'ai-${appName}-${region}-${env}'
output containerAppEnvironmentName string = 'cae-${appName}-${region}-${env}'
output keyVaultName string = 'kv-${appName}-${region}-${env}'
output logAnalyticsWorkspaceName string = 'la-${appName}-${region}-${env}'
output managedIdentityName string = 'mi-${appName}-${region}-${env}'
output redisCacheName string = 'redis-${appName}-${region}-${env}'
output connectionStringKeySecretName string = 'COSMOS-CONNECTION-STRING'
output cosmosContainerName string = 'altgraph'
output cosmosContainerPartitionKey string = '/pk'
output cosmosDatabaseAccountName string = 'cdb-${appName}-${region}-${env}'
output cosmosSqlDatabaseName string = 'dev'
