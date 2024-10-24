output "web_app_name" {
  description = "The name of the Windows web app"
  value       = azurerm_windows_web_app.main.name
}
