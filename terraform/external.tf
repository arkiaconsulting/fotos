data "azurerm_resources" "common_service_plans" {
  type = "Microsoft.Web/serverfarms"

  required_tags = {
    layer = "common"
  }
}

data "azurerm_service_plan" "common" {
  name                = element(data.azurerm_resources.common_service_plans.resources.*.name, 0)
  resource_group_name = element(data.azurerm_resources.common_service_plans.resources.*.resource_group_name, 0)
}

data "azurerm_resources" "common_log_analytics" {
  type = "Microsoft.OperationalInsights/workspaces"

  required_tags = {
    layer = "common"
  }
}

data "azurerm_log_analytics_workspace" "common" {
  name                = element(data.azurerm_resources.common_log_analytics.resources.*.name, 0)
  resource_group_name = element(data.azurerm_resources.common_log_analytics.resources.*.resource_group_name, 0)
}

data "azurerm_resources" "common_cosmos_account" {
  type = "Microsoft.DocumentDB/databaseAccounts"

  required_tags = {
    layer = "common"
  }
}

data "azurerm_cosmosdb_account" "common" {
  name                = element(data.azurerm_resources.common_cosmos_account.resources.*.name, 0)
  resource_group_name = element(data.azurerm_resources.common_cosmos_account.resources.*.resource_group_name, 0)
}

data "azurerm_resources" "common_service_bus_namespace" {
  type = "Microsoft.ServiceBus/namespaces"

  required_tags = {
    layer = "common"
  }
}

data "azurerm_servicebus_namespace" "common" {
  name                = element(data.azurerm_resources.common_service_bus_namespace.resources.*.name, 0)
  resource_group_name = element(data.azurerm_resources.common_service_bus_namespace.resources.*.resource_group_name, 0)
}

data "azurerm_resources" "common_key_vault" {
  type = "Microsoft.KeyVault/vaults"

  required_tags = {
    layer = "common"
  }
}


data "azurerm_key_vault" "common" {
  name                = element(data.azurerm_resources.common_key_vault.resources.*.name, 0)
  resource_group_name = element(data.azurerm_resources.common_key_vault.resources.*.resource_group_name, 0)
}

data "azurerm_key_vault_certificate" "arkia_dev" {
  name         = "arkia-dev"
  key_vault_id = data.azurerm_key_vault.common.id
}

data "azurerm_app_service_certificate" "arkia_dev" {
  name                = "arkia-dev"
  resource_group_name = data.azurerm_key_vault.common.resource_group_name
}
