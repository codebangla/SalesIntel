import { ProductsTable } from "@/components/products/products-table"

export default function ProductsPage() {
  return (
    <div className="container mx-auto py-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Products</h1>
      </div>
      <ProductsTable />
    </div>
  )
}
