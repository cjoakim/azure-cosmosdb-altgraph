using Microsoft.Azure.CosmosRepository.Attributes;

namespace altgraph_shared_app.Models.Npm
{
  [Container(Constants.NPM_CONTAINER_NAME)]
  [PartitionKeyPath(Constants.PARTITION_KEY)]
  public class Maintainer : NpmDocument
  {

  }
}