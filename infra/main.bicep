param appName string
param region string
param environment string
param location string = resourceGroup().location

module names 'resource-names.bicep' = {
  name: 'resource-names'
  params: {
    appName: appName
    region: region
    env: environment
  }
}

module loggingDeployment 'logging.bicep' = {
  name: 'logging-deployment'
  params: {
    appInsightsName: names.outputs.appInsightsName
    logAnalyticsWorkspaceName: names.outputs.logAnalyticsWorkspaceName
    location: location
  }
}

module managedIdentityDeployment 'managed-identity.bicep' = {
  name: 'managed-identity-deployment'
  params: {
    location: location
    managedIdentityName: names.outputs.managedIdentityName
  }
}

module keyVaultDeployment 'key-vault.bicep' = {
  name: 'key-vault-deployment'
  params: {
    keyVaultName: names.outputs.keyVaultName
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    location: location
    managedIdentityName: managedIdentityDeployment.outputs.managedIdentityName
  }
}

module cosmosDeployment 'cosmos.bicep' = {
  name: 'cosmos-deployment'
  params: {
    connectionStringKeySecretName: names.outputs.connectionStringKeySecretName
    cosmosContainerName: names.outputs.cosmosContainerName
    cosmosContainerPartitionKey: names.outputs.cosmosContainerPartitionKey
    cosmosDatabaseAccountName: names.outputs.cosmosDatabaseAccountName
    cosmosSqlDatabaseName: names.outputs.cosmosSqlDatabaseName
    keyVaultName: keyVaultDeployment.outputs.keyVaultName
    location: location
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    managedIdentityName: managedIdentityDeployment.outputs.managedIdentityName
  }
}

module acaEnvironmentDeployment 'managed-environment.bicep' = {
  name: 'aca-environment-deployment'
  params: {
    containerAppEnvironmentName: names.outputs.containerAppEnvironmentName
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    location: location
    appInsightsName: loggingDeployment.outputs.appInsightsName
    managedIdentityName: managedIdentityDeployment.outputs.managedIdentityName
    keyVaultName: keyVaultDeployment.outputs.keyVaultName
    redisCacheName: redisCacheDeployment.outputs.redisCacheName
  }
}

module acaDeployment 'container-app.bicep' = {
  name: 'aca-deployment'
  params: {
    containerAppEnvironmentName: names.outputs.containerAppEnvironmentName
    location: location
    managedIdentityName: managedIdentityDeployment.outputs.managedIdentityName
  }
}

module redisCacheDeployment 'redis-cache.bicep' = {
  name: 'redis-cache-deployment'
  params: {
    redisCacheName: names.outputs.redisCacheName
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    location: location
  }
}

output appInsightsInstrumentationKey string = loggingDeployment.outputs.appInsightsInstrumentationKey
output appInsightsName string = loggingDeployment.outputs.appInsightsName
output keyVaultName string = keyVaultDeployment.outputs.keyVaultName
output keyVaultResourceId string = keyVaultDeployment.outputs.keyVaultResourceId
output redisCacheName string = redisCacheDeployment.outputs.redisCacheName
output resourceGroupName string = resourceGroup().name
output subscriptionId string = subscription().subscriptionId
output userAssignedManagedIdentityClientId string = managedIdentityDeployment.outputs.userAssignedManagedIdentityClientId
