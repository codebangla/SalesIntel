import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { useGetInventoryAlertsQuery, useGetOrdersQuery, useGetProductsQuery } from "@/store/api/salesIntelApi"
import { TrendingUp, Activity, Target, Package } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { formatCurrency } from "@/lib/utils"

export function Dashboard() {
  const { data: products = [] } = useGetProductsQuery({})
  const { data: orders = [] } = useGetOrdersQuery({})
  const { data: alerts = [] } = useGetInventoryAlertsQuery()

  const totalProducts = products.length
  const totalOrders = orders.length
  const totalRevenue = orders.reduce((sum, order) => sum + order.amount, 0)
  const criticalAlerts = alerts.filter((alert) => alert.severity === "critical").length

  return (
    <div className="flex-1 space-y-4 p-4 md:p-8 pt-6">
      <div className="flex items-center justify-between space-y-2">
        <h2 className="text-3xl font-bold tracking-tight">Dashboard</h2>
      </div>

      <Tabs defaultValue="overview" className="space-y-4">
        <TabsList>
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="analytics">Analytics</TabsTrigger>
          <TabsTrigger value="reports">Reports</TabsTrigger>
        </TabsList>

        <TabsContent value="overview" className="space-y-4">
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">Total Products</CardTitle>
                <Package className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">{totalProducts}</div>
                <p className="text-xs text-muted-foreground">Active inventory items</p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">Total Orders</CardTitle>
                <Activity className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">{totalOrders}</div>
                <p className="text-xs text-muted-foreground">All time orders</p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
                <TrendingUp className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">{formatCurrency(totalRevenue)}</div>
                <p className="text-xs text-muted-foreground">From all orders</p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">Critical Alerts</CardTitle>
                <Target className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold text-red-600">{criticalAlerts}</div>
                <p className="text-xs text-muted-foreground">Require immediate attention</p>
              </CardContent>
            </Card>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <Card>
              <CardHeader>
                <CardTitle>Recent Orders</CardTitle>
                <CardDescription>Latest order activities</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {orders.slice(0, 5).map((order) => (
                    <div key={order.id} className="flex items-center justify-between">
                      <div>
                        <p className="text-sm font-medium">{order.customer}</p>
                        <p className="text-xs text-muted-foreground">
                          {order.orderNumber} • {order.type}
                        </p>
                      </div>
                      <div className="flex items-center space-x-2">
                        <Badge
                          variant={
                            order.status === "Completed"
                              ? "default"
                              : order.status === "Processing"
                                ? "secondary"
                                : "outline"
                          }
                        >
                          {order.status}
                        </Badge>
                        <span className="font-medium">{formatCurrency(order.amount)}</span>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Inventory Alerts</CardTitle>
                <CardDescription>Products requiring attention</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {alerts.slice(0, 5).map((alert) => (
                    <div key={alert.productId} className="flex items-center justify-between">
                      <div>
                        <p className="text-sm font-medium">{alert.productName}</p>
                        <p className="text-xs text-muted-foreground">
                          {alert.sku} • Stock: {alert.currentStock}
                        </p>
                      </div>
                      <Badge
                        variant={
                          alert.severity === "critical"
                            ? "destructive"
                            : alert.severity === "high"
                              ? "destructive"
                              : "secondary"
                        }
                      >
                        {alert.severity}
                      </Badge>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          </div>
        </TabsContent>
      </Tabs>
    </div>
  )
}
