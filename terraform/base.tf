resource "azurerm_resource_group" "main" {
  name     = "fotos"
  location = "francecentral"

  tags = local.tags
}
