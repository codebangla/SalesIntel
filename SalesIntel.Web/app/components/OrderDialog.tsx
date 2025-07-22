"use client"

import type React from "react"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { useCreateOrderMutation, useGetProductsQuery } from "@/store/api/salesIntelApi"
import { useToast } from "@/hooks/use-toast"

interface OrderDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function OrderDialog({ open, onOpenChange }: OrderDialogProps) {
  const [formData, setFormData] = useState({
    customer: "",
    type: "Quotation",
    productId: "",
    quantity: 1,
    unitPrice: 0,
  })

  const [createOrder, { isLoading }] = useCreateOrderMutation()
  const { data: products = [] } = useGetProductsQuery({})
  const { toast } = useToast()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await createOrder({
        customer: formData.customer,
        type: formData.type,
        items: [
          {
            productId: Number.parseInt(formData.productId),
            quantity: formData.quantity,
            unitPrice: formData.unitPrice,
          },
        ],
      }).unwrap()

      toast({
        title: "Success",
        description: "Order created successfully",
      })

      setFormData({
        customer: "",
        type: "Quotation",
        productId: "",
        quantity: 1,
        unitPrice: 0,
      })
      onOpenChange(false)
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to create order",
        variant: "destructive",
      })
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Create New Order</DialogTitle>
          <DialogDescription>Create a new order, quotation, or invoice.</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="customer">Customer</Label>
            <Input
              id="customer"
              value={formData.customer}
              onChange={(e) => setFormData({ ...formData, customer: e.target.value })}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="type">Type</Label>
            <Select value={formData.type} onValueChange={(value) => setFormData({ ...formData, type: value })}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Quotation">Quotation</SelectItem>
                <SelectItem value="Order">Order</SelectItem>
                <SelectItem value="Invoice">Invoice</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="product">Product</Label>
            <Select
              value={formData.productId}
              onValueChange={(value) => setFormData({ ...formData, productId: value })}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a product" />
              </SelectTrigger>
              <SelectContent>
                {products.map((product) => (
                  <SelectItem key={product.id} value={product.id.toString()}>
                    {product.name} - {product.sku}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="quantity">Quantity</Label>
              <Input
                id="quantity"
                type="number"
                value={formData.quantity}
                onChange={(e) => setFormData({ ...formData, quantity: Number.parseInt(e.target.value) || 1 })}
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="unitPrice">Unit Price</Label>
              <Input
                id="unitPrice"
                type="number"
                step="0.01"
                value={formData.unitPrice}
                onChange={(e) => setFormData({ ...formData, unitPrice: Number.parseFloat(e.target.value) || 0 })}
                required
              />
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? "Creating..." : "Create Order"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
