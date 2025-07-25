terraform {
  required_version = "~> 1.11.4"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 4.28.0, < 5.0"
    }

    azuread = {
      source  = "hashicorp/azuread"
      version = "3.4.0"
    }

    random = {
      source  = "hashicorp/random"
      version = "3.7.2"
    }
  }

  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}
