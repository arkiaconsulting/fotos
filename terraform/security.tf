resource "random_password" "token_key" {
  length  = 32
  special = true
}

resource "azurerm_key_vault_secret" "token_key" {
  name         = "fotos-token-key"
  value        = base64encode(random_password.token_key.result)
  key_vault_id = data.azurerm_key_vault.common.id
}
