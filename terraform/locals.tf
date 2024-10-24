locals {
  tags = {
    project = "fotos"
  }

  prefix = "fotos"

  web_app_name = "${local.prefix}-web-app"

  app_insights_name = "${local.prefix}-appinsights"

  storage_account_name_prefix = "${local.prefix}photos"
  storage_account_name        = "${local.storage_account_name_prefix}${random_string.storage_suffix.result}"
}
