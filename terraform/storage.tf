resource "random_string" "storage_suffix" {
  length  = 6
  special = false
  upper   = false
}

resource "azurerm_storage_account" "fotos" {
  name                     = local.storage_account_name
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "ZRS"
  account_kind             = "StorageV2"

  tags = local.tags
}

resource "azurerm_storage_container" "photos" {
  name                  = "photos"
  container_access_type = "private"
  storage_account_id    = azurerm_storage_account.fotos.id
}
