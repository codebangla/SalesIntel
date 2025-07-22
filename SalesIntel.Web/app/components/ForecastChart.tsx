import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from "recharts"
import type { ForecastDto } from "@/store/api/salesIntelApi"

interface ForecastChartProps {
  data: ForecastDto[]
}

export function ForecastChart({ data }: ForecastChartProps) {
  if (!data || data.length === 0) {
    return (
      <div className="h-[400px] flex items-center justify-center text-gray-500">
        Generate a forecast to view the chart
      </div>
    )
  }

  return (
    <ResponsiveContainer width="100%" height={400}>
      <LineChart data={data}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="date" stroke="#888888" fontSize={12} tickLine={false} axisLine={false} />
        <YAxis stroke="#888888" fontSize={12} tickLine={false} axisLine={false} />
        <Tooltip />
        <Legend />
        <Line type="monotone" dataKey="movingAverage" stroke="#8884d8" strokeWidth={2} name="Moving Average" />
        <Line type="monotone" dataKey="linearTrend" stroke="#82ca9d" strokeWidth={2} name="Linear Trend" />
        <Line type="monotone" dataKey="seasonal" stroke="#ffc658" strokeWidth={2} name="Seasonal" />
        <Line
          type="monotone"
          dataKey="capacity"
          stroke="#ff7300"
          strokeWidth={2}
          strokeDasharray="5 5"
          name="Capacity"
        />
      </LineChart>
    </ResponsiveContainer>
  )
}
