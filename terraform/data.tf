resource "azurerm_cosmosdb_sql_database" "main" {
  name                = "fotos"
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name

  autoscale_settings {
    max_throughput = 1000
  }
}

resource "azurerm_cosmosdb_sql_container" "folders" {
  name                = "folders"
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  database_name       = azurerm_cosmosdb_sql_database.main.name

  partition_key_paths = ["/parentId"]
}

resource "azurerm_cosmosdb_sql_container" "albums" {
  name                = "albums"
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  database_name       = azurerm_cosmosdb_sql_database.main.name

  partition_key_paths = ["/folderId"]
}

resource "azurerm_cosmosdb_sql_container" "photos" {
  name                = "photos"
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  database_name       = azurerm_cosmosdb_sql_database.main.name

  partition_key_paths   = ["/folderId", "/albumId"]
  partition_key_version = 2
  partition_key_kind    = "MultiHash"
}

resource "azurerm_cosmosdb_sql_container" "session_data" {
  name                = "session-data"
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  database_name       = azurerm_cosmosdb_sql_database.main.name

  partition_key_paths = ["/id"]
}
