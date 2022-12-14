# az CLI script check the status of the ACI containers.
# Chris Joakim, Microsoft, November 2022

# Parameters - change these per your Azure environment
$subscription=$Env:AZURE_SUBSCRIPTION_ID
$resource_group='chjoakimagaci'
$resource_name1='chjoakimagaci1'
$resource_name21='chjoakimagaci2'
$azure_location='eastus'

#echo 'logging in...'
#az login

echo "az account set to subscription: $subscription"
az account set --subscription $subscription

az container list --resource-group $resource_group > tmp\aci_list.json

az container show --resource-group $resource_group --name $resource_name1 > tmp\aci1_show.json
az container show --resource-group $resource_group --name $resource_name2 > tmp\aci2_show.json

# Other ACI commands:
# az container stop --resource-group $resource_group --name $resource_name1
# az container stop --resource-group $resource_group --name $resource_name2
# az container start --resource-group $resource_group --name $resource_name1
# az container start --resource-group $resource_group --name $resource_name2
