resource "azuread_application" "main" {
  display_name = "fotos"
  owners       = [data.azuread_client_config.current.object_id]
}

resource "azuread_service_principal" "main" {
  client_id                    = azuread_application.main.client_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
}

resource "azuread_service_principal_password" "main" {
  service_principal_id = azuread_service_principal.main.id
}

resource "azurerm_role_assignment" "identity_storage_blob_data_owner" {
  scope                = azurerm_storage_account.fotos.id
  role_definition_name = "Storage Blob Data Owner"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "folders_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.folders.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azuread_service_principal.main.object_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "albums_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.albums.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azuread_service_principal.main.object_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "photos_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.photos.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azuread_service_principal.main.object_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "session_data_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.session_data.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azuread_service_principal.main.object_id
}

resource "azurerm_cosmosdb_sql_role_assignment" "users_contributor" {
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  scope               = "${data.azurerm_cosmosdb_account.common.id}/dbs/${azurerm_cosmosdb_sql_database.main.name}/colls/${azurerm_cosmosdb_sql_container.users.name}"
  role_definition_id  = "${data.azurerm_cosmosdb_account.common.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002"
  principal_id        = azuread_service_principal.main.object_id
}


resource "azurerm_role_assignment" "identity_servicebus_data_sender" {
  scope                = azurerm_servicebus_topic.fotos.id
  role_definition_name = "Azure Service Bus Data Sender"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_role_assignment" "identity_servicebus_data_receiver" {
  scope                = azurerm_servicebus_topic.fotos.id
  role_definition_name = "Azure Service Bus Data Receiver"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_role_assignment" "identity_key_vault_secrets_user" {
  scope                = data.azurerm_key_vault.common.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_role_assignment" "app_config_reader" {
  scope                = data.azurerm_app_configuration.common.id
  role_definition_name = "App Configuration Data Reader"
  principal_id         = azuread_service_principal.main.object_id
}
