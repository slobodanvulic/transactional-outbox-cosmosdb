resource "azurerm_application_insights" "app_insights" {
  name                = "${local.prefix}-appinsights"
  location            = var.location
  resource_group_name = azurerm_resource_group.resource_group.name
  application_type    = "web"
}