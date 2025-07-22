import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { AlertTriangle, Package, TrendingDown, RefreshCw } from "lucide-react"
import type { InventoryAlertDto } from "@/store/api/salesIntelApi"

interface InventoryAlertsProps {
  alerts: InventoryAlertDto[]
}

export function InventoryAlerts({ alerts }: InventoryAlertsProps) {
  const getSeverityIcon = (severity: string) => {
    switch (severity) {
      case "critical":
        return <AlertTriangle className="h-5 w-5 text-red-600" />
      case "high":
        return <TrendingDown className="h-5 w-5 text-orange-600" />
      case "medium":
        return <Package className="h-5 w-5 text-yellow-600" />
      default:
        return <Package className="h-5 w-5" />
    }
  }

  const getSeverityColor = (severity: string) => {
    switch (severity) {
      case "critical":
        return "destructive"
      case "high":
        return "destructive"
      case "medium":
        return "secondary"
      default:
        return "outline"
    }
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Inventory Alerts</CardTitle>
            <CardDescription>Products where current stock is below forecasted demand</CardDescription>
          </div>
          <Button variant="outline" size="sm">
            <RefreshCw className="mr-2 h-4 w-4" />
            Refresh
          </Button>
        </div>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          {alerts.length === 0 ? (
            <p className="text-center text-gray-500 py-8">No inventory alerts at this time</p>
          ) : (
            alerts.map((alert) => (
              <div key={alert.productId} className="flex items-start space-x-4 p-4 border rounded-lg">
                {getSeverityIcon(alert.severity)}
                <div className="flex-1 space-y-2">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="font-medium">{alert.productName}</p>
                      <p className="text-sm text-muted-foreground">{alert.sku}</p>
                    </div>
                    <Badge variant={getSeverityColor(alert.severity) as any}>{alert.severity}</Badge>
                  </div>

                  <div className="grid grid-cols-3 gap-4 text-sm">
                    <div>
                      <p className="text-muted-foreground">Current Stock</p>
                      <p className="font-medium">{alert.currentStock} units</p>
                    </div>
                    <div>
                      <p className="text-muted-foreground">Future Demand</p>
                      <p className="font-medium">{alert.futureInvoiceQuantities} units</p>
                    </div>
                    <div>
                      <p className="text-muted-foreground">Shortfall</p>
                      <p className="font-medium text-red-600">-{alert.shortfall} units</p>
                    </div>
                  </div>

                  <div className="flex items-center justify-between">
                    <p className="text-sm text-muted-foreground">
                      Estimated stockout in {alert.daysUntilStockout} day{alert.daysUntilStockout !== 1 ? "s" : ""}
                    </p>
                    <div className="space-x-2">
                      <Button size="sm" variant="outline">
                        Create PO
                      </Button>
                      <Button size="sm">Reorder</Button>
                    </div>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      </CardContent>
    </Card>
  )
}
