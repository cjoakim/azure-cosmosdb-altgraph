targetScope = 'subscription'

param environmentName string
param location string
#disable-next-line no-unused-params
param principalId string = ''
param containerAppWebAppObject object = {}
param containerAppDataAppObject object = {}
param resourceGroupName string = ''
param resourceTokenParam string = ''

var resourceToken = resourceTokenParam == '' ? toLower(uniqueString(subscription().id, environmentName, location)) : resourceTokenParam

resource resourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : 'rg-${resourceToken}'
  location: location
}

module names 'resource-names.bicep' = {
  name: 'resource-names'
  scope: resourceGroup
  params: {
    resourceToken: resourceToken
  }
}

module loggingDeployment 'logging.bicep' = {
  name: 'logging-deployment'
  scope: resourceGroup
  params: {
    appInsightsName: names.outputs.appInsightsName
    logAnalyticsWorkspaceName: names.outputs.logAnalyticsWorkspaceName
    location: location
  }
}

module managedIdentityDeployment 'managed-identity.bicep' = {
  name: 'managed-identity-deployment'
  scope: resourceGroup
  params: {
    location: location
    managedIdentityName: names.outputs.managedIdentityName
  }
}

module keyVaultDeployment 'key-vault.bicep' = {
  name: 'key-vault-deployment'
  scope: resourceGroup
  params: {
    keyVaultName: names.outputs.keyVaultName
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    location: location
    managedIdentityName: managedIdentityDeployment.outputs.managedIdentityName
  }
}

module cosmosDeployment 'cosmos.bicep' = {
  name: 'cosmos-deployment'
  scope: resourceGroup
  params: {
    cosmosDatabaseAccountConnectionStringKeySecretName: names.outputs.cosmosDatabaseAccountConnectionStringKeySecretName
    cosmosContainerName: names.outputs.cosmosContainerName
    cosmosContainerPartitionKey: names.outputs.cosmosContainerPartitionKey
    cosmosDatabaseAccountName: names.outputs.cosmosDatabaseAccountName
    cosmosSqlDatabaseName: names.outputs.cosmosSqlDatabaseName
    keyVaultName: keyVaultDeployment.outputs.keyVaultName
    location: location
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    //managedIdentityName: managedIdentityDeployment.outputs.managedIdentityName
  }
}

module redisCacheDeployment 'redis-cache.bicep' = {
  name: 'redis-cache-deployment'
  scope: resourceGroup
  params: {
    redisCacheName: names.outputs.redisCacheName
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    location: location
    keyVaultName: keyVaultDeployment.outputs.keyVaultName
    redisCacheConnectionStringKeySecretName: names.outputs.redisCacheConnectionStringKeySecretName
  }
}

module acaEnvironmentDeployment 'managed-environment.bicep' = {
  name: 'aca-environment-deployment'
  scope: resourceGroup
  params: {
    containerAppEnvironmentName: names.outputs.containerAppEnvironmentName
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
    location: location
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultDeployment.outputs.keyVaultName
  scope: resourceGroup
}

module acaDeployment 'container-app.bicep' = {
  name: 'aca-deployment'
  scope: resourceGroup
  params: {
    containerAppEnvironmentName: names.outputs.containerAppEnvironmentName
    location: location
    managedIdentityName: managedIdentityDeployment.outputs.managedIdentityName
    containerAppWebAppObject: containerAppWebAppObject
    containerAppDataAppObject: containerAppDataAppObject
    cosmosDatabaseAccountConnectionStringKeySecret: keyVault.getSecret(cosmosDeployment.outputs.cosmosDatabaseAccountConnectionStringKeySecretName)
    redisCacheConnectionStringKeySecret: keyVault.getSecret(redisCacheDeployment.outputs.redisCacheConnectionStringKeySecretName)
    logAnalyticsWorkspaceName: loggingDeployment.outputs.logAnalyticsWorkspaceName
  }
}

// output appInsightsInstrumentationKey string = loggingDeployment.outputs.appInsightsInstrumentationKey
// output appInsightsName string = loggingDeployment.outputs.appInsightsName
// output keyVaultName string = keyVaultDeployment.outputs.keyVaultName
// output keyVaultResourceId string = keyVaultDeployment.outputs.keyVaultResourceId
// output redisCacheName string = redisCacheDeployment.outputs.redisCacheName
// output resourceGroupName string = resourceGroup.name
// output subscriptionId string = subscription().subscriptionId
// output userAssignedManagedIdentityClientId string = managedIdentityDeployment.outputs.userAssignedManagedIdentityClientId
