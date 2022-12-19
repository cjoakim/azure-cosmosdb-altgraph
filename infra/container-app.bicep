param containerAppObject object
param location string
param containerAppEnvironmentName string
param managedIdentityName string
param logAnalyticsWorkspaceName string
@secure()
param cosmosDatabaseAccountConnectionStringKeySecret string
@secure()
param redisCacheConnectionStringKeySecret string

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' existing = {
  name: managedIdentityName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2022-06-01-preview' existing = {
  name: containerAppEnvironmentName
}

var cosmosDatabaseAccountConnectionStringKeySecretName = 'cosmos-connectionstring'
var redisCacheConnectionStringKeySecretName = 'redis-connectionstring'

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: containerAppObject.name
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppEnvironment.id
    configuration: {
      secrets: [
        {
          name: cosmosDatabaseAccountConnectionStringKeySecretName
          value: cosmosDatabaseAccountConnectionStringKeySecret
        }
        {
          name: redisCacheConnectionStringKeySecretName
          value: redisCacheConnectionStringKeySecret
        }
      ]
      ingress: {
        allowInsecure: false
        targetPort: 80
        transport: 'auto'
        external: true
      }
    }
    template: {
      containers: [
        {
          name: containerAppObject.appId
          image: containerAppObject.image
          resources: {
            cpu: containerAppObject.cpu
            memory: containerAppObject.memory
          }
          env: [
            {
              name: 'COSMOS__CONNECTIONSTRING'
              secretRef: cosmosDatabaseAccountConnectionStringKeySecretName
            }
            {
              name: 'REDIS__CONNECTIONSTRING'
              secretRef: redisCacheConnectionStringKeySecretName
            }
          ]
          probes: [
            {
              type: 'Readiness'
              httpGet: {
                path: '/healthz'
                port: containerAppObject.appPort
                scheme: 'HTTP'
              }
              initialDelaySeconds: 30
              periodSeconds: 10
              timeoutSeconds: 5
              failureThreshold: 3
            }
            {
              type: 'Liveness'
              httpGet: {
                path: '/healthz'
                port: containerAppObject.appPort
                scheme: 'HTTP'
              }
              initialDelaySeconds: 30
              periodSeconds: 10
              timeoutSeconds: 5
              failureThreshold: 3
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logAnalyticsWorkspaceName
}

resource diagnosticSettings 'Microsoft.Insights/diagnosticsettings@2017-05-01-preview' = {
  name: 'Logging'
  scope: containerAppEnvironment
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'ContainerAppConsoleLogs'
        enabled: true
      }
      {
        category: 'ContainerAppSystemLogs'
        enabled: true
      }
    ]
  }
}

output containerAppName string = containerApp.name