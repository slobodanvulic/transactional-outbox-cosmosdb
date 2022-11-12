resource "azurerm_servicebus_namespace" "servicebus" {
  name                = "${local.prefix}-servicebus-namespace"
  location            = var.location
  resource_group_name = azurerm_resource_group.resource_group.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_namespace_authorization_rule" "rule" {
  name         = "authorization-rule"
  namespace_id = azurerm_servicebus_namespace.servicebus.id

  listen = true
  send   = true
  manage = true
}

resource "azurerm_servicebus_topic" "orders_topic" {
  name         = local.orders_topic_name
  namespace_id = azurerm_servicebus_namespace.servicebus.id

  enable_partitioning = false
}

# Inventory subscription and fwd queue
resource "azurerm_servicebus_queue" "inventory_orders_fwd_queue" {
  name         = local.inventory_orders_fwd_queue_name
  namespace_id = azurerm_servicebus_namespace.servicebus.id
}

resource "azurerm_servicebus_subscription" "inventory_orders_subscription" {
  name               = local.inventory_orders_subscription_name
  topic_id           = azurerm_servicebus_topic.orders_topic.id
  forward_to         = azurerm_servicebus_queue.inventory_orders_fwd_queue.name
  max_delivery_count = 1
}


# Invoicing subscription and fwd queue
resource "azurerm_servicebus_queue" "invoicing_orders_fwd_queue" {
  name         = local.invoicing_orders_fwd_queue_name
  namespace_id = azurerm_servicebus_namespace.servicebus.id
}

resource "azurerm_servicebus_subscription" "invoicing_orders_subscription" {
  name               = local.invoicing_orders_subscription_name
  topic_id           = azurerm_servicebus_topic.orders_topic.id
  forward_to         = azurerm_servicebus_queue.invoicing_orders_fwd_queue.name
  max_delivery_count = 1
}