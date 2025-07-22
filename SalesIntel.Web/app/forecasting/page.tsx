import { ForecastingDashboard } from "@/components/forecasting/forecasting-dashboard"

export default function ForecastingPage() {
  return (
    <div className="container mx-auto py-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Forecasting</h1>
      </div>
      <ForecastingDashboard />
    </div>
  )
}
