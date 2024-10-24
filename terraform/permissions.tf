resource "azurerm_user_assigned_identity" "main" {
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  name = "fotos-identity"
}

resource "azurerm_role_assignment" "identity_storage_blob_data_owner" {
  scope                = azurerm_storage_account.photos.id
  role_definition_name = "Storage Blob Data Owner"
  principal_id         = azurerm_user_assigned_identity.main.principal_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "folders_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.folders.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azurerm_user_assigned_identity.main.principal_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "albums_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.albums.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azurerm_user_assigned_identity.main.principal_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "photos_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.photos.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azurerm_user_assigned_identity.main.principal_id
}

resource "azurerm_role_assignment" "identity_servicebus_data_sender" {
  scope                = data.azurerm_servicebus_namespace.common.id
  role_definition_name = "Azure Service Bus Data Sender"
  principal_id         = azurerm_user_assigned_identity.main.principal_id
}

resource "azurerm_role_assignment" "identity_servicebus_data_receiver" {
  scope                = data.azurerm_servicebus_namespace.common.id
  role_definition_name = "Azure Service Bus Data Receiver"
  principal_id         = azurerm_user_assigned_identity.main.principal_id
}
