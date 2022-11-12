locals {
  prefix = "svulic-devweek-2022"

  # Service bus
  servicebus_connection_string = "Endpoint=sb://${azurerm_servicebus_namespace.servicebus.name}.servicebus.windows.net/;SharedAccessKeyName=${azurerm_servicebus_namespace_authorization_rule.rule.name};SharedAccessKey=${azurerm_servicebus_namespace_authorization_rule.rule.primary_key}"

  orders_topic_name = "OrderingService.Order.OrderCreated"

  inventory_orders_subscription_name = "Inventory.Order.OrderCreated.Subscription"
  inventory_orders_fwd_queue_name    = "Inventory.Order.Fwd.OrderCreated"

  invoicing_orders_subscription_name = "Invoicing.Order.OrderCreated.Subscription"
  invoicing_orders_fwd_queue_name    = "Invoicing.Order.Fwd.OrderCreated"

}