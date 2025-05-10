
data "azurerm_resources" "common_cosmos_account" {
  type = "Microsoft.DocumentDB/databaseAccounts"

  required_tags = {
    level = "common"
  }
}

data "azurerm_cosmosdb_account" "common" {
  name                = data.azurerm_resources.common_cosmos_account.resources[0].name
  resource_group_name = data.azurerm_resources.common_cosmos_account.resources[0].resource_group_name
}

data "azurerm_resources" "common_service_bus_namespace" {
  type = "Microsoft.ServiceBus/namespaces"

  required_tags = {
    level = "common"
  }
}

data "azurerm_servicebus_namespace" "common" {
  name                = data.azurerm_resources.common_service_bus_namespace.resources[0].name
  resource_group_name = data.azurerm_resources.common_service_bus_namespace.resources[0].resource_group_name
}

data "azurerm_resources" "common_key_vault" {
  type = "Microsoft.KeyVault/vaults"

  required_tags = {
    level = "common"
  }
}


data "azurerm_key_vault" "common" {
  name                = data.azurerm_resources.common_key_vault.resources[0].name
  resource_group_name = data.azurerm_resources.common_key_vault.resources[0].resource_group_name
}

data "azurerm_key_vault_certificate" "arkia_dev" {
  name         = "arkia-dev"
  key_vault_id = data.azurerm_key_vault.common.id
}

data "azurerm_resources" "app_container_environment" {
  type = "Microsoft.App/managedEnvironments"

  required_tags = {
    level = "common"
  }
}

data "azurerm_container_app_environment" "common" {
  name                = data.azurerm_resources.app_container_environment.resources[0].name
  resource_group_name = data.azurerm_resources.app_container_environment.resources[0].resource_group_name
}

data "azurerm_container_app_environment_certificate" "arkia_dev" {
  name                         = "arkia-dev"
  container_app_environment_id = data.azurerm_container_app_environment.common.id
}

# Find App Configuration
data "azurerm_resources" "app_configuration" {
  type = "Microsoft.AppConfiguration/configurationStores"

  required_tags = {
    level = "common"
  }
}

data "azurerm_app_configuration" "common" {
  name                = data.azurerm_resources.app_configuration.resources[0].name
  resource_group_name = data.azurerm_resources.app_configuration.resources[0].resource_group_name
}

# Find ACR
data "azurerm_resources" "acr" {
  type = "Microsoft.ContainerRegistry/registries"

  required_tags = {
    level = "common"
  }
}

data "azurerm_container_registry" "common" {
  name                = data.azurerm_resources.acr.resources[0].name
  resource_group_name = data.azurerm_resources.acr.resources[0].resource_group_name
}
