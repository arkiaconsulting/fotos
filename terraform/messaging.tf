resource "azurerm_servicebus_topic" "fotos" {
  name         = "fotos"
  namespace_id = data.azurerm_servicebus_namespace.common.id

  partitioning_enabled = true
}

resource "azurerm_servicebus_subscription" "produce_thumbnail" {
  name               = "produce-thumbnail"
  topic_id           = azurerm_servicebus_topic.fotos.id
  max_delivery_count = 10
}

resource "azurerm_servicebus_subscription" "remove_photo_binaries" {
  name               = "remove-photo-binaries"
  topic_id           = azurerm_servicebus_topic.fotos.id
  max_delivery_count = 10
}

resource "azurerm_servicebus_subscription" "notify_thumbnail_ready" {
  name               = "notify-thumbnail-ready"
  topic_id           = azurerm_servicebus_topic.fotos.id
  max_delivery_count = 10
}

resource "azurerm_servicebus_subscription" "extract_exif_metadata" {
  name               = "extract-exif-metadata"
  topic_id           = azurerm_servicebus_topic.fotos.id
  max_delivery_count = 10
}

resource "azurerm_servicebus_subscription_rule" "produce_thumbnail_photo_uploaded" {
  name            = "photo-uploaded"
  subscription_id = azurerm_servicebus_subscription.produce_thumbnail.id
  filter_type     = "CorrelationFilter"

  correlation_filter {
    label = "PhotoUploaded"
  }
}

resource "azurerm_servicebus_subscription_rule" "extract_exif_metadata_photo_uploaded" {
  name            = "photo-uploaded"
  subscription_id = azurerm_servicebus_subscription.extract_exif_metadata.id
  filter_type     = "CorrelationFilter"

  correlation_filter {
    label = "PhotoUploaded"
  }
}

resource "azurerm_servicebus_subscription_rule" "notify_thumbnail_ready_thumbnail_ready" {
  name            = "thumbnail-ready"
  subscription_id = azurerm_servicebus_subscription.notify_thumbnail_ready.id
  filter_type     = "CorrelationFilter"

  correlation_filter {
    label = "ThumbnailReady"
  }
}

resource "azurerm_servicebus_subscription_rule" "remove_photo_binaries_photo_removed" {
  name            = "photo-removed"
  subscription_id = azurerm_servicebus_subscription.remove_photo_binaries.id
  filter_type     = "CorrelationFilter"

  correlation_filter {
    label = "PhotoRemoved"
  }
}
