import { OrdersTable } from "@/components/orders/orders-table"

export default function OrdersPage() {
  return (
    <div className="container mx-auto py-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Orders</h1>
      </div>
      <OrdersTable />
    </div>
  )
}
