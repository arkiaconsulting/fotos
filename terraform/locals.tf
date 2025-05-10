locals {
  tags = {
    project = "fotos"
  }

  location = "francecentral"
  prefix   = "fotos"

  storage_account_name_prefix = local.prefix
  storage_account_name        = "${local.storage_account_name_prefix}${random_string.storage_suffix.result}"

  hostname = "fotos.arkia.dev"
}
