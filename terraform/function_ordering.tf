resource "random_string" "storage_name_ordering" {
  length  = 24
  upper   = false
  lower   = true
  numeric = true
  special = false
}

resource "azurerm_storage_account" "ordering_fn_storage" {
  name                     = random_string.storage_name_ordering.result
  resource_group_name      = azurerm_resource_group.resource_group.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_linux_function_app" "ordering_function_app" {
  name                = "${local.prefix}-ordering-function-app"
  location            = var.location
  resource_group_name = azurerm_resource_group.resource_group.name

  service_plan_id            = azurerm_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.ordering_fn_storage.name
  storage_account_access_key = azurerm_storage_account.ordering_fn_storage.primary_access_key

  functions_extension_version = "~4"
  https_only                  = true

  app_settings = {
    "FUNCTIONS_WORKER_RUNTIME"       = "dotnet-isolated"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.app_insights.instrumentation_key
    "CosmosDbConnectionString"       = "AccountEndpoint=${azurerm_cosmosdb_account.cosmosdb.endpoint};AccountKey=${azurerm_cosmosdb_account.cosmosdb.primary_key};"
    "CosmosDbDatabaseName"           = "${azurerm_cosmosdb_sql_database.sql_database.name}"
    "CosmosDbContainerName"          = "${azurerm_cosmosdb_sql_container.sql_container.name}"
    "CosmosDbLeasesContainerName"    = "${azurerm_cosmosdb_sql_container.sql_container_leases.name}"
    "ServiceBusConnectionString"     = local.servicebus_connection_string
    "ServiceBusTopicName"            = local.orders_topic_name
  }

  site_config {}
}