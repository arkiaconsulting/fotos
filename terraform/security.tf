resource "random_password" "token_key" {
  length  = 32
  special = true
}

resource "azurerm_key_vault_secret" "token_key" {
  name         = "fotos-token-key"
  value        = base64encode(random_password.token_key.result)
  key_vault_id = data.azurerm_key_vault.common.id
}

resource "azurerm_key_vault_secret" "google_client_id" {
  name         = "fotos-google-client-id"
  value        = var.fotos_google_client_id
  key_vault_id = data.azurerm_key_vault.common.id
}

resource "azurerm_key_vault_secret" "google_client_secret" {
  name         = "fotos-google-client-secret"
  value        = var.fotos_google_client_secret
  key_vault_id = data.azurerm_key_vault.common.id
}


