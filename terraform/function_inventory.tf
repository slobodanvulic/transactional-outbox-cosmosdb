resource "random_string" "storage_name_inventory" {
  length  = 24
  upper   = false
  lower   = true
  numeric = true
  special = false
}

resource "azurerm_storage_account" "inventory_fn_storage" {
  name                     = random_string.storage_name_inventory.result
  resource_group_name      = azurerm_resource_group.resource_group.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_linux_function_app" "inventory_function_app" {
  name                = "${local.prefix}-inventory-function-app"
  location            = var.location
  resource_group_name = azurerm_resource_group.resource_group.name

  service_plan_id            = azurerm_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.inventory_fn_storage.name
  storage_account_access_key = azurerm_storage_account.inventory_fn_storage.primary_access_key

  functions_extension_version = "~4"
  https_only                  = true

  app_settings = {
    "FUNCTIONS_WORKER_RUNTIME"          = "dotnet-isolated"
    "APPINSIGHTS_INSTRUMENTATIONKEY"    = azurerm_application_insights.app_insights.instrumentation_key
    "ServiceBusConnectionString"        = local.servicebus_connection_string
    "ServiceBusigOrderCreatedQueueName" = local.inventory_orders_fwd_queue_name
  }

  site_config {}
}