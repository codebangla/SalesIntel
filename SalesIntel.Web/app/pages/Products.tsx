"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { Plus, Search, Edit, Trash2 } from "lucide-react"
import { useGetProductsQuery, useDeleteProductMutation } from "@/store/api/salesIntelApi"
import { ProductDialog } from "@/components/ProductDialog"
import { formatCurrency } from "@/lib/utils"
import type { Product } from "@/store/api/salesIntelApi"

export function Products() {
  const [search, setSearch] = useState("")
  const [showDialog, setShowDialog] = useState(false)
  const [editingProduct, setEditingProduct] = useState<Product | null>(null)

  const { data: products = [], isLoading } = useGetProductsQuery({ search })
  const [deleteProduct] = useDeleteProductMutation()

  const handleEdit = (product: Product) => {
    setEditingProduct(product)
    setShowDialog(true)
  }

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure you want to delete this product?")) {
      try {
        await deleteProduct(id).unwrap()
      } catch (error) {
        console.error("Failed to delete product:", error)
      }
    }
  }

  const handleCloseDialog = () => {
    setShowDialog(false)
    setEditingProduct(null)
  }

  if (isLoading) {
    return <div className="p-8">Loading...</div>
  }

  return (
    <div className="container mx-auto py-6 px-4">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Products</h1>
        <Button onClick={() => setShowDialog(true)}>
          <Plus className="mr-2 h-4 w-4" />
          Add Product
        </Button>
      </div>

      <div className="flex items-center space-x-2 mb-6">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search products..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="pl-8"
          />
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {products.map((product) => (
          <div key={product.id} className="bg-white p-6 rounded-lg shadow border">
            <div className="flex items-start justify-between mb-4">
              <div>
                <h3 className="font-semibold text-lg">{product.name}</h3>
                <p className="text-sm text-gray-600">{product.sku}</p>
              </div>
              <div className="flex items-center space-x-2">
                <Button size="sm" variant="outline" onClick={() => handleEdit(product)}>
                  <Edit className="h-4 w-4" />
                </Button>
                <Button size="sm" variant="outline" onClick={() => handleDelete(product.id)}>
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            </div>

            <div className="space-y-2 mb-4">
              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Category:</span>
                <span className="text-sm font-medium">{product.category}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Price:</span>
                <span className="text-sm font-medium">{formatCurrency(product.price)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Stock:</span>
                <span
                  className={`text-sm font-medium ${
                    product.stock === 0 ? "text-red-600" : product.stock < 50 ? "text-yellow-600" : "text-green-600"
                  }`}
                >
                  {product.stock}
                </span>
              </div>
            </div>

            <Badge variant={product.status === "Active" ? "default" : "secondary"}>{product.status}</Badge>
          </div>
        ))}
      </div>

      <ProductDialog open={showDialog} onOpenChange={handleCloseDialog} product={editingProduct} />
    </div>
  )
}
