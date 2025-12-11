import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api";

export interface Product {
  id: number;
  name: string;
  sku: string;
  category: string;
  price: number;
  stock: number;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductDto {
  name: string;
  sku: string;
  category: string;
  price: number;
  stock: number;
  status: string;
}

export interface UpdateProductDto {
  name: string;
  sku: string;
  category: string;
  price: number;
  stock: number;
  status: string;
}

export interface Order {
  id: number;
  orderNumber: string;
  customer: string;
  type: string;
  amount: number;
  status: string;
  date: string;
  items: number;
  orderItems: OrderItem[];
}

export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface CreateOrderDto {
  customer: string;
  type: string;
  items: CreateOrderItemDto[];
}

export interface CreateOrderItemDto {
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface ForecastDto {
  date: string;
  movingAverage: number;
  linearTrend: number;
  seasonal: number;
  capacity: number;
}

export interface ForecastParametersDto {
  shortTermPeriod: number;
  trendPeriod: number;
  seasonalPeriod: number;
  resourceFactor: number;
}

export interface InventoryAlertDto {
  productId: number;
  productName: string;
  sku: string;
  currentStock: number;
  futureInvoiceQuantities: number;
  shortfall: number;
  severity: string;
  daysUntilStockout: number;
}

export const salesIntelApi = createApi({
  reducerPath: "salesIntelApi",
  baseQuery: fetchBaseQuery({
    baseUrl: API_URL,
  }),
  tagTypes: ["Product", "Order", "Forecast", "Alert"],
  endpoints: (builder) => ({
    // Products
    getProducts: builder.query<
      Product[],
      {
        search?: string;
        sortBy?: string;
        sortDescending?: boolean;
        page?: number;
        pageSize?: number;
      }
    >({
      query: (params) => ({
        url: "/products",
        params,
      }),
      providesTags: ["Product"],
    }),
    getProduct: builder.query<Product, number>({
      query: (id) => `/products/${id}`,
      providesTags: ["Product"],
    }),
    createProduct: builder.mutation<Product, CreateProductDto>({
      query: (product) => ({
        url: "/products",
        method: "POST",
        body: product,
      }),
      invalidatesTags: ["Product"],
    }),
    updateProduct: builder.mutation<
      Product,
      { id: number; product: UpdateProductDto }
    >({
      query: ({ id, product }) => ({
        url: `/products/${id}`,
        method: "PUT",
        body: product,
      }),
      invalidatesTags: ["Product"],
    }),
    deleteProduct: builder.mutation<void, number>({
      query: (id) => ({
        url: `/products/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Product"],
    }),

    // Orders
    getOrders: builder.query<
      Order[],
      {
        search?: string;
        type?: string;
        sortBy?: string;
        sortDescending?: boolean;
        page?: number;
        pageSize?: number;
      }
    >({
      query: (params) => ({
        url: "/orders",
        params,
      }),
      providesTags: ["Order"],
    }),
    createOrder: builder.mutation<Order, CreateOrderDto>({
      query: (order) => ({
        url: "/orders",
        method: "POST",
        body: order,
      }),
      invalidatesTags: ["Order"],
    }),
    convertToInvoice: builder.mutation<Order, number>({
      query: (id) => ({
        url: `/orders/${id}/convert-to-invoice`,
        method: "POST",
      }),
      invalidatesTags: ["Order"],
    }),

    // Forecasting
    generateForecast: builder.mutation<ForecastDto[], ForecastParametersDto>({
      query: (parameters) => ({
        url: "/forecasting/generate",
        method: "POST",
        body: parameters,
      }),
      invalidatesTags: ["Forecast"],
    }),
    getInventoryAlerts: builder.query<InventoryAlertDto[], void>({
      query: () => "/forecasting/inventory-alerts",
      providesTags: ["Alert"],
    }),
    getMovingAverage: builder.query<
      number,
      { productId: number; days?: number }
    >({
      query: ({ productId, days = 7 }) => ({
        url: `/forecasting/moving-average/${productId}`,
        params: { days },
      }),
    }),

    // Seed Data
    reloadSeedData: builder.mutation<{ message: string }, void>({
      query: () => ({
        url: "/seeddata/reload",
        method: "POST",
      }),
      invalidatesTags: ["Product", "Order", "Alert"],
    }),
  }),
});

export const {
  useGetProductsQuery,
  useGetProductQuery,
  useCreateProductMutation,
  useUpdateProductMutation,
  useDeleteProductMutation,
  useGetOrdersQuery,
  useCreateOrderMutation,
  useConvertToInvoiceMutation,
  useGenerateForecastMutation,
  useGetInventoryAlertsQuery,
  useGetMovingAverageQuery,
  useReloadSeedDataMutation,
} = salesIntelApi;
