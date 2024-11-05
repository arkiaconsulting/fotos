# Custom domain configuration for the web app
resource "azurerm_app_service_custom_hostname_binding" "main" {
  hostname            = "fotos.arkia.dev"
  app_service_name    = azurerm_windows_web_app.main.name
  resource_group_name = azurerm_resource_group.main.name

  depends_on = [
    azurerm_windows_web_app.main
  ]
}

# Bind the certificate to the custom domain
resource "azurerm_app_service_certificate_binding" "main" {
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.main.id
  certificate_id      = data.azurerm_app_service_certificate.arkia_dev.id
  ssl_state           = "SniEnabled"
}
