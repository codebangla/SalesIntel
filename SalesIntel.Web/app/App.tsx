import { Routes, Route } from "react-router-dom"
import { Layout } from "./components/Layout"
import { Dashboard } from "./pages/Dashboard"
import { Products } from "./pages/Products"
import { Orders } from "./pages/Orders"
import { Forecasting } from "./pages/Forecasting"
import { Analytics } from "./pages/Analytics"
import { Alerts } from "./pages/Alerts"
import { Settings } from "./pages/Settings"
import { Toaster } from "./components/ui/toaster"

function App() {
  return (
    <>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/products" element={<Products />} />
          <Route path="/orders" element={<Orders />} />
          <Route path="/forecasting" element={<Forecasting />} />
          <Route path="/analytics" element={<Analytics />} />
          <Route path="/alerts" element={<Alerts />} />
          <Route path="/settings" element={<Settings />} />
        </Routes>
      </Layout>
      <Toaster />
    </>
  )
}

export default App
