"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Plus, Search, FileText } from "lucide-react"
import { useGetOrdersQuery, useConvertToInvoiceMutation } from "@/store/api/salesIntelApi"
import { OrderDialog } from "@/components/OrderDialog"
import { formatCurrency, formatDate } from "@/lib/utils"

export function Orders() {
  const [search, setSearch] = useState("")
  const [activeTab, setActiveTab] = useState("all")
  const [showDialog, setShowDialog] = useState(false)

  const { data: orders = [], isLoading } = useGetOrdersQuery({
    search,
    type: activeTab === "all" ? undefined : activeTab,
  })

  const [convertToInvoice] = useConvertToInvoiceMutation()

  const handleConvertToInvoice = async (orderId: number) => {
    try {
      await convertToInvoice(orderId).unwrap()
    } catch (error) {
      console.error("Failed to convert order to invoice:", error)
    }
  }

  if (isLoading) {
    return <div className="p-8">Loading...</div>
  }

  return (
    <div className="container mx-auto py-6 px-4">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Orders</h1>
        <Button onClick={() => setShowDialog(true)}>
          <Plus className="mr-2 h-4 w-4" />
          New Order
        </Button>
      </div>

      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <div className="flex items-center justify-between mb-6">
          <TabsList>
            <TabsTrigger value="all">All Orders</TabsTrigger>
            <TabsTrigger value="Quotation">Quotations</TabsTrigger>
            <TabsTrigger value="Order">Orders</TabsTrigger>
            <TabsTrigger value="Invoice">Invoices</TabsTrigger>
          </TabsList>

          <div className="relative max-w-sm">
            <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Search orders..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-8"
            />
          </div>
        </div>

        <TabsContent value={activeTab}>
          <div className="space-y-4">
            {orders.map((order) => (
              <div key={order.id} className="bg-white p-6 rounded-lg shadow border">
                <div className="flex items-start justify-between mb-4">
                  <div>
                    <h3 className="font-semibold text-lg">{order.orderNumber}</h3>
                    <p className="text-sm text-gray-600">{order.customer}</p>
                  </div>
                  <div className="flex items-center space-x-2">
                    <Badge
                      variant={order.type === "Invoice" ? "default" : order.type === "Order" ? "secondary" : "outline"}
                    >
                      {order.type}
                    </Badge>
                    <Badge
                      variant={
                        order.status === "Completed"
                          ? "default"
                          : order.status === "Processing"
                            ? "secondary"
                            : order.status === "Pending"
                              ? "outline"
                              : "destructive"
                      }
                    >
                      {order.status}
                    </Badge>
                  </div>
                </div>

                <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-4">
                  <div>
                    <span className="text-sm text-gray-600">Amount:</span>
                    <p className="font-medium">{formatCurrency(order.amount)}</p>
                  </div>
                  <div>
                    <span className="text-sm text-gray-600">Items:</span>
                    <p className="font-medium">{order.items}</p>
                  </div>
                  <div>
                    <span className="text-sm text-gray-600">Date:</span>
                    <p className="font-medium">{formatDate(order.date)}</p>
                  </div>
                  <div className="flex items-center space-x-2">
                    {order.type === "Order" && (
                      <Button size="sm" variant="outline" onClick={() => handleConvertToInvoice(order.id)}>
                        <FileText className="h-4 w-4 mr-1" />
                        Convert to Invoice
                      </Button>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </TabsContent>
      </Tabs>

      <OrderDialog open={showDialog} onOpenChange={setShowDialog} />
    </div>
  )
}
