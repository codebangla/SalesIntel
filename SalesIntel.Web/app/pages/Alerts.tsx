import { useGetInventoryAlertsQuery } from "@/store/api/salesIntelApi"
import { InventoryAlerts } from "@/components/InventoryAlerts"

export function Alerts() {
  const { data: alerts = [], isLoading } = useGetInventoryAlertsQuery()

  if (isLoading) {
    return <div className="p-8">Loading...</div>
  }

  return (
    <div className="container mx-auto py-6 px-4">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Alerts</h1>
      </div>

      <InventoryAlerts alerts={alerts} />
    </div>
  )
}
