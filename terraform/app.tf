resource "azurerm_windows_web_app" "main" {
  name                            = local.web_app_name
  resource_group_name             = azurerm_resource_group.main.name
  location                        = azurerm_resource_group.main.location
  service_plan_id                 = data.azurerm_service_plan.common.id
  https_only                      = true
  client_affinity_enabled         = true
  key_vault_reference_identity_id = azurerm_user_assigned_identity.main.id

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.main.id]
  }

  site_config {
    always_on           = true
    http2_enabled       = true
    minimum_tls_version = 1.2
    websockets_enabled  = true

    application_stack {
      dotnet_version = "v8.0"
    }
  }

  app_settings = {
    "AZURE_CLIENT_ID"                             = azurerm_user_assigned_identity.main.client_id
    "APPINSIGHTS_INSTRUMENTATIONKEY"              = azurerm_application_insights.main.instrumentation_key
    "APPLICATIONINSIGHTS_CONNECTION_STRING"       = azurerm_application_insights.main.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION"  = "~3"
    "Logging:LogLevel:Default"                    = "Information"
    "Logging:LogLevel:Microsoft.AspNetCore"       = "Warning"
    "Logging:LogLevel:Azure.Messaging.ServiceBus" = "Warning"
    "Logging:LogLevel:Azure.Core"                 = "Warning"
    "System.Net.Http"                             = "Warning"
    "MainStorage:blobServiceUri"                  = "https://${azurerm_storage_account.photos.name}.blob.core.windows.net/"
    "MainStorage:PhotosContainer"                 = azurerm_storage_container.photos.name
    "CosmosDb:AccountEndpoint"                    = data.azurerm_cosmosdb_account.common.endpoint
    "CosmosDb:DatabaseId"                         = azurerm_cosmosdb_sql_database.main.name
    "CosmosDb:ContainerId"                        = azurerm_cosmosdb_sql_container.photos.name
    "CosmosDb:FoldersContainerId"                 = azurerm_cosmosdb_sql_container.folders.name
    "CosmosDb:AlbumsContainerId"                  = azurerm_cosmosdb_sql_container.albums.name
    "CosmosDb:SessionDataContainerId"             = azurerm_cosmosdb_sql_container.session_data.name
    "ServiceBus:fullyQualifiedNamespace"          = "${data.azurerm_servicebus_namespace.common.name}.servicebus.windows.net"
    "ServiceBus:MainTopic"                        = azurerm_servicebus_topic.fotos.name
    "ServiceBus:ProduceThumbnailSubscription"     = azurerm_servicebus_subscription.produce_thumbnail.name
    "ServiceBus:ExtractExifMetadataSubscription"  = azurerm_servicebus_subscription.extract_exif_metadata.name
    "ServiceBus:RemovePhotoBinariesSubscription"  = azurerm_servicebus_subscription.remove_photo_binaries.name
    "BaseUrl"                                     = "https://${local.web_app_name}.azurewebsites.net"
    "Instrumentation:ServiceName"                 = "fotos-app"
    "Google:ClientId"                             = "@Microsoft.KeyVault(SecretUri=${local.google.client_id})"
    "Google:ClientSecret"                         = "@Microsoft.KeyVault(SecretUri=${local.google.client_secret})"
  }

  tags = local.tags
}
