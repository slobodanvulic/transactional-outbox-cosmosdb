resource "azurerm_cosmosdb_account" "cosmosdb" {
  name                = "${local.prefix}-cosmosdb-account"
  location            = var.location
  resource_group_name = azurerm_resource_group.resource_group.name
  offer_type          = "Standard"

  enable_automatic_failover = false

  consistency_policy {
    consistency_level = "BoundedStaleness"
  }

  geo_location {
    location          = var.location
    failover_priority = 0
    zone_redundant    = true
  }
}

resource "azurerm_cosmosdb_sql_database" "sql_database" {
  name                = "${local.prefix}-sql-database"
  resource_group_name = azurerm_resource_group.resource_group.name
  account_name        = azurerm_cosmosdb_account.cosmosdb.name
}

resource "azurerm_cosmosdb_sql_container" "sql_container" {
  name                  = "orders"
  resource_group_name   = azurerm_resource_group.resource_group.name
  account_name          = azurerm_cosmosdb_account.cosmosdb.name
  database_name         = azurerm_cosmosdb_sql_database.sql_database.name
  partition_key_path    = "/partitionKey"
  partition_key_version = 1
  default_ttl           = -1
  throughput            = 400

  indexing_policy {
    indexing_mode = "consistent"
  }
}

resource "azurerm_cosmosdb_sql_container" "sql_container_leases" {
  name                = "orders-leases"
  resource_group_name = azurerm_resource_group.resource_group.name
  account_name        = azurerm_cosmosdb_account.cosmosdb.name
  database_name       = azurerm_cosmosdb_sql_database.sql_database.name
  partition_key_path  = "/id"
}