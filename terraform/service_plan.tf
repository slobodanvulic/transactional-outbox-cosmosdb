resource "azurerm_service_plan" "app_service_plan" {
  name                = "${local.prefix}-app_service_plan"
  resource_group_name = azurerm_resource_group.resource_group.name
  location            = var.location
  os_type             = "Linux"
  sku_name            = "Y1"

}