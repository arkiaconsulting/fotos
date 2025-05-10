resource "azurerm_app_configuration_key" "blob_service_uri" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "MainStorage:blobServiceUri"
  value                  = azurerm_storage_account.fotos.primary_blob_endpoint
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "photos_blob_container" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "MainStorage:PhotosContainer"
  value                  = azurerm_storage_container.photos.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "cosmos_database_id" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Cosmos:DatabaseId"
  value                  = azurerm_cosmosdb_sql_database.main.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "cosmos_photos_container_id" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Cosmos:PhotosContainerId"
  value                  = azurerm_cosmosdb_sql_container.photos.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "cosmos_folders_container_id" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Cosmos:FoldersContainerId"
  value                  = azurerm_cosmosdb_sql_container.folders.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "cosmos_albums_container_id" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Cosmos:AlbumsContainerId"
  value                  = azurerm_cosmosdb_sql_container.albums.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "cosmos_session_data_container_id" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Cosmos:SessionDataContainerId"
  value                  = azurerm_cosmosdb_sql_container.session_data.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "cosmos_users_container_id" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Cosmos:UsersContainerId"
  value                  = azurerm_cosmosdb_sql_container.users.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "servicebus_main_topic" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "ServiceBus:MainTopic"
  value                  = azurerm_servicebus_topic.fotos.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "servicebus_produce_thumbnail_subscription" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "ServiceBus:ProduceThumbnailSubscription"
  value                  = azurerm_servicebus_subscription.produce_thumbnail.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "servicebus_extract_exif_metadata_subscription" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "ServiceBus:ExtractExifMetadataSubscription"
  value                  = azurerm_servicebus_subscription.extract_exif_metadata.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "servicebus_remove_photo_binaries_subscription" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "ServiceBus:RemovePhotoBinariesSubscription"
  value                  = azurerm_servicebus_subscription.remove_photo_binaries.name
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "base_url" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "BaseUrl"
  value                  = "https://${local.hostname}"
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "instrumentation_service_name" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Instrumentation:ServiceName"
  value                  = "fotosapp"
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "google_client_id" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Google:ClientId"
  vault_key_reference    = azurerm_key_vault_secret.google_client_id.versionless_id
  type                   = "vault"
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "google_client_secret" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "Google:ClientSecret"
  vault_key_reference    = azurerm_key_vault_secret.google_client_secret.versionless_id
  type                   = "vault"
  label                  = "fotos"

  tags = local.tags
}

resource "azurerm_app_configuration_key" "access_token_signing_key" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  key                    = "AccessTokenSigningKey"
  vault_key_reference    = azurerm_key_vault_secret.token_key.versionless_id
  type                   = "vault"
  label                  = "fotos"

  tags = local.tags
}










