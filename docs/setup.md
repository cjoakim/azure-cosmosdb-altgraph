# AltGraph - How to Setup This App in your Environment

## Required Development Desktop Software

- git source control system
- .NET 7
- Docker desktop is optional, but it may be the simplest way to run AltGraph yourself

## Azure PaaS Services

- **Azure CosmosDB SQL API Account**
  - Database named **dev**
  - Container named **npm_graph**
    - Partition key: **/pk**
    - Indexing Policy:
      - See file default indexing file **altgraph-data-app/indexing/default.json** in this repo
      - See recommended file **altgraph-data-app/indexing/altgraph_indexing.json** in this repo
  - Container named **imdb_graph**
    - Partition key: **/pk**
  - Container named **imdb_seed**
    - Partition key: **/pk**
- **Azure Redis Cache Account**
- **Azure Container Apps (optional)**

## Environment Variables

- See files:
  - **src/altgraph-data-app/appsettings.json**
  - **src/altgraph-web-app/appsettings.json**
- See your Azure Portal for the values of these settings

If you accept the default names for the CosmosDB database & container, the only values you need to specify are the connection strings to Redis & CosmosDB.

### Example - Windows PowerShell

```
$env:COSMOS__CONNECTIONSTRING="AccountEndpoint=https://cdb-altgraph-ussc-dev.documents.azure.com:443/;AccountKey=GnqEkKYYdNzFakekeyMINkJrmtIm4gng4kQ9J2kB6uhJjkWJkWJOK768tG0WACDbl7wnHw==;"

$env:REDIS__CONNECTIONSTRING="redis-altgraph-ussc-dev.redis.cache.windows.net:6380,password=Gndk9f2kFFakekey66UKVJ3HAzCaFRkYG8=,ssl=True,abortConnect=False"
```

### Example - Linux

```
set COSMOS__CONNECTIONSTRING="AccountEndpoint=https://cdb-altgraph-ussc-dev.documents.azure.com:443/;AccountKey=GnqEkKYYdNzFakekeyMINkJrmtIm4gng4kQ9J2kB6uhJjkWJkWJOK768tG0WACDbl7wnHw==;"

set REDIS__CONNECTIONSTRING="redis-altgraph-ussc-dev.redis.cache.windows.net:6380,password=Gndk9f2kFFakekey66UKVJ3HAzCaFRkYG8=,ssl=True,abortConnect=False"
```

## Clone the Repo and compile the code

The **>** pefix character in these instructions refer to your shell prompt;
such as in a PowerShell or macOS Terminal. These instructions are oriented
toward Windows PowerShell; equivalent bash script for linux/macOS may be
added at a later date.

Note: You will have to open a new PowerShell or linux/macOS terminal **after**
you have set your environment variable.

```
> git clone https://github.com/jordanbean-msft/azure-cosmosdb-altgraph.git

> cd azure-cosmosdb-altgraph/src

> dotnet build altgraph_data_app.sln

> dotnet build altgraph_web_app.sln
```

## Download the Raw IMDb data

These files are too large to store in this GitHub, so you need to download them.

Visit https://datasets.imdbws.com/ with your web browser, and download the
following three files...

```
name.basics.tsv
title.basics.tsv
title.principals.tsv
```

...to directory **src/altgraph-data-app/data/imdb_raw** within this repository.
_NOTE_ You may have to manually create this directory if it doesn't already exist.

## Wrangle the Raw Data, load CosmosDB

This process transforms the raw data into a format suitable for loading
into CosmosDB for the AltGraph reference applications.

```
> .\imdb_wrangling_process.ps1

> .\npm_load_cosmos.ps1

> .\imdb_load_cosmos.ps1
```

The imdb wrangling and loading steps take several minutes each
due to the size of the data.

## Running the AltGraph Web Application

There are multiple ways to do this; the following are instructions
for the three most common ways:

- The "Software Developer" way with code
- Docker and Docker Compose
- Azure Container Apps

All three approaches described below assume that you have first provisioned
your Azure resources, have updated your `appsettings.json` files and have wrangled/loaded
the data as described above.

In all cases, it may take approximately 50-seconds for the IMDb in-memory graph data
to be loaded from your CosmosDB.

Once the AltGraph web application is running, see file **CosmosDB-AltGraph.pdf**
regarding web application UI usage.

### Running AltGraph - The Software Development Way

```
> cd src/altgraph_web_app

> dotnet run               <-- Runs the Web Application locally
                               Then visit http://localhost:5224 (note that your port # may be different) with your browser
```

### Running AltGraph - with Docker and Docker Compose locally

**NOTE: You must have already set the environment variables (REDIS\_\_CONNECTIONSTRING & COSMOS_CONNECTIONSTRING) described above.**

```
> cd src

> docker compose up -d
```

This approach downloads Docker container **ghcr.io/jordanbean-msft/azure-cosmosdb-altgraph/altgraph-web-app:release-\*.\*.\***
from DockerHub and runs it on your host.

### Running AltGraph - in an Azure Container App - via Azure Developer CLI

```
> azd init

> azd provision
```

_NOTE: the `azd provision` step will take approximately 40 minutes to complete (mostly due to creating the Redis cache)._

Now follow the steps in the [Environment Variables](#environment-variables) & [Wrangle the Raw Data, load CosmosDB](#wrangle-the-raw-data-load-cosmosdb) sections above to load the data.

After the Azure Container Apps is provisioned, visit the `containerAppFQDN` at the FQDN shown in the deployment output. Allow one minute for the ACA to start.

### Running AltGraph - in an Azure Container App - via Az CLI

Set the following environment varibles (substitute your environment name (prefix for resource group name), location & UPN for the one shown)

```
> $env:AZURE_ENV_NAME="altgraphrjb"

> $env:AZURE_LOCATION="SouthCentralUS"

> $env:AZURE_PRINCIPAL_ID=$(az ad user show --id dwight.schrute@dunder-mifflin.com --query id -o tsv)
```

```
> cd infra

> az deployment create --template-file main.bicep --parameters main.parameters.json --location SouthCentralUS --parameters environmentName=$env:AZURE_ENV_NAME location=$env:AZURE_LOCATION principalId=$env:AZURE_PRINCIPAL_ID
```

_NOTE: the `az deployment create` step will take approximately 40 minutes to complete (mostly due to creating the Redis cache)._

Now follow the steps in the [Environment Variables](#environment-variables) & [Wrangle the Raw Data, load CosmosDB](#wrangle-the-raw-data-load-cosmosdb) sections above to load the data.
