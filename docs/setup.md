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
      - See file default indexing file **altgraph_data_app/indexing/default.json** in this repo
      - See recommended file **altgraph_data_app/indexing/altgraph_indexing.json** in this repo

  - Container named **imdb_graph**

    - Partition key: **/pk**

  - Container named **imdb_seed**
    - Partition key: **/pk**

- **Azure Redis Cache Account**

- **Azure Container Instance (optional)**

At this time this repo only has provisioning scripts for the Azure Container Instance
(see the az directory). It is expected that you manually provision the other resources
in Azure Portal.

## Environment Variables

- See files:
  - **altgraph_data_app/appsettings.json**
  - **altgraph_web_app/appsettings.json**
- See your Azure Portal for the values of these settings

## Clone the Repo and compile the code

The **>** pefix character in these instructions refer to your shell prompt;
such as in a PowerShell or macOS Terminal. These instructions are oriented
toward Windows PowerShell; equivalent bash script for linux/macOS may be
added at a later date.

Note: You will have to open a new PowerShell or linux/macOS terminal **after**
you have set your environment variable.

```
> git clone https://github.com/jordanbean-msft/azure-cosmosdb-altgraph.git

> cd azure-cosmosdb-altgraph

> dotnet build
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

...to directory **altgraph_data_app/data/imdb_raw** within this repository.
You may have to manually create this directory if it doesn't already exist.

## Wrangle the Raw Data, load CosmosDB

This process transforms the raw data into a format suitable for loading
into CosmosDB for the AltGraph reference applications.

```
> cd altgraph_data_app

> .\npm_wrangling_process.ps1

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
- Azure Container Instance

All three approaches described below assume that you have first provisioned
your Azure resources, have updated your `appsettings.json` files and have wrangled/loaded
the data as described above.

In all cases, it may take approximately 50-seconds for the IMDb in-memory graph data
to be loaded from your CosmosDB.

Once the AltGraph web application is running, see file **CosmosDB-AltGraph.pdf**
regarding web application UI usage.

### Running AltGraph - The Software Development Way

```
> cd altgraph_web_app

> dotnet run               <-- Runs the Web Application locally
                               Then visit http://localhost:5224 (note that your port # may be different) with your browser
```

### Running AltGraph - with Docker and Docker Compose locally

```
> cd altgraph_web_app

> docker-compose up -d
```

This approach downloads Docker container **cjoakim/azure-cosmosdb-altgraph-v2**
from DockerHub and runs it on your host.

See the comments at the end of file **altgraph_web_app/docker-compose.yml**
regarding common docker and docker-compose commands.

### Running AltGraph - in an Azure Container Instance

```
> cd az

... read and modify the provision-webapp-aci1.ps1 script, as necessary, with your configuration info

> .\provision-webapp-aci1.ps1

... after the Azure Container Instance is provisioned, visit port 8080 at the fqdn
... shown in the deployment output.  Allow one-minute for the ACI to start.
```
