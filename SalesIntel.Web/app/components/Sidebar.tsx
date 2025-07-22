import { NavLink } from "react-router-dom"
import { BarChart3, Package, ShoppingCart, TrendingUp, Home, Settings, AlertTriangle } from "lucide-react"
import { cn } from "@/lib/utils"

const menuItems = [
  {
    title: "Dashboard",
    href: "/",
    icon: Home,
  },
  {
    title: "Products",
    href: "/products",
    icon: Package,
  },
  {
    title: "Orders",
    href: "/orders",
    icon: ShoppingCart,
  },
  {
    title: "Forecasting",
    href: "/forecasting",
    icon: TrendingUp,
  },
  {
    title: "Analytics",
    href: "/analytics",
    icon: BarChart3,
  },
  {
    title: "Alerts",
    href: "/alerts",
    icon: AlertTriangle,
  },
  {
    title: "Settings",
    href: "/settings",
    icon: Settings,
  },
]

export function Sidebar() {
  return (
    <div className="w-64 bg-white shadow-lg">
      <div className="p-6">
        <div className="flex items-center gap-2">
          <BarChart3 className="h-8 w-8 text-blue-600" />
          <h1 className="text-xl font-bold text-gray-900">SalesIntel</h1>
        </div>
      </div>

      <nav className="mt-6">
        <div className="px-3">
          {menuItems.map((item) => (
            <NavLink
              key={item.href}
              to={item.href}
              className={({ isActive }) =>
                cn(
                  "flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors mb-1",
                  isActive ? "bg-blue-50 text-blue-700" : "text-gray-600 hover:bg-gray-50 hover:text-gray-900",
                )
              }
            >
              <item.icon className="h-5 w-5" />
              {item.title}
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  )
}
