resource "azurerm_resource_group" "main" {
  name     = "fotos"
  location = local.location

  tags = local.tags
}
